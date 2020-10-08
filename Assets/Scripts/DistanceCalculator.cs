using System.Collections.Generic;

public class DistanceCalculator
{
    private List<AcousticRay> Rays
    {
        get;
        set;
    }

    public DistanceCalculator(List<AcousticRay> rays)
    {
        Rays = rays;
    }

    public void ComputeDistances()
    {
        for (int indexRay = 0; indexRay < Rays.Count; ++indexRay)
            ComputeDistance(indexRay);
    }

    private void ComputeDistance(int indexRay)
    {
        if (Rays[indexRay].CollisionPoints.Count == 0)
        {
            Rays[indexRay].Distances.Add(System.Numerics.Vector3.Distance(
               Rays[indexRay].Source,
               Rays[indexRay].MicrophonePosition));
            return;
        }

        Rays[indexRay].Distances.Add(System.Numerics.Vector3.Distance(
            Rays[indexRay].Source,
            Rays[indexRay].CollisionPoints[0]));

        for (int indexPosition = 1; indexPosition < Rays[indexRay].CollisionPoints.Count; ++indexPosition)
        {
            float distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                Rays[indexRay].CollisionPoints[indexPosition],
                Rays[indexRay].CollisionPoints[indexPosition - 1]);
            Rays[indexRay].Distances.Add(Rays[indexRay].Distances[Rays[indexRay].Distances.Count - 1] +
                                         distanceBetweenTwoPoints);
        }

        Rays[indexRay].Distances.Add(Rays[indexRay].Distances[Rays[indexRay].Distances.Count - 1] +
                                     System.Numerics.Vector3.Distance(
                                         Rays[indexRay].CollisionPoints[Rays[indexRay].CollisionPoints.Count - 1],
                                         Rays[indexRay].MicrophonePosition));
    }

}
