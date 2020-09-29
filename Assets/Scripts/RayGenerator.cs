using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public int numberOfRay;
    public double InitialPower = 25;
    private int numberOfColissions = 9;
    private readonly int maxDistance = 200;
    public int numberOfRays = 10000;
    public int IntersectedRays;
    public int IntersectedRaysWithDuplicate;
    private LineRenderer[] lines;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;
    private RaysDrawer intersectedRayDrawer;
    private EnergyCalculator energyCalculator;
    private MicrophoneSphere microphone;
    public Material lineMaterial;

    private void Start()
    {
        CreateRays();
        CreateMicrophone();
        CreateIntersectedRaysWithMicrophone();
        DrawMicrophone();
    }

    private void Update()
    {
        if (numberOfRay <= IntersectedRays && numberOfRay >= 1)
            intersectedRayDrawer.Draw(numberOfRay - 1);
        else
        {
            Debug.Log("The number of ray does not exist...");
        }
    }

    private void CreateRays()
    {
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            numberOfRays,
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
        List<AcousticRay> raysWithoutDuplicates = RemoveDuplicates(newRays);
        intersectedLines = LinesCreator.GenerateLines(raysWithoutDuplicates.Count, transform, lineMaterial);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, raysWithoutDuplicates);
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

    private void CalculateEnergy()
    {
        energyCalculator = new EnergyCalculator(numberOfRays, InitialPower);
        energyCalculator.CalculateEnergy(rayGeometryGenerator.Rays);
    }

    private void CreateAndDrawAllRays()
    {
        lines = LinesCreator.GenerateLines(numberOfRays, transform, lineMaterial);
        rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.Rays);
        rayDrawer.Draw();
    }
}
