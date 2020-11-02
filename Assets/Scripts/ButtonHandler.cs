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
        var file = indexMicrophone.ToString() + "M.txt";
        var magnitudeAndPhase = FileHandler.ReadFromFile(file);
        var freq = new List<float>();

        foreach (var frequency in frequencies)
            freq.Add((float)frequency);

        chartDrawer = new ChartDrawer(menuCanvas);
        chartDrawer.DrawFrequencyChart(freq, magnitudeAndPhase.Item1, magnitudeAndPhase.Item2);
    }

    public void ShowFrequencyChart(
        InputField microphoneField,
        GameObject menuCanvas,
        ChartDrawer chartDrawer,
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophoneStr = microphoneField.text;
        var numberOfMicrophone = 0;

        if (numberOfMicrophoneStr.All(char.IsDigit) == true)
            numberOfMicrophone = Int32.Parse(numberOfMicrophoneStr) - 1;

        var okToDraw = false;
        foreach (var micro in microphones)
            if (micro.id == numberOfMicrophone)
                okToDraw = true;

        if (okToDraw == true)
            DrawChartFrequency(menuCanvas, chartDrawer, frequencies, numberOfMicrophone);
        else
            Debug.Log("The microphone for which you want to see the result does not exist.");
    }

}
