using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler
{
    private void DrawChartFrequency(
        GameObject menuCanvas,
        ChartDrawer chartDrawer,
        List<double> frequencies,
        int indexMicrophone)
    {
        string file = indexMicrophone.ToString() + "M.txt";
        Tuple<List<float>, List<float>> magnitudeAndPhse = FileHandler.ReadFromFile(file);
        List<float> freq = new List<float>();

        foreach (double frequency in frequencies)
            freq.Add((float)frequency);

        chartDrawer = new ChartDrawer(menuCanvas);
        chartDrawer.DrawFrequencieChart(freq, magnitudeAndPhse.Item1, magnitudeAndPhse.Item2);
    }

    public void ShowFrequencyChart(
        InputField microphoneField,
        GameObject menuCanvas,
        ChartDrawer chartDrawer,
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        string numberOfMicrophoneStr = microphoneField.text;
        int numberOfMicrophone = 0;

        if (numberOfMicrophoneStr.All(char.IsDigit) == true)
            numberOfMicrophone = Int32.Parse(numberOfMicrophoneStr) - 1;

        bool okToDraw = false;
        foreach (MicrophoneSphere micro in microphones)
            if (micro.Id == numberOfMicrophone)
                okToDraw = true;

        if (okToDraw == true)
            DrawChartFrequency(menuCanvas, chartDrawer, frequencies, numberOfMicrophone);
        else
            Debug.Log("The microphone for which you want to see the result does not exist.");
    }

}
