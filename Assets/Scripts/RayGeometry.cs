using System;
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;

public class RayGeometry
{
    private readonly int m_numberOfRays;
    private readonly int m_numberOfCollisions;
    private readonly int m_maxDistance;
    private readonly IRayCaster m_rayCaster;

    public List<AcousticRay> rays { get; set; } = new List<AcousticRay>();

    public RayGeometry(Vector3 sourcePos,
        List<MicrophoneSphere> microphones,
        int nrOfRays,
        int nrOfCollisions = 3,
        int maxDist = 200)
    {
        m_numberOfCollisions = nrOfCollisions;
        m_numberOfRays = nrOfRays;
        m_maxDistance = maxDist;

        for (int indexRay = 0; indexRay < m_numberOfRays; ++indexRay)
            for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
            {
                rays.Add(new AcousticRay(sourcePos, microphones[indexMicro].center));
            }

        m_rayCaster = new IRayCaster(m_maxDistance);
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
        int indexRay = 0;

        for (int index = 0; index < m_numberOfRays; ++index)
        {
            var theta = Math.Asin(-1.0 + 2.0 * index / (m_numberOfRays + 1.0));
            var phi = goldenAngle * index;

            GenerateRay(rays[index].source,
               GetCartesianCoordinates(1, theta, phi),
               indexRay);
            ++indexRay;
        }
    }

    private void GenerateRay(Vector3 position, Vector3 direction, int numberOfRay)
    {
        m_rayCaster.totalDistance = 0;
        m_rayCaster.numberOfReflections = 0;
        m_rayCaster.position = position;
        m_rayCaster.direction = direction;

        while (m_rayCaster.totalDistance <= m_maxDistance &&
            m_rayCaster.numberOfReflections <= m_numberOfCollisions)
        {
            m_rayCaster.RayCast(rays, numberOfRay);
        }
    }

    public List<AcousticRay> GetIntersectedRays(MicrophoneSphere microphone)
    {
        var newRays = new List<AcousticRay>();

        foreach (var ray in rays)
        {
            if (ray.collisionPoints.Count > 0 &&
                microphone.LineIntersectionWithSphere(ray.source,
                    ray.collisionPoints[0]))
            {
                newRays.Add(ray.TruncateRay(0, microphone.center));
            }
            else
            {
                for (int indexPosition = 0; indexPosition < ray.collisionPoints.Count - 1; ++indexPosition)
                {
                    if (microphone.LineIntersectionWithSphere(ray.collisionPoints[indexPosition],
                        ray.collisionPoints[indexPosition + 1]))
                        newRays.Add(ray.TruncateRay(indexPosition + 1, microphone.center));
                }
            }
        }

        return newRays;
    }
}
