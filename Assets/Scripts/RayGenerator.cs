using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NWaves.Signals;
using UnityEngine;

using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;
using UnityEngine.UI;

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

    private int m_maxDistance = 200;
    private int m_numberOfReflections = 8;
    private Dictionary<double, Echogram> m_echograms;
    private Dictionary<int, List<Complex>> m_frequencyResponse;
    private List<double> m_frequencies;
    private Dictionary<int, List<AcousticRay>> m_rays;
    private Dictionary<int, DiscreteSignal> impulseResponses;
    private LineRenderer[] m_intersectedLines;
    private RayGeometry m_rayGeometryGenerator;
    private RaysDrawer m_intersectedRayDrawer;
    private List<MicrophoneSphere> m_microphones;
    private AudioSource m_audioSource;
    private UiConfigurationInput m_configurationInput;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        CreateMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        DrawMicrophones();

        ComputeIntensities();
        IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();
        FileHandler.WriteFrequencies(m_frequencyResponse, m_microphones);

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
        for (double index = 0; index < 22050; index += 22050.0 / 8192.0)
            m_frequencies.Add(index);

        Debug.Log(m_frequencies.Count);

        m_echograms = new Dictionary<double, Echogram>();
        foreach (var frequency in m_frequencies)
        {
            var phaseCalculator = new PhaseCalculator(m_rays, m_microphones, intensities);
            m_echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }

        /*Se poate comenta linia 194 dupa ce s-au generat fisierele macar o data.*/
        //FileHandler.WriteToFileTimePressure(m_Echograms, m_Rays, m_Microphones, m_Frequencies);
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

        uiHandler.InitializeUi(impulseResponses, m_microphones, m_frequencies, step);

        m_configurationInput = new UiConfigurationInput(MenuCanvas);
        m_configurationInput.setConfiguration.onClick.AddListener(RunSolver);
    }

    private void RunSolver()
    {
        var numberOfReflections = InputHandler.GetNumber(m_configurationInput.numberOfReflections);
        if (numberOfReflections != -1)
            m_numberOfReflections = numberOfReflections;

        var maxDistance = InputHandler.GetNumber(m_configurationInput.maxDistance);
        if (maxDistance != -1)
            m_maxDistance = maxDistance;

        var frequencyStep = InputHandler.GetCheckedDropdownElement(m_configurationInput.frequencyStep);

        CreateRays();
        CreateIntersectedRaysWithMicrophones();

        ComputeIntensities();
        IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();
        FileHandler.WriteFrequencies(m_frequencyResponse, m_microphones);

        ConvolveSound();
    }
}
