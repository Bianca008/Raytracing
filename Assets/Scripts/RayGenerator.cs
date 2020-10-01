using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay;
    public float InitialPower = 1;
    public int NumberOfRays = 100000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    public Material LineMaterial;
    public GameObject ChartArea;

    private readonly int maxDistance = 200;
    private int numberOfColissions = 9;
    private List<AcousticRay> rays;
    private LineRenderer[] lines;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;
    private RaysDrawer intersectedRayDrawer;
    private MicrophoneSphere microphone;
    private ChartDrawer chartDrawer;

    private void Start()
    {
        CreateRays();
        CreateMicrophone();
        CreateIntersectedRaysWithMicrophone();
        DrawMicrophone();

        ComputeIntensities();
        DrawChartTimePressure();
    }

    private void Update()
    {
        if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1)
            intersectedRayDrawer.Draw(NumberOfRay - 1);
        else
        {
            Debug.Log("The number of ray does not exist...");
        }

        if (Input.GetKey("i"))
            chartDrawer.Enable = false;
        if (Input.GetKey("o"))
            chartDrawer.Enable = true;
    }

    private void CreateRays()
    {
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            NumberOfRays,
            numberOfColissions,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
    }

    private List<AcousticRay> RemoveDuplicates(List<AcousticRay> rays)
    {
        int indexRay = 0;
        while (indexRay < rays.Count - 1)
        {
            if (rays[indexRay].ColissionPoints.Count == rays[indexRay + 1].ColissionPoints.Count)
            {
                int size = rays[indexRay].ColissionPoints.Count;
                double epsilon = 1e-3;
                int indexPointCompared = 0;
                bool okToDelete = true;
                while (indexPointCompared < size)
                {
                    if (System.Numerics.Vector3.Distance
                        (rays[indexRay].ColissionPoints[indexPointCompared],
                        rays[indexRay + 1].ColissionPoints[indexPointCompared]) > epsilon)
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

        IntersectedRays = rays.Count;

        return rays;
    }

    private void CreateIntersectedRaysWithMicrophone()
    {
        List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone);

        newRays.Sort(delegate (AcousticRay first, AcousticRay second)
        {
            return first.Distance.CompareTo(second.Distance);

        });
        IntersectedRaysWithDuplicate = newRays.Count;
        rays = RemoveDuplicates(newRays);
        intersectedLines = LinesCreator.GenerateLines(rays.Count, transform, LineMaterial);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, rays);
    }

    private void CreateMicrophone()
    {
        microphone = new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f);
    }

    private void DrawMicrophone()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = VectorConverter.Convert(microphone.Center);
        sphere.transform.localScale = new UnityEngine.Vector3(microphone.Radius, microphone.Radius, microphone.Radius);
    }

    private void ComputeIntensities()
    {
        IntensityCalculator intensityCalculator = new IntensityCalculator(rays, InitialPower);
        intensityCalculator.ComputePower();
    }

    private void DrawChartTimePressure()
    {
        DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        distanceCalculator.ComputeDistances();

        List<List<double>> times = TimeCalculator.GetTime(rays);
        List<float> xTime = new List<float>();
        for (int index = 0; index < times.Count; ++index)
            xTime.Add((float)times[index][times[index].Count - 1]);

        List<float> yPressure = new List<float>();
        for (int index = 0; index < rays.Count; ++index)
            yPressure.Add((float)PressureConverter.ConvertIntensityToPressure(
                (float)rays[index].Intensities[rays[index].Intensities.Count - 1]));

        WriteTimeAndPressure(xTime, yPressure);

        chartDrawer = new ChartDrawer(ChartArea);
        chartDrawer.Draw(xTime, yPressure);
    }

    private void WriteTimeAndPressure(List<float> time, List<float> pressure)
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("timePressure.txt", true))
        {
            for (int index = 0; index < time.Count; ++index)
                file.WriteLine(time[index] + " " + pressure[index]);
        }
    }

}
