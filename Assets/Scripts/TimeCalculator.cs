using System.Collections.Generic;

public class TimeCalculator
{
    public static List<List<double>> GetTime(List<AcousticRay> rays, double airSoundSpeed = 343.21)
    {
        List<List<double>> rayTimes = new List<List<double>>();

        /*
         v = d/t
         then t = d/v
         */

        for (int indexRay = 0; indexRay < rays.Count; ++indexRay)
        {
            List<double> times = new List<double>();

            for (int indexPosition = 0; indexPosition < rays[indexRay].Distances.Count; ++indexPosition)
                times.Add(rays[indexRay].Distances[indexPosition]/ airSoundSpeed);

            rayTimes.Add(times);
        }

        return rayTimes;
    }
}
