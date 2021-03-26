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
        foreach (var microphone in microphones)
        {
            rayTimes[microphone.Id] = new List<float>();
            for (var indexRay = 0; indexRay < rays[microphone.Id].Count; ++indexRay)
                rayTimes[microphone.Id].Add(
                    (float)(rays[microphone.Id][indexRay].GetDistance() / airSoundSpeed));
        }

        return rayTimes;
    }

    public static List<float> GetTimeForMicrophone(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        int idMicrophone,
        double airSoundSpeed = 343.21)
    {
        var time = GetTime(rays, microphones, airSoundSpeed);

        return time[idMicrophone];
    }
}
