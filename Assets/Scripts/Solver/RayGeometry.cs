using System;
using System.Collections.Generic;
using System.Linq;
using Vector3 = System.Numerics.Vector3;

public class RayGeometry
{
    private readonly int numberOfRays;
    private readonly int numberOfCollisions;
    private readonly int maxDistance;
    private readonly RayCaster rayCaster;

    public List<AcousticRay> Rays { get; set; } = new List<AcousticRay>();

    public RayGeometry(Vector3 sourcePos,
        List<MicrophoneSphere> microphones,
        int nrOfRays,
        int nrOfCollisions = 3,
        int maxDist = 200)
    {
        numberOfCollisions = nrOfCollisions;
        numberOfRays = nrOfRays;
        maxDistance = maxDist;

        for (var indexRay = 0; indexRay < numberOfRays; ++indexRay)
            foreach (var microphone in microphones)
            {
                Rays.Add(new AcousticRay(sourcePos, microphone.Center));
            }

        rayCaster = new RayCaster(maxDistance);
    }

    public Vector3 GetCartesianCoordinates(double distance, double theta, double phi)
    {
        /*Sin and Cos are using radians.*/
        var x = distance * Math.Sin(phi) * Math.Cos(theta);
        var y = distance * Math.Sin(phi) * Math.Sin(theta);
        var z = distance * Math.Cos(phi);

        return new Vector3((float)x, (float)y, (float)z);
    }

    public void GenerateRays()
    {
        //!!!Var3 - Fibo Sphere!!!
        var goldenRatio = (1 + Math.Sqrt(5)) / 2;
        var goldenAngle = (2 - goldenRatio) * (2 * Math.PI);
        var indexRay = 0;

        for (var index = 0; index < numberOfRays; ++index)
        {
            var theta = Math.Asin(-1.0 + 2.0 * index / (numberOfRays + 1.0));
            var phi = goldenAngle * index;

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
            rayCaster.NumberOfReflections <= numberOfCollisions)
        {
            rayCaster.RayCast(Rays, numberOfRay);
        }
    }

    public List<AcousticRay> GetIntersectedRays(MicrophoneSphere microphone)
    {
        var newRays = new List<AcousticRay>();

        foreach (var ray in Rays)
        {
            if (ray.CollisionPoints.Count == 0)
            {
                var ok = false;
                foreach (var newRay in newRays.Where(newRay => newRay.CollisionPoints.Count == 0))
                    ok = true;
                if (ok == false)
                    newRays.Add(ray);
            }
            else
            if (ray.CollisionPoints.Count > 0 &&
                microphone.LineIntersectionWithSphere(ray.Source,
                    ray.CollisionPoints[0]))
            {
                newRays.Add(ray.TruncateRay(0, microphone.Center));
            }
            else
            {
                for (var indexPosition = 0; indexPosition < ray.CollisionPoints.Count - 1; ++indexPosition)
                {
                    if (microphone.LineIntersectionWithSphere(ray.CollisionPoints[indexPosition],
                        ray.CollisionPoints[indexPosition + 1]))
                        newRays.Add(ray.TruncateRay(indexPosition + 1, microphone.Center));
                }
            }
        }

        return newRays;
    }
}
