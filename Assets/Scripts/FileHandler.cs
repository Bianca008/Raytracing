using System;
using System.Collections.Generic;
using System.Numerics;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class FileHandler
{
    public static Tuple<List<float>, List<float>> ReadFromFile(string fileName)
    {
        var x = new List<float>();
        var y = new List<float>();
        using (var file = new System.IO.StreamReader(fileName))
        {
            while (!file.EndOfStream)
            {
                var text = file.ReadLine();
                var bits = text.Split(' ');
                x.Add(Single.Parse(bits[0]));
                y.Add(Single.Parse(bits[1]));
            }
        }

        return new Tuple<List<float>, List<float>>(x, y);
    }

    public static void WriteToFile(List<float> x, List<float> y, string fileName)
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName, false))
        {
            for (int index = 0; index < x.Count; ++index)
                file.WriteLine(x[index] + " " + y[index]);
        }
    }

    public static void WriteFrquencies(Echogram frequencyResponse, List<MicrophoneSphere> microphones)
    {
        for (int indexMicro = 0; indexMicro < frequencyResponse.Count; ++indexMicro)
        {
            var x = new List<float>();
            var y = new List<float>();
            for (int index = 0; index < frequencyResponse[microphones[indexMicro].id].Count; ++index)
            {
                x.Add((float)frequencyResponse[microphones[indexMicro].id][index].Magnitude);
                y.Add((float)frequencyResponse[microphones[indexMicro].id][index].Phase);
            }
            FileHandler.WriteToFile(x, y, microphones[indexMicro].id.ToString() + "M.txt");
        }
    }

    public static void WriteToFileTimePressure(
        Dictionary<double, Echogram> echograms,
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        List<double> frequencies)
    {
        var distanceCalculator = new DistanceCalculator(rays, microphones);
        distanceCalculator.ComputeDistances();

        var times = TimeCalculator.GetTime(rays, microphones);

        for (int indexFrequency = 0; indexFrequency < frequencies.Count; ++indexFrequency)
            for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
            {
                var xTime = times[microphones[indexMicro].id];
                var yMagnitude = new List<float>();
                var yPhase = new List<float>();
                var microphoneIntensities = echograms[frequencies[indexFrequency]][microphones[indexMicro].id];
                for (int index = 0; index < microphoneIntensities.Count; ++index)
                {
                    yPhase.Add((float)(microphoneIntensities[index].Phase * 180 / Math.PI));
                    yMagnitude.Add((float)microphoneIntensities[index].Magnitude);
                }

                WriteToFile(xTime, yMagnitude, "results/timeMagnitude" +
                    (microphones[indexMicro].id + 1).ToString() + "M" + frequencies[indexFrequency].ToString() + "Hz.txt");

                WriteToFile(xTime, yPhase, "results/timePhase" +
                   (microphones[indexMicro].id + 1).ToString() + "M" + frequencies[indexFrequency].ToString() + "Hz.txt");
            }
    }
}
