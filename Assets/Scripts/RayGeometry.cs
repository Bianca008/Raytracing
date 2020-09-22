using System;
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;

public class RayGeometry
{
    private readonly int numberOfRays;
    private readonly int numberOfColissions;
    private readonly int maxDistance;
    private readonly IRayCaster rayCaster;

    public List<AcousticRay> Rays
    {
        get;
        set;
    }

    public RayGeometry(Vector3 sourcePos, int nrOfRays, int nrOfColissions = 3, int maxDist = 200)
    {
        Rays = new List<AcousticRay>();
        numberOfColissions = nrOfColissions;
        numberOfRays = nrOfRays;
        maxDistance = maxDist;

        for (int index = 0; index < numberOfRays; ++index)
        {
            Rays.Add(new AcousticRay(sourcePos));
        }

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

            GenerateRay(Rays[index].Source,
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
            rayCaster.RayCast(Rays, numberOfRay);
        }
    }

    public List<AcousticRay> GetIntersectedRays(MicrophoneSphere microphone)
    {
        List<AcousticRay> newRays = new List<AcousticRay>();

        for (int indexRay = 0; indexRay < Rays.Count; ++indexRay)
            for (int indexPosition = 0; indexPosition < Rays[indexRay].ColissionPoints.Count - 1; ++indexPosition)
            {
                if (microphone.LineIntersectionWithSphere(Rays[indexRay].ColissionPoints[indexPosition],
                    Rays[indexRay].ColissionPoints[indexPosition + 1]))
                    newRays.Add(Rays[indexRay].TruncateRay(indexPosition + 1, microphone.Center));
            }

        return newRays;
    }
}
