using System;
using System.Collections.Generic;
using System.Numerics;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class FileHandler
{
    public static Tuple<List<float>, List<float>> ReadFromFile(string fileName)
    {
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        using (System.IO.StreamReader file = new System.IO.StreamReader(fileName))
        {
            while (!file.EndOfStream)
            {
                string text = file.ReadLine();
                string[] bits = text.Split(' ');
                x.Add(System.Single.Parse(bits[0]));
                y.Add(System.Single.Parse(bits[1]));
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

    public static void WriteFrquencies(Echogram frequencyReponse, List<MicrophoneSphere> microphones)
    {
        for (int indexMicro = 0; indexMicro < frequencyReponse.Count; ++indexMicro)
        {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            for (int index = 0; index < frequencyReponse[microphones[indexMicro].Id].Count; ++index)
            {
                x.Add((float)frequencyReponse[microphones[indexMicro].Id][index].Magnitude);
                y.Add((float)frequencyReponse[microphones[indexMicro].Id][index].Phase);
            }
            FileHandler.WriteToFile(x, y, microphones[indexMicro].Id.ToString() + "M.txt");
        }
    }

    public static void WriteToFileTimePressure(
        Dictionary<double, Echogram> echograms,
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        List<double> frequencies)
    {
        DistanceCalculator distanceCalculator = new DistanceCalculator(rays, microphones);
        distanceCalculator.ComputeDistances();

        Dictionary<int, List<float>> times = TimeCalculator.GetTime(rays, microphones);

        for (int indexFrequencie = 0; indexFrequencie < frequencies.Count; ++indexFrequencie)
            for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
            {
                List<float> xTime = times[microphones[indexMicro].Id];
                List<float> yMagnitude = new List<float>();
                List<float> yPhase = new List<float>();
                List<Complex> microphoneIntensities = echograms[frequencies[indexFrequencie]][microphones[indexMicro].Id];
                for (int index = 0; index < microphoneIntensities.Count; ++index)
                {
                    yPhase.Add((float)(microphoneIntensities[index].Phase * 180 / Math.PI));
                    yMagnitude.Add((float)microphoneIntensities[index].Magnitude);
                }

                WriteToFile(xTime, yMagnitude, "results/timeMagnitude" +
                    (microphones[indexMicro].Id + 1).ToString() + "M" + frequencies[indexFrequencie].ToString() + "Hz.txt");

                WriteToFile(xTime, yPhase, "results/timePhase" +
                   (microphones[indexMicro].Id + 1).ToString() + "M" + frequencies[indexFrequencie].ToString() + "Hz.txt");
            }
    }
}
