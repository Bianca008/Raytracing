using System.Collections.Generic;
using System.Linq;

public class DistanceCalculator
{
    private Dictionary<int, List<AcousticRay>> rays
    {
        get;
        set;
    }

    private List<MicrophoneSphere> microphones
    {
        get;
        set;
    }

    public DistanceCalculator(Dictionary<int, List<AcousticRay>> rays, List<MicrophoneSphere> microphones)
    {
        this.microphones = microphones;
        this.rays = rays;
    }

    public void ComputeDistances()
    {
        for (var indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
            for (var indexRay = 0; indexRay < rays[microphones[indexMicro].Id].Count; ++indexRay)
                ComputeDistance(microphones[indexMicro].Id, indexRay);
    }

    private void ComputeDistance(int indexMicro, int indexRay)
    {
        if (rays[indexMicro][indexRay].CollisionPoints.Count == 0)
        {
            rays[indexMicro][indexRay].Distances.Add(System.Numerics.Vector3.Distance(
                rays[indexMicro][indexRay].Source,
                rays[indexMicro][indexRay].MicrophonePosition));
            return;
        }

        rays[indexMicro][indexRay].Distances.Add(System.Numerics.Vector3.Distance(
            rays[indexMicro][indexRay].Source,
            rays[indexMicro][indexRay].CollisionPoints[0]));

        for (var indexPosition = 1; indexPosition < rays[indexMicro][indexRay].CollisionPoints.Count; ++indexPosition)
        {
            var distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                rays[indexMicro][indexRay].CollisionPoints[indexPosition],
                rays[indexMicro][indexRay].CollisionPoints[indexPosition - 1]);
            rays[indexMicro][indexRay].Distances.Add(rays[indexMicro][indexRay].
                                                         Distances[rays[indexMicro][indexRay].Distances.Count - 1] +
                                                         distanceBetweenTwoPoints);
        }

        rays[indexMicro][indexRay].Distances.Add(rays[indexMicro][indexRay].Distances[
                                                     rays[indexMicro][indexRay].Distances.Count - 1] +
                                                     System.Numerics.Vector3.Distance(
                                                     rays[indexMicro][indexRay].CollisionPoints[
                                                     rays[indexMicro][indexRay].CollisionPoints.Count - 1],
                                                     rays[indexMicro][indexRay].MicrophonePosition));
    }

}
