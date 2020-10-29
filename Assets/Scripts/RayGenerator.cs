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
    public InputField numberOfMicrophoneInputField;
    public InputField frequencyInputField;

    private readonly int maxDistance = 200;
    private int numberOfReflections = 8;
    private Dictionary<double, Echogram> echograms;
    private Dictionary<int, List<Complex>> frequencyReponse;
    private List<double> frequencies;
    private Dictionary<int, List<AcousticRay>> rays;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer intersectedRayDrawer;
    private List<MicrophoneSphere> microphones;
    private ChartDrawer chartDrawer;
    private AudioSource audioSource;

    private void Start()
    {
        AddListeneForShowButton();
        audioSource = GetComponent<AudioSource>();

        CreateMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        DrawMicrophones();

        ComputeIntensities();
        //WriteToFileTimePressure();
        IntersectedRays = rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyReponse();
        FileHandler.WriteFrquencies(frequencyReponse, microphones);

        SoundConvolver.ConvolveSound(audioSource, frequencyReponse, microphones);
    }

    private void Update()
    {
        if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1 &&
            NumberOfMicrophone <= microphones.Count && NumberOfMicrophone >= 1)
        {
            intersectedRayDrawer.Draw(NumberOfMicrophone - 1, NumberOfRay - 1);
            IntersectedRays = rays[NumberOfMicrophone - 1].Count;
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
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            microphones,
            NumberOfRays,
            numberOfReflections,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
    }

    private List<AcousticRay> RemoveDuplicates(List<AcousticRay> rays)
    {
        int indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count &&
                rays[indexRay].CollisionPoints.Count == 0)
            {
                rays.RemoveAt(indexRay);
            }
            else
            if (Math.Abs(rays[indexRay].Distance - rays[indexRay + 1].Distance) < 1e-2 &&
                rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count &&
                rays[indexRay].CollisionPoints.Count > 0)
            {
                int size = rays[indexRay].CollisionPoints.Count;
                int indexPointCompared = 0;
                bool ok = false;
                while (indexPointCompared < size && ok == false)
                {
                    double distance = System.Numerics.Vector3.Distance
                    (rays[indexRay].CollisionPoints[indexPointCompared],
                        rays[indexRay + 1].CollisionPoints[indexPointCompared]);
                    if (distance < 0.06 * rays[indexRay].Distance)
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
        rays = new Dictionary<int, List<AcousticRay>>();
        for (int indexMicrophone = 0; indexMicrophone < microphones.Count; ++indexMicrophone)
        {
            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphones[indexMicrophone]);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);

            });
            IntersectedRaysWithDuplicate += newRays.Count;
            List<AcousticRay> raysWithoutDuplicates = RemoveDuplicates(newRays);
            rays[microphones[indexMicrophone].Id] = raysWithoutDuplicates;
        }

        int count = 0;
        for (int index = 0; index < rays.Count; ++index)
            count += rays[index].Count;
        intersectedLines = LinesCreator.GenerateLines(count, transform, LineMaterial);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, rays);
    }

    private void CreateMicrophones()
    {
        microphones = new List<MicrophoneSphere>();
        microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f));
        microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(-1.5f, 1.2f, 1.7f), 0.1f));
    }

    private void DrawMicrophones()
    {
        foreach (var microphone in microphones)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = VectorConverter.Convert(microphone.Center);
            sphere.transform.localScale = new UnityEngine.Vector3(microphone.Radius,
                microphone.Radius,
                microphone.Radius);
        }
    }

    private void ComputeIntensities()
    {
        IntensityCalculator intensityCalculator = new IntensityCalculator(rays, microphones, InitialPower);
        intensityCalculator.ComputePower();
        intensityCalculator.TransformIntensitiesToPressure();
        Dictionary<int, List<double>> intensities = intensityCalculator.Intensities;

        frequencies = new List<double>();
        for (double index = 0; index < 22050; index += 22050.0 / 8192.0)
            frequencies.Add(index);

        Debug.Log(frequencies.Count);

        echograms = new Dictionary<double, Echogram>();
        foreach (double frequency in frequencies)
        {
            PhaseCalculator phaseCalculator = new PhaseCalculator(rays, microphones, intensities);
            echograms[frequency] = phaseCalculator.ComputePhase(frequency);
        }
    }

    private void ComputeFrequencyReponse()
    {
        frequencyReponse = new Dictionary<int, List<Complex>>();

        for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
        {
            List<Complex> values = new List<Complex>();
            for (int indexFrequencie = 0; indexFrequencie < frequencies.Count; ++indexFrequencie)
            {
                Complex sumi = echograms[frequencies[indexFrequencie]][microphones[indexMicro].Id].
                    Aggregate((s, a) => s + a);
                values.Add(sumi);
            }
            frequencyReponse[microphones[indexMicro].Id] = values;
        }
    }

    private void AddListeneForShowButton()
    {
        ButtonHandler buttonHandler = new ButtonHandler();

        ShowButton.onClick.AddListener(() =>
        {
            buttonHandler.ShowFrequencyChart(numberOfMicrophoneInputField,
                                      MenuCanvas,
                                      chartDrawer,
                                      frequencies,
                                      microphones);
        });
    }

    private void DrawChart(int indexMicrophone, double indexFrequencie)
    {
        string timeMagnitudeFile = "results/timeMagnitude" +
                (indexMicrophone).ToString() + "M" +
                indexFrequencie.ToString() + "Hz.txt";
        string timePhaseFile = "results/timePhase" +
                (indexMicrophone).ToString() + "M" +
                indexFrequencie.ToString() + "Hz.txt";

        Tuple<List<float>, List<float>> timePhase = FileHandler.ReadFromFile(timePhaseFile);
        Tuple<List<float>, List<float>> timeMagnitude = FileHandler.ReadFromFile(timeMagnitudeFile);

        for (int index = 0; index < timeMagnitude.Item1.Count; ++index)
            timeMagnitude.Item1[index] = (float)Math.Round(timeMagnitude.Item1[index] * 1000, 2);

        chartDrawer = new ChartDrawer(MenuCanvas);
        chartDrawer.Draw(timeMagnitude.Item1, timeMagnitude.Item2, timePhase.Item2);
    }
}
