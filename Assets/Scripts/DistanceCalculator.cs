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
        for (int indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].id].Count; ++indexRay)
                ComputeDistance(microphones[indexMicro].id, indexRay);
    }

    private void ComputeDistance(int indexMicro, int indexRay)
    {
        if (rays[indexMicro][indexRay].collisionPoints.Count == 0)
        {
            rays[indexMicro][indexRay].distances.Add(System.Numerics.Vector3.Distance(
                rays[indexMicro][indexRay].source,
                rays[indexMicro][indexRay].microphonePosition));
            return;
        }

        rays[indexMicro][indexRay].distances.Add(System.Numerics.Vector3.Distance(
            rays[indexMicro][indexRay].source,
            rays[indexMicro][indexRay].collisionPoints[0]));

        for (int indexPosition = 1; indexPosition < rays[indexMicro][indexRay].collisionPoints.Count; ++indexPosition)
        {
            var distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                rays[indexMicro][indexRay].collisionPoints[indexPosition],
                rays[indexMicro][indexRay].collisionPoints[indexPosition - 1]);
            rays[indexMicro][indexRay].distances.Add(rays[indexMicro][indexRay].
                                                         distances[rays[indexMicro][indexRay].distances.Count - 1] +
                                                         distanceBetweenTwoPoints);
        }

        rays[indexMicro][indexRay].distances.Add(rays[indexMicro][indexRay].distances[
                                                     rays[indexMicro][indexRay].distances.Count - 1] +
                                                     System.Numerics.Vector3.Distance(
                                                     rays[indexMicro][indexRay].collisionPoints[
                                                     rays[indexMicro][indexRay].collisionPoints.Count - 1],
                                                     rays[indexMicro][indexRay].microphonePosition));
    }

}
