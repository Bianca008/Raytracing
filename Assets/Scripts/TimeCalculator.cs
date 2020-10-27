using System.Collections.Generic;

public class TimeCalculator
{
    public static Dictionary<int, List<float>> GetTime(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones, 
        double airSoundSpeed = 343.21)
    {
        Dictionary<int, List<float>> rayTimes = new Dictionary<int, List<float>>();

        /*
         v = d/t
         then t = d/v
         */
        for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
        {
            rayTimes[indexMicro] = new List<float>();
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].Id].Count; ++indexRay)
                rayTimes[microphones[indexMicro].Id].Add(
                    (float)(rays[microphones[indexMicro].Id][indexRay].Distance / airSoundSpeed));
        }

        return rayTimes;
    }
}
