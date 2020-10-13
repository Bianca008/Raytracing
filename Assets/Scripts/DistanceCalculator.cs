using System.Collections.Generic;
using System.Linq;

public class DistanceCalculator
{
    private Dictionary<int, List<AcousticRay>> Rays
    {
        get;
        set;
    }

    public DistanceCalculator(Dictionary<int, List<AcousticRay>> rays)
    {
        Rays = rays;
    }

    public void ComputeDistances()
    {
        for (int indexMicro = 0; indexMicro < Rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < Rays[indexMicro].Count; ++indexRay)
                ComputeDistance(indexMicro, indexRay);
    }

    private void ComputeDistance(int indexMicro, int indexRay)
    {
        if (Rays[indexMicro][indexRay].CollisionPoints.Count == 0)
        {
            Rays[indexMicro][indexRay].Distances.Add(System.Numerics.Vector3.Distance(
                Rays[indexMicro][indexRay].Source,
                Rays[indexMicro][indexRay].MicrophonePosition));
            return;
        }

        Rays[indexMicro][indexRay].Distances.Add(System.Numerics.Vector3.Distance(
            Rays[indexMicro][indexRay].Source,
            Rays[indexMicro][indexRay].CollisionPoints[0]));

        for (int indexPosition = 1; indexPosition < Rays[indexMicro][indexRay].CollisionPoints.Count; ++indexPosition)
        {
            float distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                Rays[indexMicro][indexRay].CollisionPoints[indexPosition],
                Rays[indexMicro][indexRay].CollisionPoints[indexPosition - 1]);
            Rays[indexMicro][indexRay].Distances.Add(Rays[indexMicro][indexRay].
                                                         Distances[Rays[indexMicro][indexRay].Distances.Count - 1] +
                                                         distanceBetweenTwoPoints);
        }

        Rays[indexMicro][indexRay].Distances.Add(Rays[indexMicro][indexRay].Distances[
                                                     Rays[indexMicro][indexRay].Distances.Count - 1] +
                                                     System.Numerics.Vector3.Distance(
                                                     Rays[indexMicro][indexRay].CollisionPoints[
                                                     Rays[indexMicro][indexRay].CollisionPoints.Count - 1],
                                                     Rays[indexMicro][indexRay].MicrophonePosition));
    }

}
