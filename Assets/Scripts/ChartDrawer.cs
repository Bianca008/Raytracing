using System.Collections.Generic;
using UnityEngine;
using XCharts;

public class ChartDrawer
{
    public GameObject chartArea
    {
        get;
        set;
    }

    public bool enable
    {
        get => m_TimeMagnitudeChart.enabled;
        set => m_TimeMagnitudeChart.enabled = value;
    }

    private BarChart m_TimePhaseChart;
    private BarChart m_TimeMagnitudeChart;

    private LineChart m_TimePhaseLineChart;
    private LineChart m_TimeMagnitudeLineChart;
    private LineChart m_impulseResponseLineChart;

    public ChartDrawer(GameObject canvas)
    {
        chartArea = canvas;
    }

    public void DrawTimeChart(List<float> xTime, List<float> yMagnitude, List<float> yPhase)
    {
        foreach (var barChart in chartArea.GetComponentsInChildren<BarChart>())
        {
            switch (barChart.name)
            {
                case "TimeMagnitude":
                    m_TimeMagnitudeChart = barChart;
                    break;
                case "TimePhase":
                    m_TimePhaseChart = barChart;
                    break;
            }
        }

        m_TimeMagnitudeChart.title.show = true;
        m_TimeMagnitudeChart.title.text = "Time(1e+3)-Magnitude chart";
        m_TimeMagnitudeChart.tooltip.show = true;
        m_TimeMagnitudeChart.legend.show = false;
        m_TimeMagnitudeChart.xAxises[0].show = true;
        m_TimeMagnitudeChart.xAxises[1].show = false;
        m_TimeMagnitudeChart.yAxises[0].show = true;
        m_TimeMagnitudeChart.yAxises[1].show = false;
        m_TimeMagnitudeChart.xAxises[0].type = Axis.AxisType.Category;
        m_TimeMagnitudeChart.yAxises[0].type = Axis.AxisType.Value;
        m_TimeMagnitudeChart.xAxises[0].splitNumber = 10;
        m_TimeMagnitudeChart.xAxises[0].boundaryGap = true;
        m_TimeMagnitudeChart.RemoveData();
        m_TimeMagnitudeChart.AddSerie(SerieType.Bar, "Magnitude");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_TimeMagnitudeChart.AddXAxisData(xTime[index].ToString());
            m_TimeMagnitudeChart.AddData(0, yMagnitude[index]);
        }

        m_TimePhaseChart.title.show = true;
        m_TimePhaseChart.title.text = "Time(1e+3)-Phase chart";
        m_TimePhaseChart.tooltip.show = true;
        m_TimePhaseChart.legend.show = false;
        m_TimePhaseChart.xAxises[0].show = true;
        m_TimePhaseChart.xAxises[1].show = false;
        m_TimePhaseChart.yAxises[0].show = true;
        m_TimePhaseChart.yAxises[1].show = false;
        m_TimePhaseChart.xAxises[0].type = Axis.AxisType.Category;
        m_TimePhaseChart.yAxises[0].type = Axis.AxisType.Value;
        m_TimePhaseChart.xAxises[0].splitNumber = 10;
        m_TimePhaseChart.xAxises[0].boundaryGap = true;
        m_TimePhaseChart.RemoveData();
        m_TimePhaseChart.AddSerie(SerieType.Bar, "Phase");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_TimePhaseChart.AddXAxisData(xTime[index].ToString());
            m_TimePhaseChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawFrequencyChart(List<float> xFrequency, List<float> yMagnitude, List<float> yPhase)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            switch (lineChart.name)
            {
                case "FrequencyMagnitude":
                    m_TimeMagnitudeLineChart = lineChart;
                    break;
                case "FrequencyPhase":
                    m_TimePhaseLineChart = lineChart;
                    break;
            }
        }

        m_TimeMagnitudeLineChart.title.show = true;
        m_TimeMagnitudeLineChart.title.text = "Frequency(Hz)-Magnitude chart";
        m_TimeMagnitudeLineChart.tooltip.show = true;
        m_TimeMagnitudeLineChart.legend.show = false;
        m_TimeMagnitudeLineChart.xAxises[0].show = true;
        m_TimeMagnitudeLineChart.xAxises[1].show = false;
        m_TimeMagnitudeLineChart.yAxises[0].show = true;
        m_TimeMagnitudeLineChart.yAxises[1].show = false;
        m_TimeMagnitudeLineChart.xAxises[0].type = Axis.AxisType.Category;
        m_TimeMagnitudeLineChart.yAxises[0].type = Axis.AxisType.Value;
        m_TimeMagnitudeLineChart.xAxises[0].splitNumber = 10;
        m_TimeMagnitudeLineChart.xAxises[0].boundaryGap = true;
        m_TimeMagnitudeLineChart.RemoveData();
        m_TimeMagnitudeLineChart.AddSerie(SerieType.Line, "Magnitude").sampleDist = 1;

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            m_TimeMagnitudeLineChart.AddXAxisData(xFrequency[index].ToString());
            m_TimeMagnitudeLineChart.AddData(0, yMagnitude[index]);
        }

        m_TimePhaseLineChart.title.show = true;
        m_TimePhaseLineChart.title.text = "Frequency(Hz)-Phase chart";
        m_TimePhaseLineChart.tooltip.show = true;
        m_TimePhaseLineChart.legend.show = false;
        m_TimePhaseLineChart.xAxises[0].show = true;
        m_TimePhaseLineChart.xAxises[1].show = false;
        m_TimePhaseLineChart.yAxises[0].show = true;
        m_TimePhaseLineChart.yAxises[1].show = false;
        m_TimePhaseLineChart.xAxises[0].type = Axis.AxisType.Category;
        m_TimePhaseLineChart.yAxises[0].type = Axis.AxisType.Value;
        m_TimePhaseLineChart.xAxises[0].splitNumber = 10;
        m_TimePhaseLineChart.xAxises[0].boundaryGap = true;
        m_TimePhaseLineChart.RemoveData();
        m_TimePhaseLineChart.AddSerie(SerieType.Line, "Phase").sampleDist = 1;

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            m_TimePhaseLineChart.AddXAxisData(xFrequency[index].ToString());
            m_TimePhaseLineChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawImpulseResponseChart(List<float> xTime, List<float> yImpulseResponse)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            if (lineChart.name == "ImpulseResponse")
                m_impulseResponseLineChart = lineChart;
        }

        m_impulseResponseLineChart.title.show = true;
        m_impulseResponseLineChart.title.text = "Impulse response";
        m_impulseResponseLineChart.tooltip.show = true;
        m_impulseResponseLineChart.legend.show = false;
        m_impulseResponseLineChart.xAxises[0].show = true;
        m_impulseResponseLineChart.xAxises[1].show = false;
        m_impulseResponseLineChart.yAxises[0].show = true;
        m_impulseResponseLineChart.yAxises[1].show = false;
        m_impulseResponseLineChart.xAxises[0].type = Axis.AxisType.Category;
        m_impulseResponseLineChart.yAxises[0].type = Axis.AxisType.Value;
        m_impulseResponseLineChart.xAxises[0].splitNumber = 10;
        m_impulseResponseLineChart.xAxises[0].boundaryGap = true;
        m_impulseResponseLineChart.RemoveData();
        m_impulseResponseLineChart.AddSerie(SerieType.Line).sampleDist = 1;

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_impulseResponseLineChart.AddXAxisData(xTime[index].ToString());
            m_impulseResponseLineChart.AddData(0, yImpulseResponse[index]);
        }

        //m_impulseResponseLineChart.RefreshAxisMinMaxValue();
    }
}
