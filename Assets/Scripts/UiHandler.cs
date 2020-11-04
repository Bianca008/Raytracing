using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler
{
    private ChartDrawer chartDrawer
    {
        get;
        set;
    }

    GameObject menuCanvas
    {
        get;
        set;
    }

    public UiHandler(GameObject menuCanvas)
    {
        this.menuCanvas = menuCanvas;
        chartDrawer = new ChartDrawer(this.menuCanvas);
    }

    private void DrawChartFrequency(
        List<double> frequencies,
        int indexMicrophone)
    {
        var file = indexMicrophone.ToString() + "M.txt";
        var (magnitude, phase) = FileHandler.ReadFromFile(file);
        var freq = frequencies.Select(frequency => (float)frequency).ToList();

        chartDrawer.DrawFrequencyChart(freq, magnitude, phase);
    }

    private void ShowFrequencyChart(
        InputField microphoneField,
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
            DrawChartFrequency(frequencies, numberOfMicrophone);
        else
            Debug.Log("The microphone for which you want to see the result does not exist.");
    }

    public void InitializeUi(
        Button showFrequencyEchogramButton,
        Button showButton,
        Button showTimeEchogramButton,
        InputField numberOfMicrophoneInputField,
        List<MicrophoneSphere> microphones,
        List<double> frequencies)
    {
        var inputPanel = GameObject.Find("InputPanel");
        inputPanel.SetActive(false);
        var frequencyPanel = GameObject.Find("FrequencyPanel");
        frequencyPanel.SetActive(false);
        var timePanel = GameObject.Find("TimePanel");
        timePanel.SetActive(false);

        AddListenerForShowFrequencyEchogram(showFrequencyEchogramButton);
        AddListenerForShowButton(showButton, numberOfMicrophoneInputField, frequencies, microphones);
        AddListenerForShowTimeEchogram(showTimeEchogramButton);
    }

    private void AddListenerForShowButton(
        Button showButton,
        InputField numberOfMicrophoneInputField,
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        showButton.onClick.AddListener(() =>
        {
            ShowFrequencyChart(numberOfMicrophoneInputField,
                frequencies,
                microphones);
        });
    }

    private void AddListenerForShowFrequencyEchogram(Button showFrequencyEchogramButton)
    {
        showFrequencyEchogramButton.onClick.AddListener(SetActiveFrequencyEchogramUi);
    }

    private void AddListenerForShowTimeEchogram(Button showTimeEchogramButton)
    {
        showTimeEchogramButton.onClick.AddListener(SetActiveTimeEchogramUi);
    }

    private void SetActiveTimeEchogramUi()
    {
        var inputPanel = menuCanvas.transform.Find("InputPanel").gameObject;
        inputPanel.SetActive(true);
        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var timePanel = buttonsAndPlotPanel.transform.Find("TimePanel").gameObject;
        timePanel.SetActive(true);
        var frequencyPanel = buttonsAndPlotPanel.transform.Find("FrequencyPanel").gameObject;
        frequencyPanel.SetActive(false);
        //DrawChart(1, 2.691650390625);
    }

    private void SetActiveFrequencyEchogramUi()
    {
        var inputPanel = menuCanvas.transform.Find("InputPanel").gameObject;
        inputPanel.SetActive(true);
        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var frequencyPanel = buttonsAndPlotPanel.transform.Find("FrequencyPanel").gameObject;
        frequencyPanel.SetActive(true);
        var timePanel = buttonsAndPlotPanel.transform.Find("TimePanel").gameObject;
        timePanel.SetActive(false);
    }

    private void DrawChart(int indexMicrophone, double indexFrequency)
    {
        var timeMagnitudeFile = "results/timeMagnitude" +
                                (indexMicrophone).ToString() + "M" +
                                indexFrequency.ToString() + "Hz.txt";
        var timePhaseFile = "results/timePhase" +
                            (indexMicrophone).ToString() + "M" +
                            indexFrequency.ToString() + "Hz.txt";

        var (tm, phase) = FileHandler.ReadFromFile(timePhaseFile);
        var (time, magnitude) = FileHandler.ReadFromFile(timeMagnitudeFile);

        for (int index = 0; index < time.Count; ++index)
            time[index] = (float)Math.Round(time[index] * 1000, 2);

        chartDrawer.Draw(time, magnitude, phase);
    }

}
