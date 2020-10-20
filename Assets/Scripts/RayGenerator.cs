using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay;
    public float InitialPower = 1;
    public int NumberOfMicrophone;
    public int NumberOfRays = 1000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    public Material LineMaterial;
    public GameObject ChartArea;

    private int previousChartForMicrophone;
    private readonly int maxDistance = 200;
    private int numberOfReflections = 3;
    private Dictionary<int, List<Complex>> intensities;
    private Dictionary<int, List<AcousticRay>> rays;
    private LineRenderer[] lines;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;
    private RaysDrawer intersectedRayDrawer;
    private List<MicrophoneSphere> microphones;
    private ChartDrawer chartDrawer;

    private void Start()
    {
        CreateMicrophones();
        CreateRays();
        CreateIntersectedRaysWithMicrophones();
        DrawMicrophones();

        ComputeIntensities();
        WriteToFileTimePressure();
        IntersectedRays = rays[NumberOfMicrophone - 1].Count;
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

        if (previousChartForMicrophone != NumberOfMicrophone && NumberOfMicrophone >= 1 && NumberOfMicrophone <= microphones.Count)
        {
            DrawChart(NumberOfMicrophone);
            previousChartForMicrophone = NumberOfMicrophone;
        }
        else
            Debug.Log("The chart you want to see does not exists.");

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
        intensities = intensityCalculator.Intensities;

        PhaseCalculator phaseCalculator = new PhaseCalculator(rays, microphones, intensities);
        phaseCalculator.ComputePhase(1000);
        intensities = phaseCalculator.Echogram;
    }

    private void WriteToFileTimePressure()
    {
        DistanceCalculator distanceCalculator = new DistanceCalculator(rays, microphones);
        distanceCalculator.ComputeDistances();

        Dictionary<int, List<float>> times = TimeCalculator.GetTime(rays, microphones);
      
        for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
        {
            List<float> xTime = times[microphones[indexMicro].Id];
            List<float> yPressure = new List<float>();
            List<Complex> microphoneIntensities = intensities[microphones[indexMicro].Id];
            for (int index = 0; index < microphoneIntensities.Count; ++index)
                //yPressure.Add((float)(microphoneIntensities[index].Phase * 180/Math.PI));
                yPressure.Add((float)microphoneIntensities[index].Magnitude);

            WriteTimeAndPressure(xTime, yPressure, "timePressure" + (microphones[indexMicro].Id + 1).ToString() + ".txt");
        }
    }

    private void WriteTimeAndPressure(List<float> time, List<float> pressure, string fileName)
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName, false))
        {
            for (int index = 0; index < time.Count; ++index)
                file.WriteLine(time[index] + " " + pressure[index]);
        }
    }

    private void DrawChart(int indexMicrophone)
    {
        List<float> xTime = new List<float>();
        List<float> yPressure = new List<float>();
        using (System.IO.StreamReader file = new System.IO.StreamReader("timePressure" + indexMicrophone.ToString() + ".txt"))
        {
            while (!file.EndOfStream)
            {
                string text = file.ReadLine();
                string[] bits = text.Split(' ');
                xTime.Add(System.Single.Parse(bits[0]));
                yPressure.Add(System.Single.Parse(bits[1]));
            }
        }
        chartDrawer = new ChartDrawer(ChartArea);
        chartDrawer.Draw(xTime, yPressure);
    }
}
