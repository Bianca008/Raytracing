using System;
using System.Collections.Generic;
using System.Numerics;

public class RayGeometry
{
    private int numberOfRays;
    private int numberOfColissions;
    private int maxDistance;
    private Vector3 sourcePosition;
    private IRayCaster rayCaster;

    public List<List<Vector3>> LinePosistions
    {
        get;
        set;
    }

    public RayGeometry(Vector3 sourcePos, int nrOfRays, int nrOfColissions = 3, int maxDist = 200)
    {
        sourcePosition = sourcePos;
        numberOfColissions = nrOfColissions;
        numberOfRays = nrOfRays;
        maxDistance = maxDist;
        LinePosistions = new List<List<Vector3>>();

        for (int index = 0; index < numberOfRays; ++index)
            LinePosistions.Add(new List<Vector3>() { sourcePosition });

        rayCaster = new IRayCaster(maxDistance);
    }

    public Vector3 GetCartesianCoordinates(double distance, double theta, double phi)
    {
        /*Sin and Cos are using radians.*/
        double x = distance * Math.Sin(phi) * Math.Cos(theta);
        double y = distance * Math.Sin(phi) * Math.Sin(theta);
        double z = distance * Math.Cos(phi);

        return new Vector3((float)x, (float)y, (float)z);
    }

    public void GenerateRays()
    {
        //!!!Var3 - Fibo Sphere!!!
        double goldenRatio = (1 + Math.Sqrt(5)) / 2;
        double goldenAngle = (2 - goldenRatio) * (2 * Math.PI);
        int indexRay = 0;

        for (int index = 0; index < numberOfRays; ++index)
        {
            double theta = Math.Asin(-1 + 2 * index / (numberOfRays + 1));
            double phi = goldenAngle * index;

            GenerateRay(sourcePosition,
               GetCartesianCoordinates(1, theta, phi),
               indexRay);
            ++indexRay;
        }
    }

    private void GenerateRay(Vector3 position, Vector3 direction, int numberOfRay)
    {
        rayCaster.TotalDistance = 0;
        rayCaster.NumberOfReflections = 0;
        rayCaster.Position = position;
        rayCaster.Direction = direction;

        while (rayCaster.TotalDistance <= maxDistance &&
            rayCaster.NumberOfReflections < numberOfColissions)
        {
            rayCaster.RayCast(LinePosistions, numberOfRay);
        }
    }
}
