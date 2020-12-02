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
            rayTimes[microphones[indexMicro].id] = new List<float>();
            for (int indexRay = 0; indexRay < rays[microphones[indexMicro].id].Count; ++indexRay)
                rayTimes[microphones[indexMicro].id].Add(
                    (float)(rays[microphones[indexMicro].id][indexRay].GetDistance() / airSoundSpeed));
        }

        return rayTimes;
    }

    public static List<float> GetTimeForMicrophone(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        int idMicrophone,
        double airSoundSpeed = 343.21
        )
    {
        var time = GetTime(rays, microphones, airSoundSpeed);

        return time[idMicrophone];
    }
}
