using System;
using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay;
    public float InitialPower = 1;
    public int ChartForMicrophone;
    public int NumberOfRays = 1000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    public Material LineMaterial;
    public GameObject ChartArea;

    private int previousChartForMicrophone;
    private readonly int maxDistance = 200;
    private int numberOfReflections = 3;
    private List<double> intensities;
    private List<AcousticRay> rays;
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
    }

    private void Update()
    {
        if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1)
            intersectedRayDrawer.Draw(NumberOfRay - 1);
        else
            Debug.Log("The number of ray does not exist...");

        if (previousChartForMicrophone != ChartForMicrophone && ChartForMicrophone >= 1 && ChartForMicrophone <= microphones.Count)
        {
            DrawChart(ChartForMicrophone);
            previousChartForMicrophone = ChartForMicrophone;
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
            if (Math.Abs(rays[indexRay].Distance - rays[indexRay + 1].Distance) < 1e-5 &&
                rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count &&
                rays[indexRay].CollisionPoints.Count > 0)
            {
                int size = rays[indexRay].CollisionPoints.Count;
                int indexPointCompared = 0;
                bool ok = false;
                while (indexPointCompared < size && ok == false)
                {
                    if (System.Numerics.Vector3.Distance
                    (rays[indexRay].CollisionPoints[indexPointCompared],
                        rays[indexRay + 1].CollisionPoints[indexPointCompared]) <
                        0.9 * rays[indexRay].Distance)
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

        IntersectedRays += rays.Count;
        return rays;
        /*int indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].CollisionPoints.Count == rays[indexRay + 1].CollisionPoints.Count)
            {
                int size = rays[indexRay].CollisionPoints.Count;
                double epsilon = 1e-3;
                int indexPointCompared = 0;
                bool okToDelete = true;
                while (indexPointCompared < size)
                {
                    if (System.Numerics.Vector3.Distance
                        (rays[indexRay].CollisionPoints[indexPointCompared],
                        rays[indexRay + 1].CollisionPoints[indexPointCompared]) > epsilon)
                    {
                        okToDelete = false;
                    }

                    ++indexPointCompared;
                }

                if (okToDelete == true)
                    rays.RemoveAt(indexRay);
                else
                    ++indexRay;
            }
            else
                ++indexRay;
        }

        IntersectedRays += rays.Count;

        return rays;*/
    }

    private void CreateIntersectedRaysWithMicrophones()
    {
        rays = new List<AcousticRay>();
        for (int indexMicrophone = 0; indexMicrophone < microphones.Count; ++indexMicrophone)
        {
            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphones[indexMicrophone]);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);

            });
            IntersectedRaysWithDuplicate += newRays.Count;
            List<AcousticRay> raysWithoutDuplicates = RemoveDuplicates(newRays);
            for (int indexRay = 0; indexRay < raysWithoutDuplicates.Count; ++indexRay)
                rays.Add(raysWithoutDuplicates[indexRay]);
        }
        intersectedLines = LinesCreator.GenerateLines(rays.Count, transform, LineMaterial);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, rays);
    }

    private void CreateMicrophones()
    {
        microphones = new List<MicrophoneSphere>();
        microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f));
        microphones.Add(new MicrophoneSphere(new System.Numerics.Vector3(-2, 1.6f, 1.7f), 0.1f));
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
        IntensityCalculator intensityCalculator = new IntensityCalculator(rays, InitialPower);
        intensityCalculator.ComputePower();
        intensities = intensityCalculator.Intensities;
    }

    private void WriteToFileTimePressure()
    {
        DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        distanceCalculator.ComputeDistances();

        List<List<double>> times = TimeCalculator.GetTime(rays);
        int indexFile = 1;
        List<float> xTime = new List<float>();
        List<float> yPressure = new List<float>();
        xTime.Add((float)times[0][times[0].Count - 1]);
        yPressure.Add((float)PressureConverter.ConvertIntensityToPressure(
            (float)intensities[0]));
        for (int index = 1; index < times.Count; ++index)
        {
            if (rays[index].MicrophonePosition != rays[index - 1].MicrophonePosition)
            {
                WriteTimeAndPressure(xTime, yPressure, "timePressure" + indexFile.ToString() + ".txt");
                ++indexFile;
                xTime.Clear();
                yPressure.Clear();
            }
            xTime.Add((float)times[index][times[index].Count - 1]);
            yPressure.Add((float)PressureConverter.ConvertIntensityToPressure(
                (float)intensities[index]));
        }
        WriteTimeAndPressure(xTime, yPressure, "timePressure" + indexFile.ToString() + ".txt");
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
