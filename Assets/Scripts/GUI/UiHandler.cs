using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NWaves.Signals;
using UnityEngine;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class UiHandler
{
    private ChartDrawer chartDrawer
    {
        get;
    }

    private GameObject menuCanvas
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

    public UiHandler(UiTimeEchogram uiTimeEcho,
                     UiFrequencyEchogram freqEcho,
                     UiImpulseResponse uiImpulseResp,
                     List<double> frequencies)
    {
        this.menuCanvas = GameObject.Find("MenuObject").gameObject.transform.Find("Menu").gameObject;
        chartDrawer = new ChartDrawer(this.menuCanvas);

        uiTimeEchogram = uiTimeEcho;
        uiFrequencyEchogram = freqEcho;
        uiImpulseResponse = uiImpulseResp;
        this.frequencies = frequencies;
    }

    private List<double> frequencies { get; set; }

    private float step
    {
        get
        {
            return (frequencies.Count == 0) ? 0 : (float)(1 / (2 * frequencies[frequencies.Count - 1])); ;
        }
    }

    public void InitializeUi(
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms,
        Echogram frequencyResponse,
        Dictionary<int, DiscreteSignal> impulseResponses)
    {
        AddListenerForShowTimeButton(rays, microphones, echograms);
        AddListenerForShowFrequencyButton(frequencyResponse, microphones);
        AddListenerForShowImpulseResponse(impulseResponses, microphones);
    }

    private void AddListenerForShowTimeButton(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms)
    {
        uiTimeEchogram.ShowButton.onClick.AddListener(() =>
        {
            ShowTimeChart(rays,
                microphones,
                echograms);
        });
    }

    private void ShowTimeChart(
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms)
    {
        var numberOfMicrophone = InputHandler.GetNumber(uiTimeEchogram.MicrophoneInputField);
        var frequency = InputHandler.GetNumber(uiTimeEchogram.FrequencyInputField);

        if (numberOfMicrophone != -1 && frequency != -1 && frequencies.Count > 0)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.Id == numberOfMicrophone)
                    okToDraw = true;

            double freq = frequencies.Aggregate((x, y) => Math.Abs(x - frequency) < Math.Abs(y - frequency) ? x : y);
           // uiTimeEchogram.FrequencyInputField.text = freq.ToString();

            if (okToDraw == true)
                DrawTimeEchogram(rays, microphones, echograms, numberOfMicrophone, freq);
            else
                Debug.Log("The microphone for which you want to see the result does not exist.");
        }
    }

    private void DrawTimeEchogram(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms,
        int indexMicrophone,
        double indexFrequency)
    {
        var (time, magnitude, phase) = GetPhaseAndMagnitude(rays, microphones, echograms, indexMicrophone, indexFrequency);

        using (var outputFile = new StreamWriter("xTimeMagnitude.txt"))
        {
            foreach (var x in time)
            {
                outputFile.WriteLine(x);
            }
        }

        using (var outputFile = new StreamWriter("yTimeMagnitude.txt"))
        {
            foreach (var y in magnitude)
            {
                outputFile.WriteLine(y);
            }
        }

        using (var outputFile = new StreamWriter("xTimePhase.txt"))
        {
            foreach (var x in time)
            {
                outputFile.WriteLine(x);
            }
        }

        using (var outputFile = new StreamWriter("yTimePhase.txt"))
        {
            foreach (var y in phase)
            {
                outputFile.WriteLine(y);
            }
        }

        for (int index = 0; index < time.Count; ++index)
            time[index] = (float)Math.Round(time[index] * 1000, 2);

        chartDrawer.DrawTimeChart(time, magnitude, phase);
    }

    private void AddListenerForShowFrequencyButton(Echogram frequencyResponse,
                                                  List<MicrophoneSphere> microphones)
    {
        uiFrequencyEchogram.ShowButton.onClick.AddListener(() =>
        {
            ShowFrequencyChart(frequencyResponse,
                microphones);
        });
    }

    private void ShowFrequencyChart(Echogram frequencyResponse,
                                    List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophone = InputHandler.GetNumber(uiFrequencyEchogram.MicrophoneInputField);

        if (numberOfMicrophone != -1)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.Id == numberOfMicrophone)
                    okToDraw = true;

            if (okToDraw == true)
                DrawChartFrequency(frequencyResponse, numberOfMicrophone);
            else
                Debug.Log("The microphone for which you want to see the result does not exist.");
        }
    }

    private void DrawChartFrequency(Echogram frequencyResponse,
                                    int indexMicrophone)
    {
        var (magnitude, phase) = GetMagnitudeAndPhase(frequencyResponse, indexMicrophone);
        var freq = frequencies.Select(frequency => (float)frequency).ToList();

        chartDrawer.DrawFrequencyChart(freq, magnitude, phase);
    }

    private void AddListenerForShowImpulseResponse(Dictionary<int, DiscreteSignal> impulseResponses,
       List<MicrophoneSphere> microphones)
    {
        uiImpulseResponse.ShowButton.onClick.AddListener(() =>
        {
            ShowImpulseResponseChart(impulseResponses, microphones);
        });
    }

    private void ShowImpulseResponseChart(Dictionary<int, DiscreteSignal> impulseResponses,
                                          List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophone = InputHandler.GetNumber(uiImpulseResponse.MicrophoneInputField);

        if (numberOfMicrophone != -1)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.Id == numberOfMicrophone)
                    okToDraw = true;

            if (okToDraw == true)
                DrawChartImpulseResponse(impulseResponses, numberOfMicrophone);
            else
                Debug.Log("The microphone for which you want to see the result does not exist.");
        }
    }

    private void DrawChartImpulseResponse(Dictionary<int, DiscreteSignal> impulseResponses,
                                          int numberOfMicrophone)
    {
        if (!impulseResponses.ContainsKey(numberOfMicrophone))
            return;

        var xTime = new List<float>() { 0 };

        for (int index = 1; index < impulseResponses[numberOfMicrophone].Samples.Length; ++index)
            xTime.Add(step + xTime[index - 1]);

        var signal = impulseResponses[numberOfMicrophone];
        var yImpulseResponse = new List<float>(signal.Samples);

        chartDrawer.DrawImpulseResponseChart(xTime, yImpulseResponse);
    }

    private Tuple<List<float>, List<float>, List<float>> GetPhaseAndMagnitude(
        Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms,
        int idMicro,
        double freq)
    {
        var distanceCalculator = new DistanceCalculator(rays, microphones);
        distanceCalculator.ComputeDistances();

        var xTime = TimeCalculator.GetTimeForMicrophone(rays, microphones, idMicro);
        var yMagnitude = new List<float>();
        var yPhase = new List<float>();
        var microphoneIntensities = echograms[freq][idMicro];

        for (int index = 0; index < microphoneIntensities.Count; ++index)
        {
            yPhase.Add((float)(microphoneIntensities[index].Phase * 180 / Math.PI));
            yMagnitude.Add((float)microphoneIntensities[index].Magnitude);
        }

        return new Tuple<List<float>, List<float>, List<float>>(xTime, yMagnitude, yPhase);
    }

    private Tuple<List<float>, List<float>> GetMagnitudeAndPhase(Echogram frequencyResponse, int idMicro)
    {
        if (!frequencyResponse.ContainsKey(idMicro))
            return new Tuple<List<float>, List<float>>(new List<float>(), new List<float>());

        var x = new List<float>();
        var y = new List<float>();
        for (int index = 0; index < frequencyResponse[idMicro].Count; ++index)
        {
            x.Add((float)frequencyResponse[idMicro][index].Magnitude);
            y.Add((float)frequencyResponse[idMicro][index].Phase);
        }

        return new Tuple<List<float>, List<float>>(x, y);
    }
}
