using System.Collections.Generic;

public class TimeCalculator
{
    public static Dictionary<int, List<float>> GetTime(Dictionary<int, List<AcousticRay>> rays,
        double airSoundSpeed = 343.21)
    {
        Dictionary<int, List<float>> rayTimes = new Dictionary<int, List<float>>();

        /*
         v = d/t
         then t = d/v
         */
        for (int indexMicro = 0; indexMicro < rays.Count; ++indexMicro)
        {
            List<float> times = new List<float>();
            rayTimes[indexMicro] = new List<float>();
            for (int indexRay = 0; indexRay < rays[indexMicro].Count; ++indexRay)
            {
                for (int indexPosition = 0; indexPosition < rays[indexMicro][indexRay].Distances.Count; ++indexPosition)
                    times.Add((float)(rays[indexMicro][indexRay].Distances[indexPosition] / airSoundSpeed));
                rayTimes[indexMicro].Add((float)(rays[indexMicro][indexRay].Distance / airSoundSpeed));
            }

        }

        return rayTimes;
    }
}
