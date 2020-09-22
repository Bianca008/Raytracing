using System.Collections.Generic;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public int numberOfRay;
    public double StartEnergy = 25;
    private int numberOfColissions = 3;
    private readonly int maxDistance = 200;
    private readonly int numberOfRays = 2000;

    private LineRenderer[] lines;
    private LineRenderer[] intersectedLines;
    private RayGeometry rayGeometryGenerator;
    //private RaysDrawer rayDrawer;
    private RaysDrawer intersectedRayDrawer;
    private EnergyCalculator energyCalculator;
    private MicrophoneSphere microphone;

    private void Start()
    {
        lines = LinesCreator.GenerateLines(numberOfRays, transform);
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            numberOfRays,
            numberOfColissions,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
        //rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.Rays);
        energyCalculator = new EnergyCalculator(numberOfRays, StartEnergy);
        microphone = new MicrophoneSphere(new System.Numerics.Vector3(0, 0.5f, 1.75f), 0.05f);
        List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone);
        intersectedLines = LinesCreator.GenerateLines(newRays.Count, transform);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, newRays);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = VectorConverter.Convert(microphone.Center);
        sphere.transform.localScale = new UnityEngine.Vector3(microphone.Radius*5, microphone.Radius*5, microphone.Radius*5);
        //rayDrawer.Draw();
        //intersectedRayDrawer.Draw();
        energyCalculator.CalculateEnergy(rayGeometryGenerator.Rays);
    }

    private void Update()
    {
        intersectedRayDrawer.Draw(numberOfRay);
    }
}
