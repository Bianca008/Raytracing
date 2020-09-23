﻿using System.Collections.Generic;
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

    public Material lineMaterial;
    //Material

    private void Start()
    {
        lines = LinesCreator.GenerateLines(numberOfRays, transform, lineMaterial);
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            numberOfRays,
            numberOfColissions,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
        //rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.Rays);
        energyCalculator = new EnergyCalculator(numberOfRays, StartEnergy);
        microphone = new MicrophoneSphere(new System.Numerics.Vector3(0, 0.5f, 1.75f), 0.05f);
        List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone);

        newRays.Sort(delegate (AcousticRay first, AcousticRay second)
        {
            return first.Distance.CompareTo(second.Distance);

        });

        intersectedLines = LinesCreator.GenerateLines(newRays.Count, transform, lineMaterial);
        intersectedRayDrawer = new RaysDrawer(intersectedLines, newRays);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = VectorConverter.Convert(microphone.Center);
        sphere.transform.localScale = new UnityEngine.Vector3(microphone.Radius, microphone.Radius, microphone.Radius);
        //rayDrawer.Draw();
        //intersectedRayDrawer.Draw();
        energyCalculator.CalculateEnergy(rayGeometryGenerator.Rays);
    }

    private void Update()
    {
        intersectedRayDrawer.Draw(numberOfRay);
    }
}
