using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public Button ShowButton;
    public Button ShowFrequencyEchogramButton;
    public InputField NumberOfMicrophoneInputField;

    private const int maxDistance = 200;
    private readonly int numberOfReflections = 8;
    private Dictionary<double, Echogram> m_Echograms;
    private Dictionary<int, List<Complex>> m_FrequencyResponse;
    private List<double> m_Frequencies;
    private Dictionary<int, List<AcousticRay>> m_Rays;
    private LineRenderer[] m_IntersectedLines;
    private RayGeometry m_RayGeometryGenerator;
    private RaysDrawer m_IntersectedRayDrawer;
    private List<MicrophoneSphere> m_Microphones;
    private ChartDrawer m_ChartDrawer;
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        CreateMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        DrawMicrophones();

        ComputeIntensities();
        //WriteToFileTimePressure();
        IntersectedRays = m_Rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyResponse();
        FileHandler.WriteFrequencies(m_FrequencyResponse, m_Microphones);

        SoundConvolver.ConvolveSound(m_AudioSource, m_FrequencyResponse, m_Microphones);
        InitializeUi();
    }

    private void Update()
    {
        if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1 &&
            NumberOfMicrophone <= m_Microphones.Count && NumberOfMicrophone >= 1)
        {
            m_IntersectedRayDrawer.Draw(NumberOfMicrophone - 1, NumberOfRay - 1);
            IntersectedRays = m_Rays[NumberOfMicrophone - 1].Count;
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
        m_RayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            m_Microphones,
            NumberOfRays,
            numberOfReflections,
            maxDistance);
        m_RayGeometryGenerator.GenerateRays();
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
        m_Rays = new Dictionary<int, List<AcousticRay>>();
        foreach (var microphone in m_Microphones)
        {
            List<AcousticRay> newRays = m_RayGeometryGenerator.GetIntersectedRays(microphone);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());

            });
            IntersectedRaysWithDuplicate += newRays.Count;
            var raysWithoutDuplicates = RemoveDuplicates(newRays);
            m_Rays[microphone.id] = raysWithoutDuplicates;
        }

        var count = 0;
        for (int index = 0; index < m_Rays.Count; ++index)
            count += m_Rays[index].Count;
        m_IntersectedLines = LinesCreator.GenerateLines(count, transform, LineMaterial);
        m_IntersectedRayDrawer = new RaysDrawer(m_IntersectedLines, m_Rays);
    }

    private void CreateMicrophones()
    {
        m_Microphones = new List<MicrophoneSphere>
        {
            new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f),
            new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f)
        };
    }

    private void DrawMicrophones()
    {
        foreach (var microphone in m_Microphones)
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
        var intensityCalculator = new IntensityCalculator(m_Rays, m_Microphones, InitialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        var intensities = intensityCalculator.intensities;

        m_Frequencies = new List<double>();
        for (double index = 0; index < 22050; index += 22050.0 / 8192.0)
            m_Frequencies.Add(index);

        Debug.Log(m_Frequencies.Count);

        m_Echograms = new Dictionary<double, Echogram>();
        foreach (var frequency in m_Frequencies)
        {
            var phaseCalculator = new PhaseCalculator(m_Rays, m_Microphones, intensities);
            m_Echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }
    }

    private void ComputeFrequencyResponse()
    {
        m_FrequencyResponse = new Dictionary<int, List<Complex>>();

        foreach (var microphone in m_Microphones)
        {
            var values = new List<Complex>();
            for (int indexFrequency = 0; indexFrequency < m_Frequencies.Count; ++indexFrequency)
            {
                var sumi = m_Echograms[m_Frequencies[indexFrequency]][microphone.id].
                    Aggregate((s, a) => s + a);
                values.Add(sumi);
            }
            m_FrequencyResponse[microphone.id] = values;
        }
    }

    private void AddListenerForShowButton()
    {
        var buttonHandler = new ButtonHandler();

        ShowButton.onClick.AddListener(() =>
        {
            buttonHandler.ShowFrequencyChart(NumberOfMicrophoneInputField,
                                      MenuCanvas,
                                      m_ChartDrawer,
                                      m_Frequencies,
                                      m_Microphones);
        });
    }

    private void AddListenerForShowFrequencyEchogram()
    {
        var buttonHandler = new ButtonHandler();

        ShowFrequencyEchogramButton.onClick.AddListener(SetActiveFrequncyEchogramUi);
    }

    private void InitializeUi()
    {
        var inputPanel = GameObject.Find("InputPanel");
        inputPanel.SetActive(false);
        var chartPanel = GameObject.Find("ChartPanel");
        chartPanel.SetActive(false);

        AddListenerForShowFrequencyEchogram();
        AddListenerForShowButton();
    }

    private void SetActiveFrequncyEchogramUi()
    {
        var inputPanel = MenuCanvas.transform.Find("InputPanel").gameObject;
        Debug.Log(inputPanel);
        inputPanel.SetActive(true);
        var buttonsAndPlotPanel= MenuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var chartPanel = buttonsAndPlotPanel.transform.Find("ChartPanel").gameObject;
        chartPanel.SetActive(true);
    }

    private void DrawChart(int indexMicrophone, double indexFrequency)
    {
        var timeMagnitudeFile = "results/timeMagnitude" +
                                (indexMicrophone).ToString() + "M" +
                                indexFrequency.ToString() + "Hz.txt";
        var timePhaseFile = "results/timePhase" +
                            (indexMicrophone).ToString() + "M" +
                            indexFrequency.ToString() + "Hz.txt";

        var timePhase = FileHandler.ReadFromFile(timePhaseFile);
        var timeMagnitude = FileHandler.ReadFromFile(timeMagnitudeFile);

        for (int index = 0; index < timeMagnitude.Item1.Count; ++index)
            timeMagnitude.Item1[index] = (float)Math.Round(timeMagnitude.Item1[index] * 1000, 2);

        m_ChartDrawer = new ChartDrawer(MenuCanvas);
        m_ChartDrawer.Draw(timeMagnitude.Item1, timeMagnitude.Item2, timePhase.Item2);
    }
}
