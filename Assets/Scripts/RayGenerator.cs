using System;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public double startEnergy = 25;
    public double absorptionCoefficient = 0.2;
    public double minimumEnergy = 0;

    private int numberOfColissions = 3;
    private int maxDistance = 200;
    private readonly int numberOfRays = 3;

    private LineRenderer[] lines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;

    private void Start()
    {
        lines = new LineRenderer[numberOfRays];

        for (int index = 0; index < numberOfRays; ++index)
            lines[index] = SetLineProperties();

        rayGeometryGenerator = new RayGeometry(transform.position, numberOfRays, numberOfColissions, maxDistance);

        rayGeometryGenerator.GenerateRays();

        rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.LinePosistions);
    }

    private double CurrentEnergy
    {
        get;
        set;
    }

    private double CalculateEnergyInPoint(Vector3 position, double distance)
    {
        /*
         E = Ef/ Nr D_theta_phi e^-gama_d prod(1-alfa_i)

        Ef - total energy emitted by the sound
        Nr - number of rays
        D_theta_phi - source directively; used D_theta_phi = 1
        gama -air absorbtion
        d - distance path traveld by the ray
        alfa_i - absorbtion coefficient at surface i
         */

        double airAbsorption = 0.0013;
        double energy = startEnergy / numberOfRays * Math.Exp(-airAbsorption * distance) * (1 - absorptionCoefficient);

        return energy;
    }

    public Tuple<double, double, double> GetPolarCoordinates(Vector3 position)
    {
        double distanceFromOrigin = Math.Sqrt(Math.Pow(position.x, 2) +
                                    Math.Pow(position.y, 2) +
                                    Math.Pow(position.z, 2));
        double theta = Math.Atan(position.y / position.x);
        double phi = Math.Acos(position.z / distanceFromOrigin);

        /*This transformation is in radians.*/
        return Tuple.Create(distanceFromOrigin, theta, phi);
    }

    private void Update()
    {
        //GenerateRays();
        rayGeometryGenerator.GenerateRays();
        rayDrawer.Draw();
    }

    private LineRenderer SetLineProperties()
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.positionCount = 1;
        line.transform.SetParent(transform);

        return line;
    }
}
