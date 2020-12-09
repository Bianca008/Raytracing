using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NWaves.Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay;
    public float InitialPower = 1;
    public int NumberOfMicrophone;
    public int NumberOfRays = 1000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    public Material LineMaterial;
    public GameObject MenuCanvas;

    private double m_frequencyStep = 8192.0;
    private double m_maxFrequency = 22050.0;
    private int m_maxDistance = 200;
    private int m_numberOfReflections = 8;
    private Dictionary<double, Echogram> m_echograms;
    private Echogram m_frequencyResponse;
    private List<double> m_frequencies;
    private Dictionary<int, List<AcousticRay>> m_rays;
    private Dictionary<int, DiscreteSignal> impulseResponses;
    private List<MicrophoneSphere> m_microphones;
    private LineRenderer[] m_intersectedLines;
    private RayGeometry m_rayGeometryGenerator;
    private RaysDrawer m_intersectedRayDrawer;
    private AudioSource m_audioSource;
    private UiConfigurationInput m_configurationInput;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        CreateMicrophones();
        DrawMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();

        ComputeIntensities();
        IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();

        ConvolveSound();
        InitializeUi();
    }

    private void Update()
    {
        if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1 &&
            NumberOfMicrophone <= m_microphones.Count && NumberOfMicrophone >= 1)
        {
            m_intersectedRayDrawer.Draw(NumberOfMicrophone - 1, NumberOfRay - 1);
            IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;
        }
        else
            Debug.Log("The number of ray or the number of microphone does not exist...");

        if (Input.GetKey("i"))
            MenuCanvas.SetActive(false);
        if (Input.GetKey("o"))
            MenuCanvas.SetActive(true);
    }

    private void CreateRays()
    {
        m_rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            m_microphones,
            NumberOfRays,
            m_numberOfReflections,
            m_maxDistance);
        m_rayGeometryGenerator.GenerateRays();
    }

    private List<AcousticRay> RemoveDuplicates(List<AcousticRay> rays)
    {
        var indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].collisionPoints.Count == rays[indexRay + 1].collisionPoints.Count &&
                rays[indexRay].collisionPoints.Count == 0)
            {
                rays.RemoveAt(indexRay);
            }
            else
            if (Math.Abs(rays[indexRay].GetDistance() - rays[indexRay + 1].GetDistance()) < 1e-2 &&
                rays[indexRay].collisionPoints.Count == rays[indexRay + 1].collisionPoints.Count &&
                rays[indexRay].collisionPoints.Count > 0)
            {
                var size = rays[indexRay].collisionPoints.Count;
                var indexPointCompared = 0;
                var ok = false;
                while (indexPointCompared < size && ok == false)
                {
                    double distance = System.Numerics.Vector3.Distance
                    (rays[indexRay].collisionPoints[indexPointCompared],
                        rays[indexRay + 1].collisionPoints[indexPointCompared]);
                    if (distance < 0.06 * rays[indexRay].GetDistance())
                    {
                        ok = true;
                        rays.RemoveAt(indexRay);
                    }

                    ++indexPointCompared;
                }
                if (ok == false)
                    ++indexRay;
            }
            else
                ++indexRay;
        }

        return rays;
    }

    private void CreateIntersectedRaysWithMicrophones()
    {
        m_rays = new Dictionary<int, List<AcousticRay>>();
        foreach (var microphone in m_microphones)
        {
            var newRays = m_rayGeometryGenerator.GetIntersectedRays(microphone);

            newRays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));
            IntersectedRaysWithDuplicate += newRays.Count;
            var raysWithoutDuplicates = RemoveDuplicates(newRays);
            m_rays[microphone.id] = raysWithoutDuplicates;
        }

        var count = 0;
        for (int index = 0; index < m_rays.Count; ++index)
            count += m_rays[index].Count;
        m_intersectedLines = LinesCreator.GenerateLines(count, transform, LineMaterial);
        m_intersectedRayDrawer = new RaysDrawer(m_intersectedLines, m_rays);
    }

    private void CreateMicrophones()
    {
        m_microphones = new List<MicrophoneSphere>
        {
            new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f),
            new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f)
        };
    }

    private void DrawMicrophones()
    {
        foreach (var microphone in m_microphones)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = VectorConverter.Convert(microphone.center);
            sphere.transform.localScale = new UnityEngine.Vector3(microphone.radius,
                microphone.radius,
                microphone.radius);
        }
    }

    private void ComputeIntensities()
    {
        var intensityCalculator = new IntensityCalculator(m_rays, m_microphones, InitialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        var intensities = intensityCalculator.intensities;

        m_frequencies = new List<double>();
        for (double index = 0; index < m_maxFrequency; index += m_maxFrequency / m_frequencyStep)
            m_frequencies.Add(index);

        Debug.Log(m_frequencies.Count);

        m_echograms = new Dictionary<double, Echogram>();
        foreach (var frequency in m_frequencies)
        {
            var phaseCalculator = new PhaseCalculator(m_rays, m_microphones, intensities);
            m_echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }
    }

    private void ComputeFrequencyResponse()
    {
        m_frequencyResponse = new Dictionary<int, List<Complex>>();

        foreach (var microphone in m_microphones)
        {
            var values = new List<Complex>();
            for (int indexFrequency = 0; indexFrequency < m_frequencies.Count; ++indexFrequency)
            {
                var sumi = m_echograms[m_frequencies[indexFrequency]][microphone.id].
                    Aggregate((s, a) => s + a);
                values.Add(sumi);
            }
            m_frequencyResponse[microphone.id] = values;
        }
    }

    private void ConvolveSound()
    {
        impulseResponses = new Dictionary<int, DiscreteSignal>();

        foreach (var freqResponse in m_frequencyResponse)
        {
            impulseResponses[freqResponse.Key] = ImpulseResponseTranformer.Transform(freqResponse.Value);
        }

        SoundConvolver.ConvolveSound(m_audioSource, impulseResponses, m_microphones);
    }

    private void InitializeUi()
    {
        var uiTabController = new UiTabController(MenuCanvas);
        var uiTimeEchogram = new UiTimeEchogram(MenuCanvas);
        var uiFrequencyEchogram = new UiFrequencyEchogram(MenuCanvas);
        var uiImpulseResponse = new UiImpulseResponse(MenuCanvas);

        var uiHandler = new UiHandler(MenuCanvas, uiTimeEchogram, uiFrequencyEchogram, uiImpulseResponse);

        var step = (float)(1 / (2 * m_frequencies[m_frequencies.Count - 1]));

        uiHandler.InitializeUi(m_rays,
            m_microphones,
            m_frequencies,
            m_echograms,
            m_frequencyResponse,
            impulseResponses,
            step);

        m_configurationInput = new UiConfigurationInput(MenuCanvas);
        AddListenerForSoundButton();
        m_configurationInput.setConfiguration.onClick.AddListener(RunSolver);
    }

    private void RunSolver()
    {
        Debug.Log("---------------------Solver started!-----------------------");
        var numberOfReflections = InputHandler.GetNumber(m_configurationInput.numberOfReflections);
        if (numberOfReflections != -1)
            m_numberOfReflections = numberOfReflections;

        var maxDistance = InputHandler.GetNumber(m_configurationInput.maxDistance);
        if (maxDistance != -1)
            m_maxDistance = maxDistance;

        m_frequencyStep = InputHandler.GetCheckedDropdownElement(m_configurationInput.frequencyStep);

        Debug.Log(m_maxFrequency + " fr1  " + m_frequencyStep + " " + m_numberOfReflections + " " + m_maxDistance);

        CreateRays();
        CreateIntersectedRaysWithMicrophones();

        ComputeIntensities();
        IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();

        ConvolveSound();

        Debug.Log("---------------------Solver finished!----------------------");
    }

    private void AddListenerForSoundButton()
    {
        m_configurationInput.soundButton.onClick.AddListener(ShowDialog);
    }

    private void ShowDialog()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with wav", "", "wav");
        StartCoroutine(LoadAudioClip(path));
    }

    private IEnumerator LoadAudioClip(string path)
    {
        if (path.Length != 0)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    m_audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    m_audioSource.clip.name = Path.GetFileNameWithoutExtension(path);
                    m_audioSource.Play();
                    m_maxFrequency = SoundConvolver.GetMaxFrequency(m_audioSource);
                    Debug.Log(m_maxFrequency + " fr  " + m_frequencyStep);
                }
            }
            Debug.Log("---------------------File is loaded!----------------------");
        }
    }
}
