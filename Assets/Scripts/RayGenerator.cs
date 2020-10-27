using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using NWaves.Signals;

using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;
using System.IO;
using NWaves.Audio;
using NWaves.Operations;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay;
    public float InitialPower = 1;
    public int NumberOfMicrophone;
    public int Frequencie;
    public int NumberOfRays = 1000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    public Material LineMaterial;
    public GameObject ChartArea;

    private int previousChartForMicrophone;
    private double previousCharFrequencie;
    private readonly int maxDistance = 200;
    private int numberOfReflections = 8;
    private Dictionary<double, Echogram> echograms;
    private Dictionary<int, List<Complex>> frequencyReponse;
    private List<double> frequencies;
    private Dictionary<int, List<AcousticRay>> rays;
    private LineRenderer[] lines;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;
    private RaysDrawer intersectedRayDrawer;
    private List<MicrophoneSphere> microphones;
    private ChartDrawer chartDrawer;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        CreateMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        DrawMicrophones();

        ComputeIntensities();
        //WriteToFileTimePressure();
        IntersectedRays = rays[NumberOfMicrophone - 1].Count;

        ComputeFrequencyReponse();
        WriteFrquencies();

        ConvolveSound();
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

        //if ((previousChartForMicrophone != NumberOfMicrophone ||
        //    previousCharFrequencie != Frequencie) &&
        //    NumberOfMicrophone >= 1 &&
        //    NumberOfMicrophone <= microphones.Count &&
        //    frequencies.Contains(Frequencie) == true)
        //{
        //    DrawChart(NumberOfMicrophone, Frequencie);
        //    previousChartForMicrophone = NumberOfMicrophone;
        //    previousCharFrequencie = Frequencie;
        //}
        //else
        //    Debug.Log("The chart you want to see does not exists.");

        if (previousChartForMicrophone != NumberOfMicrophone &&
            NumberOfMicrophone >= 1 &&
            NumberOfMicrophone <= microphones.Count)
        {
            DrawChartFrequency(NumberOfMicrophone);
            previousChartForMicrophone = NumberOfMicrophone;
        }

        if (Input.GetKey("i"))
            chartDrawer.Enable = false;
        if (Input.GetKey("o"))
            chartDrawer.Enable = true;
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
        for (int index = 0; index < microphones.Count; ++index)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = VectorConverter.Convert(microphones[0].Center);
            sphere.transform.localScale = new UnityEngine.Vector3(microphones[0].Radius, microphones[0].Radius, microphones[0].Radius);
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
        for (int index = 0; index < frequencies.Count; ++index)
        {
            PhaseCalculator phaseCalculator = new PhaseCalculator(rays, microphones, intensities);
            echograms[frequencies[index]] = phaseCalculator.ComputePhase(frequencies[index]);
        }
    }

    private void WriteToFileTimePressure()
    {
        DistanceCalculator distanceCalculator = new DistanceCalculator(rays, microphones);
        distanceCalculator.ComputeDistances();

        Dictionary<int, List<float>> times = TimeCalculator.GetTime(rays, microphones);

        for (int indexFrequencie = 0; indexFrequencie < frequencies.Count; ++indexFrequencie)
            for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
            {
                List<float> xTime = times[microphones[indexMicro].Id];
                List<float> yMagnitude = new List<float>();
                List<float> yPhase = new List<float>();
                List<Complex> microphoneIntensities = echograms[frequencies[indexFrequencie]][microphones[indexMicro].Id];
                for (int index = 0; index < microphoneIntensities.Count; ++index)
                {
                    yPhase.Add((float)(microphoneIntensities[index].Phase * 180 / Math.PI));
                    yMagnitude.Add((float)microphoneIntensities[index].Magnitude);
                }

                WriteToFile(xTime, yMagnitude, "timeMagnitude" +
                    (microphones[indexMicro].Id + 1).ToString() + "M" + frequencies[indexFrequencie].ToString() + "Hz.txt");

                WriteToFile(xTime, yPhase, "timePhase" +
                   (microphones[indexMicro].Id + 1).ToString() + "M" + frequencies[indexFrequencie].ToString() + "Hz.txt");
            }
    }

    private void WriteToFile(List<float> x, List<float> y, string fileName)
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName, false))
        {
            for (int index = 0; index < x.Count; ++index)
                file.WriteLine(x[index] + " " + y[index]);
        }
    }

    private void DrawChart(int indexMicrophone, double indexFrequencie)
    {
        string timeMagnitudeFile = "timeMagnitude" +
                (indexMicrophone).ToString() + "M" +
                indexFrequencie.ToString() + "Hz.txt";
        string timePhaseFile = "timePhase" +
                (indexMicrophone).ToString() + "M" +
                indexFrequencie.ToString() + "Hz.txt";

        Tuple<List<float>, List<float>> timePhase = FileReader.ReadFromFile(timePhaseFile);
        Tuple<List<float>, List<float>> timeMagnitude = FileReader.ReadFromFile(timeMagnitudeFile);

        for (int index = 0; index < timeMagnitude.Item1.Count; ++index)
            timeMagnitude.Item1[index] = (float)Math.Round(timeMagnitude.Item1[index] * 1000, 2);

        chartDrawer = new ChartDrawer(ChartArea);
        chartDrawer.Draw(timeMagnitude.Item1, timeMagnitude.Item2, timePhase.Item2);
    }

    private void DrawChartFrequency(int indexMicrophone)
    {
        string file = indexMicrophone.ToString() + "M.txt";
        Tuple<List<float>, List<float>> magnitudeAndPhse = FileReader.ReadFromFile(file);
        List<float> freq = new List<float>();
        for (int index = 0; index < frequencies.Count; ++index)
            freq.Add((float)frequencies[index]);
        chartDrawer = new ChartDrawer(ChartArea);
        chartDrawer.DrawFrequencieChart(freq, magnitudeAndPhse.Item1, magnitudeAndPhse.Item2);
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

        //using (System.IO.StreamWriter file =
        //   new System.IO.StreamWriter("frecvReIm8Fr.txt", false))
        //{
        //    //for (int indexFrequencie = 0; indexFrequencie < frequencies.Count; ++indexFrequencie)
        //    for (int index = 0; index < frequencyReponse[microphones[0].Id].Count; ++index)
        //    {
        //        file.WriteLine(frequencies[index] + " " +
        //            frequencyReponse[microphones[0].Id][index].Real + " " +
        //            frequencyReponse[microphones[0].Id][index].Imaginary + " ");
        //    }
        //}
    }

    private void WriteFrquencies()
    {
        for (int indexMicro = 0; indexMicro < frequencyReponse.Count; ++indexMicro)
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            for (int index = 0; index < frequencyReponse[microphones[indexMicro].Id].Count; ++index)
            {
                x.Add((float)frequencyReponse[microphones[indexMicro].Id][index].Magnitude);
                y.Add((float)frequencyReponse[microphones[indexMicro].Id][index].Phase);
            }
            WriteToFile(x, y, microphones[indexMicro].Id.ToString() + "M.txt");
        }
    }

    private void ConvolveSound()
    {
        DiscreteSignal attention;

        // load
        using (var stream = new FileStream(audioSource.clip.name + ".wav", FileMode.Open))
        {
            var waveFile = new WaveFile(stream);
            attention = waveFile[Channels.Left];
        }

        Debug.Log(frequencyReponse[0].Count);
        NWaves.Transforms.RealFft value = new NWaves.Transforms.RealFft(frequencyReponse[0].Count);

        List<float> re = new List<float>();
        List<float> im = new List<float>();

        for(int index=0;index<frequencyReponse[0].Count;++index)
        {
            re.Add((float)frequencyReponse[0][index].Real);
            im.Add((float)frequencyReponse[0][index].Imaginary);
        }

        float[] outputArray = new float[re.Count];
        value.Inverse(re.ToArray(), im.ToArray(), outputArray);
        float maxi = outputArray.Max();
        for (int index = 0; index < outputArray.Length; ++index)
            outputArray[index] /= maxi;

        Debug.Log(outputArray[0]);
        DiscreteSignal impulseRespone = new DiscreteSignal(22050, outputArray);
        DiscreteSignal convolutionResult = Operation.Convolve(attention, impulseRespone);

        // save
        using (var stream = new FileStream("convolutionAttention.wav", FileMode.Create))
        {
            var waveFile = new WaveFile(convolutionResult);
            waveFile.SaveTo(stream);
        }
    }
}
