using System.Collections.Generic;

public class TimeCalculator
{
    public static Dictionary<int, List<float>> GetTime(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones, 
        double airSoundSpeed = 343.21)
    {
        var rayTimes = new Dictionary<int, List<float>>();

        /*
         v = d/t
         then t = d/v
         */
        for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
        {
            rayTimes[indexMicro] = new List<float>();
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].id].Count; ++indexRay)
                rayTimes[microphones[indexMicro].id].Add(
                    (float)(rays[microphones[indexMicro].id][indexRay].GetDistance() / airSoundSpeed));
        }

        return rayTimes;
    }
}
