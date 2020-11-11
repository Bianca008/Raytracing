using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NWaves.Signals;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler
{
    private ChartDrawer chartDrawer
    {
        get;
    }

    GameObject menuCanvas
    {
        get;
    }

    private UiTimeEchogram uiTimeEchogram
    {
        get;
    }

    private UiFrequencyEchogram uiFrequencyEchogram
    {
        get;
    }

    private UiImpulseResponse uiImpulseResponse
    {
        get;
    }

    public UiHandler(GameObject menuCanvas,
       UiTimeEchogram uiTimeEcho,
       UiFrequencyEchogram freqEcho,
       UiImpulseResponse uiImpulseResp)
    {
        this.menuCanvas = menuCanvas;
        chartDrawer = new ChartDrawer(this.menuCanvas);

        uiTimeEchogram = uiTimeEcho;
        uiFrequencyEchogram = freqEcho;
        uiImpulseResponse = uiImpulseResp;
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
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophoneStr = uiFrequencyEchogram.microphoneInputField.text;
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

    private void ShowTimeChart(
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        var frequencyFieldStr = uiTimeEchogram.frequencyInputField.text;
        var numberOfMicrophoneStr = uiTimeEchogram.microphoneInputField.text;
        var numberOfMicrophone = 0;
        var frequency = 0.0;
        if (numberOfMicrophoneStr.All(char.IsDigit) == true)
            numberOfMicrophone = Int32.Parse(numberOfMicrophoneStr);
        if (frequencyFieldStr.All(char.IsDigit) == true)
            frequency = Int32.Parse(frequencyFieldStr);

        var okToDraw = false;
        foreach (var micro in microphones)
            if (micro.id == numberOfMicrophone)
                okToDraw = true;

        var okToDrawFr = false;
        foreach (var fr in frequencies)
            if (Math.Abs((int)fr - frequency) < 1e-2)
                okToDrawFr = true;

        if (okToDraw == true && okToDrawFr == true)
            DrawTimeEchogram(numberOfMicrophone, frequency);
        else
            Debug.Log("The microphone for which you want to see the result does not exist.");
    }

    public void InitializeUi(
        Button showFrequencyEchogramButton,
        Button showTimeEchogramButton,
        Button showImpulseResponseButton,
        Dictionary<int, DiscreteSignal> impulseResponses,
        List<MicrophoneSphere> microphones,
        List<double> frequencies,
        float step)
    {
        SetAllUiElementsInactive();

        AddListenerForShowFrequencyEchogram(showFrequencyEchogramButton);
        AddListenerForShowButton(frequencies, microphones);
        AddListenerForShowTimeEchogram(showTimeEchogramButton);
        AddListenerForShowTimeButton(frequencies, microphones);
        AddListenerForShowImpulseResponse(impulseResponses, microphones, step);
        AddListenerForShowImpulseResponseButton(showImpulseResponseButton);
    }

    private void AddListenerForShowImpulseResponseButton(Button showImpulseResponseButton)
    {
        showImpulseResponseButton.onClick.AddListener(SetActiveImpulseResponseUi);
    }

    private void AddListenerForShowImpulseResponse(Dictionary<int, DiscreteSignal> impulseResponses, 
        List<MicrophoneSphere> microphones,
        float step)
    {
        uiImpulseResponse.showButton.onClick.AddListener(() =>
        {
            ShowImpulseResponseChart(impulseResponses, microphones, step);
        });
    }

    private void ShowImpulseResponseChart(Dictionary<int, DiscreteSignal> impulseResponses,
        List<MicrophoneSphere> microphones,
        float step)
    {
        var numberOfMicrophoneStr = uiFrequencyEchogram.microphoneInputField.text;
        var numberOfMicrophone = 0;

        if (numberOfMicrophoneStr.All(char.IsDigit) == true)
            numberOfMicrophone = Int32.Parse(numberOfMicrophoneStr) - 1;

        var okToDraw = false;
        foreach (var micro in microphones)
            if (micro.id == numberOfMicrophone)
                okToDraw = true;

        if (okToDraw == true)
            DrawChartImpulseResponse(impulseResponses, step, numberOfMicrophone);
        else
            Debug.Log("The microphone for which you want to see the result does not exist.");
    }

    private void DrawChartImpulseResponse(Dictionary<int, DiscreteSignal> impulseResponses,
        float step,
        int numberOfMicrophone)
    {
        var xTime = new List<float>() { 0 };

        for (int index = 1; index < impulseResponses[numberOfMicrophone].Samples.Length; ++index)
            xTime.Add(step + xTime[index - 1]);

        var signal = impulseResponses[numberOfMicrophone];
        var yImpulseResponse = new List<float>(signal.Samples);

        using (StreamWriter outputFile = new StreamWriter("impulseTime.txt"))
        {
            for (int index = 0; index < xTime.Count; ++index)
            {
                outputFile.WriteLine(xTime[index] + "," );
            }
        }

        using (StreamWriter outputFile = new StreamWriter("impulse.txt"))
        {
            for (int index = 0; index < xTime.Count; ++index)
            {
                outputFile.WriteLine(yImpulseResponse[index] + ",");
            }
        }

        chartDrawer.DrawImpulseResponseChart(xTime, yImpulseResponse);
    }

    private void AddListenerForShowButton(
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        uiFrequencyEchogram.showButton.onClick.AddListener(() =>
        {
            ShowFrequencyChart(
                frequencies,
                microphones);
        });
    }

    private void AddListenerForShowTimeButton(
        List<double> frequencies,
        List<MicrophoneSphere> microphones)
    {
        uiTimeEchogram.showButton.onClick.AddListener(() =>
        {
            ShowTimeChart(
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
        SetAllUiElementsInactive();

        var inputTimePanel = menuCanvas.transform.Find("InputTimePanel").gameObject;
        inputTimePanel.SetActive(true);
        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var timePanel = buttonsAndPlotPanel.transform.Find("TimePanel").gameObject;
        timePanel.SetActive(true);
    }

    private void SetActiveFrequencyEchogramUi()
    {
        SetAllUiElementsInactive();

        var inputPanel = menuCanvas.transform.Find("InputPanel").gameObject;
        inputPanel.SetActive(true);
        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var frequencyPanel = buttonsAndPlotPanel.transform.Find("FrequencyPanel").gameObject;
        frequencyPanel.SetActive(true);
    }

    private void SetActiveImpulseResponseUi()
    {
        SetAllUiElementsInactive();

        var inputPanel = menuCanvas.transform.Find("InputImpulseResponsePanel").gameObject;
        inputPanel.SetActive(true);
        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var frequencyPanel = buttonsAndPlotPanel.transform.Find("ImpulseResponsePanel").gameObject;
        frequencyPanel.SetActive(true);
    }

    private void DrawTimeEchogram(int indexMicrophone, double indexFrequency)
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

        chartDrawer.DrawTimeChart(time, magnitude, phase);
    }

    private void SetAllUiElementsInactive()
    {
        var inputTimePanel = menuCanvas.transform.Find("InputTimePanel").gameObject;
        inputTimePanel.SetActive(false);
        var inputPanel = menuCanvas.transform.Find("InputPanel").gameObject;
        inputPanel.SetActive(false);
        var inputImpulseResponsePanel = menuCanvas.transform.Find("InputImpulseResponsePanel").gameObject;
        inputImpulseResponsePanel.SetActive(false);

        var buttonsAndPlotPanel = menuCanvas.transform.Find("ButtonsAndPlotPanel").gameObject;
        var frequencyPanel = buttonsAndPlotPanel.transform.Find("FrequencyPanel").gameObject;
        frequencyPanel.SetActive(false);
        var timePanel = buttonsAndPlotPanel.transform.Find("TimePanel").gameObject;
        timePanel.SetActive(false);
        var impulseResponsePanel = buttonsAndPlotPanel.transform.Find("ImpulseResponsePanel").gameObject;
        impulseResponsePanel.SetActive(false);
    }

}
