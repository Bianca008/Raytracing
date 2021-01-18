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
using UnityEngine.UI;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay = 1;
    public float InitialPower = 1;
    public int NumberOfMicrophone = 1;
    public int NumberOfRays = 0;
    public int IntersectedRays = 0;
    public int IntersectedRaysWithDuplicate = 0;
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
    private Dictionary<int, DiscreteSignal> m_impulseResponses;
    private List<MicrophoneSphere> m_microphones;
    private LineRenderer[] m_intersectedLines;
    private RayGeometry m_rayGeometryGenerator;
    private RaysDrawer m_intersectedRayDrawer;
    private AudioSource m_audioSource;
    private UiMenuHandler m_menuHandler;
    private UiConfigurationInput m_configurationInput;
    private UiRayMenu m_rayMenu;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_microphones = new List<MicrophoneSphere>();
        m_frequencies = new List<double>();
        m_echograms = new Dictionary<double, Echogram>();
        m_frequencyResponse = new Dictionary<int, List<Complex>>();
        m_impulseResponses = new Dictionary<int, DiscreteSignal>();
        m_rays = new Dictionary<int, List<AcousticRay>>();

        InitializeUi();
    }

    private void Update()
    {
        //if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1 &&
        //    NumberOfMicrophone <= m_microphones.Count && NumberOfMicrophone >= 1)
        //{
        //    m_intersectedRayDrawer.Draw(NumberOfMicrophone - 1, NumberOfRay - 1);
        //    IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;
        //}
        //else
        //    Debug.Log("The number of ray or the number of microphone does not exist...");

        //if (Input.GetKey("i"))
        //    MenuCanvas.SetActive(false);
        //if (Input.GetKey("o"))
        //    MenuCanvas.SetActive(true);

       // Pressed();
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
        m_rays.Clear();
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
        m_microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f));
        m_microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f));
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
            sphere.AddComponent<BoxCollider>();
        }
    }

    private void ComputeIntensities()
    {
        var intensityCalculator = new IntensityCalculator(m_rays, m_microphones, InitialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        var intensities = intensityCalculator.intensities;

        m_frequencies.Clear();
        for (double index = 0; index < m_maxFrequency; index += m_maxFrequency / m_frequencyStep)
            m_frequencies.Add(index);

        m_echograms.Clear();
        foreach (var frequency in m_frequencies)
        {
            var phaseCalculator = new PhaseCalculator(m_rays, m_microphones, intensities);
            m_echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }
    }

    private void ComputeFrequencyResponse()
    {
        m_frequencyResponse.Clear();

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
        m_impulseResponses.Clear();

        foreach (var freqResponse in m_frequencyResponse)
        {
            m_impulseResponses[freqResponse.Key] = ImpulseResponseTranformer.Transform(freqResponse.Value);
        }

        SoundConvolver.ConvolveSound(m_audioSource, m_impulseResponses, m_microphones);
    }

    private void InitializeUi()
    {
        m_configurationInput = new UiConfigurationInput(MenuCanvas);
        AddListenerForSoundButton();
        //m_configurationInput.setConfiguration.onClick.AddListener(() =>
        //{
        //    SetVisibilityForLoadingText();
        //});
        m_configurationInput.setConfiguration.onClick.AddListener(RunSolver);

        var uiTabController = new UiTabController(MenuCanvas);
        var uiTimeEchogram = new UiTimeEchogram(MenuCanvas);
        var uiFrequencyEchogram = new UiFrequencyEchogram(MenuCanvas);
        var uiImpulseResponse = new UiImpulseResponse(MenuCanvas);

        var uiHandler = new UiHandler(MenuCanvas, uiTimeEchogram, uiFrequencyEchogram, uiImpulseResponse);

        var step = (m_frequencies.Count == 0) ? 0 : (float)(1 / (2 * m_frequencies[m_frequencies.Count - 1]));

        uiHandler.InitializeUi(m_rays,
            m_microphones,
            m_frequencies,
            m_echograms,
            m_frequencyResponse,
            m_impulseResponses,
            step);

        m_rayMenu = new UiRayMenu();
        m_menuHandler = new UiMenuHandler();
        m_menuHandler.AddListenerForMenuButton(MenuCanvas);
        m_menuHandler.AddListenerForRayMenuButton(m_rayMenu.canvasMenu);
        AddListenerForShowButton();
    }

    private void SetVisibilityForLoadingText()
    {
        Text loadingText = MenuCanvas.transform.Find("TabPanel").
                        gameObject.transform.Find("TabPanels").
                        gameObject.transform.Find("InputTabPanel").
                        gameObject.transform.Find("LoadingText").GetComponent<Text>();
        Color visibleColor = loadingText.color;
        visibleColor.a = 1 - visibleColor.a;

        loadingText.color = visibleColor;
    }

    private void RunSolver()
    {
        SetVisibilityForLoadingText();
        Debug.Log("---------------------Solver started!-----------------------");
        if (m_microphones.Count == 0)
        {
            CreateMicrophones();
            DrawMicrophones();
        }

        var numberOfReflections = InputHandler.GetNumber(m_configurationInput.numberOfReflections);
        if (numberOfReflections != -1)
            m_numberOfReflections = numberOfReflections;

        var maxDistance = InputHandler.GetNumber(m_configurationInput.maxDistance);
        if (maxDistance != -1)
            m_maxDistance = maxDistance;

        m_frequencyStep = InputHandler.GetCheckedDropdownElement(m_configurationInput.frequencyStep);

        Debug.Log("Parameters: number of reflections: " + m_numberOfReflections +
            " max distance: " + m_maxDistance +
            " number of steps: " + m_frequencyStep +
            " sound: " + m_audioSource.clip.name);

        Debug.Log("The geometric calculation starts.");
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        Debug.Log("The geometric calculation ends.");

        Debug.Log("The physical calculation starts.");
        ComputeIntensities();
        IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();
        Debug.Log("The physical calculation ends.");

        Debug.Log("The sound convolution starts.");
        ConvolveSound();
        Debug.Log("The sound convolution ends.");

        Debug.Log("---------------------Solver finished!----------------------");
        //SetVisibilityForLoadingText();
    }

    private void Pressed()
    {
        if (Input.GetMouseButtonDown(0) == false)
            return;

        var clickedPosition = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

        Debug.Log("clicked: " + clickedPosition);
        //Debug.DrawLine(Camera.main.transform.position, clickedPosition);

        foreach (MicrophoneSphere microphone in m_microphones)
        {
            if (microphone.IsAroundMicro(VectorConverter.Convert(clickedPosition)) == true && SoundConvolver.convolvedSounds.Count > 0)
            {
                float[] f = SoundConvolver.convolvedSounds[microphone.id];
                var clip = AudioClip.Create("testSound", f.Length, 1, 44100, false, false);
                clip.SetData(f, 0);
                AudioSource.PlayClipAtPoint(clip, clickedPosition, 1.0f);
            }
        }
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
                }
            }
            Debug.Log("---------------------File is loaded!----------------------");
        }
    }

    public void AddListenerForShowButton()
    {
        m_rayMenu.showButton.onClick.AddListener(()=>
        { m_intersectedRayDrawer.Draw(Int32.Parse(m_rayMenu.microphoneNumber.text), Int32.Parse(m_rayMenu.rayNumber.text)); });
    }
}
