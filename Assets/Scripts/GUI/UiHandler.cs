using System;
using System.Collections.Generic;
using System.Linq;
using NWaves.Signals;
using UnityEngine;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class UiHandler
{
    private ChartDrawer m_chartDrawer
    {
        get;
    }

    GameObject menuCanvas
    {
        get;
    }

    private UiTimeEchogram m_uiTimeEchogram
    {
        get;
    }

    private UiFrequencyEchogram m_uiFrequencyEchogram
    {
        get;
    }

    private UiImpulseResponse m_uiImpulseResponse
    {
        get;
    }

    public UiHandler(GameObject menuCanvas,
                     UiTimeEchogram uiTimeEcho,
                     UiFrequencyEchogram freqEcho,
                     UiImpulseResponse uiImpulseResp,
                     List<double> frequencies)
    {
        this.menuCanvas = menuCanvas;
        m_chartDrawer = new ChartDrawer(this.menuCanvas);

        m_uiTimeEchogram = uiTimeEcho;
        m_uiFrequencyEchogram = freqEcho;
        m_uiImpulseResponse = uiImpulseResp;
        m_frequencies = frequencies;
    }

    private List<double> m_frequencies { get; set; }

    private float m_step
    {
        get
        {
            return (m_frequencies.Count == 0) ? 0 : (float)(1 / (2 * m_frequencies[m_frequencies.Count - 1])); ;
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
        AddListenerForShowFrquencyButton(frequencyResponse, microphones);
        AddListenerForShowImpulseResponse(impulseResponses, microphones);
    }

    private void AddListenerForShowTimeButton(Dictionary<int, List<AcousticRay>> rays,
        List<MicrophoneSphere> microphones,
        Dictionary<double, Echogram> echograms)
    {
        m_uiTimeEchogram.showButton.onClick.AddListener(() =>
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
        var numberOfMicrophone = InputHandler.GetNumber(m_uiTimeEchogram.microphoneInputField);
        var frequency = InputHandler.GetNumber(m_uiTimeEchogram.frequencyInputField);

        if (numberOfMicrophone != -1 && frequency != -1)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.id == numberOfMicrophone)
                    okToDraw = true;   

            double freq = m_frequencies.Aggregate((x, y) => Math.Abs(x - frequency) < Math.Abs(y - frequency) ? x : y);
            m_uiTimeEchogram.frequencyInputField.text = freq.ToString();

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

        for (int index = 0; index < time.Count; ++index)
            time[index] = (float)Math.Round(time[index] * 1000, 2);

        m_chartDrawer.DrawTimeChart(time, magnitude, phase);
    }

    private void AddListenerForShowFrquencyButton(Echogram frequencyResponse, 
                                                  List<MicrophoneSphere> microphones)
    {
        m_uiFrequencyEchogram.showButton.onClick.AddListener(() =>
        {
            ShowFrequencyChart(frequencyResponse,
                microphones);
        });
    }

    private void ShowFrequencyChart(Echogram frequencyResponse, 
                                    List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophone = InputHandler.GetNumber(m_uiFrequencyEchogram.microphoneInputField);

        if (numberOfMicrophone != -1)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.id == numberOfMicrophone)
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
        var freq = m_frequencies.Select(frequency => (float)frequency).ToList();

        m_chartDrawer.DrawFrequencyChart(freq, magnitude, phase);
    }

    private void AddListenerForShowImpulseResponse(Dictionary<int, DiscreteSignal> impulseResponses,
       List<MicrophoneSphere> microphones)
    {
        m_uiImpulseResponse.showButton.onClick.AddListener(() =>
        {
            ShowImpulseResponseChart(impulseResponses, microphones);
        });
    }

    private void ShowImpulseResponseChart(Dictionary<int, DiscreteSignal> impulseResponses,
                                          List<MicrophoneSphere> microphones)
    {
        var numberOfMicrophone = InputHandler.GetNumber(m_uiImpulseResponse.microphoneInputField);

        if (numberOfMicrophone != -1)
        {
            var okToDraw = false;
            foreach (var micro in microphones)
                if (micro.id == numberOfMicrophone)
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
        var xTime = new List<float>() { 0 };

        for (int index = 1; index < impulseResponses[numberOfMicrophone].Samples.Length; ++index)
            xTime.Add(m_step + xTime[index - 1]);

        var signal = impulseResponses[numberOfMicrophone];
        var yImpulseResponse = new List<float>(signal.Samples);

        m_chartDrawer.DrawImpulseResponseChart(xTime, yImpulseResponse);
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
