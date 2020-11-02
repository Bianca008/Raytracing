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
        var (magnitude, phase) = FileHandler.ReadFromFile(file);
        var freq = frequencies.Select(frequency => (float) frequency).ToList();

        chartDrawer = new ChartDrawer(menuCanvas);
        chartDrawer.DrawFrequencyChart(freq, magnitude, phase);
    }

    public void ShowFrequencyChart(
        InputField microphoneField,
        GameObject menuCanvas,
        ChartDrawer chartDrawer,
        List<double> f
    
    s,
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
