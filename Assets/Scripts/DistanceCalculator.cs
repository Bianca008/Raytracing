using System.Collections.Generic;
using System.Linq;

public class DistanceCalculator
{
    private Dictionary<int, List<AcousticRay>> m_rays
    {
        get;
        set;
    }

    private List<MicrophoneSphere> m_microphones
    {
        get;
        set;
    }

    public DistanceCalculator(Dictionary<int, List<AcousticRay>> rays, List<MicrophoneSphere> microphones)
    {
        this.m_microphones = microphones;
        this.m_rays = rays;
    }

    public void ComputeDistances()
    {
        for (int indexMicro = 0; indexMicro < m_rays.Count; ++indexMicro)
            for (int indexRay = 0; indexRay < m_rays[m_microphones[indexMicro].id].Count; ++indexRay)
                ComputeDistance(m_microphones[indexMicro].id, indexRay);
    }

    private void ComputeDistance(int indexMicro, int indexRay)
    {
        if (m_rays[indexMicro][indexRay].collisionPoints.Count == 0)
        {
            m_rays[indexMicro][indexRay].distances.Add(System.Numerics.Vector3.Distance(
                m_rays[indexMicro][indexRay].source,
                m_rays[indexMicro][indexRay].microphonePosition));
            return;
        }

        m_rays[indexMicro][indexRay].distances.Add(System.Numerics.Vector3.Distance(
            m_rays[indexMicro][indexRay].source,
            m_rays[indexMicro][indexRay].collisionPoints[0]));

        for (int indexPosition = 1; indexPosition < m_rays[indexMicro][indexRay].collisionPoints.Count; ++indexPosition)
        {
            var distanceBetweenTwoPoints = System.Numerics.Vector3.Distance(
                m_rays[indexMicro][indexRay].collisionPoints[indexPosition],
                m_rays[indexMicro][indexRay].collisionPoints[indexPosition - 1]);
            m_rays[indexMicro][indexRay].distances.Add(m_rays[indexMicro][indexRay].
                                                         distances[m_rays[indexMicro][indexRay].distances.Count - 1] +
                                                         distanceBetweenTwoPoints);
        }

        m_rays[indexMicro][indexRay].distances.Add(m_rays[indexMicro][indexRay].distances[
                                                     m_rays[indexMicro][indexRay].distances.Count - 1] +
                                                     System.Numerics.Vector3.Distance(
                                                     m_rays[indexMicro][indexRay].collisionPoints[
                                                     m_rays[indexMicro][indexRay].collisionPoints.Count - 1],
                                                     m_rays[indexMicro][indexRay].microphonePosition));
    }

}
