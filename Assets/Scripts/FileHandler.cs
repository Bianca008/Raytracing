using System;
using System.Collections.Generic;

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
}
