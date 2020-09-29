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
        Rays[indexRay].Distances.Add(0);
        for (int indexPosition = 1; indexPosition < Rays[indexRay].ColissionPoints.Count; ++indexPosition)
        {
            float distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                Rays[indexRay].ColissionPoints[indexPosition],
                Rays[indexRay].ColissionPoints[indexPosition - 1]);
            Rays[indexRay].Distances.Add(Rays[indexRay].Distances[Rays[indexRay].Distances.Count - 1] +
                                         distanceBetweenTwoPoints);
        }
    }

}
