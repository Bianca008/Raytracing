using System;
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
        get => m_TimeMagnitudeBarChart.enabled;
        set => m_TimeMagnitudeBarChart.enabled = value;
    }

    private BarChart m_TimePhaseBarChart;
    private BarChart m_TimeMagnitudeBarChart;

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
                    m_TimeMagnitudeBarChart = barChart;
                    break;
                case "TimePhase":
                    m_TimePhaseBarChart = barChart;
                    break;
            }
        }

        SetBarChart(m_TimeMagnitudeBarChart, "Time(1e+3)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_TimeMagnitudeBarChart.AddXAxisData(xTime[index].ToString());
            m_TimeMagnitudeBarChart.AddData(0, yMagnitude[index]);
        }

        SetBarChart(m_TimePhaseBarChart, "Time(1e+3)-Phase chart", "Phase");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_TimePhaseBarChart.AddXAxisData(xTime[index].ToString());
            m_TimePhaseBarChart.AddData(0, yPhase[index]);
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

        SetLineChart(m_TimeMagnitudeLineChart, "Frequency(Hz)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            m_TimeMagnitudeLineChart.AddXAxisData(xFrequency[index].ToString());
            m_TimeMagnitudeLineChart.AddData(0, yMagnitude[index]);
        }

        SetLineChart(m_TimePhaseLineChart, "Frequency(Hz) - Phase chart", "Phase");

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

        //m_impulseResponseLineChart.yAxises[0].min = -5000;
        //m_impulseResponseLineChart.yAxises[0].max = 25000;
        ///*nu am reusit sa imi dau seama exact ce se intampla,
        // dar am vazut ca modifica valorile care sunt pe axa y*/
        //m_impulseResponseLineChart.yAxises[0].ceilRate = 1;
        ///*adauga mai multe valori pe axa y*/
        //m_impulseResponseLineChart.yAxises[0].interval = 1;

        SetLineChart(m_impulseResponseLineChart, "Impulse response");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_impulseResponseLineChart.AddXAxisData(xTime[index].ToString());
            m_impulseResponseLineChart.AddData(0, yImpulseResponse[index]);
        }
    }

    private void SetLineChart(LineChart chart, String name, String tag = null)
    {
        chart.yAxis0.min = 0;
        chart.title.show = true;
        chart.title.text = name;
        chart.tooltip.show = true;
        chart.legend.show = false;
        chart.xAxises[0].show = true;
        chart.xAxises[1].show = false;
        chart.yAxises[0].show = true;
        chart.yAxises[1].show = false;
        chart.xAxises[0].type = Axis.AxisType.Category;
        chart.yAxises[0].type = Axis.AxisType.Value;
        chart.xAxises[0].splitNumber = 10;
        chart.xAxises[0].boundaryGap = true;
        chart.RemoveData();

        if (tag == null)
            chart.AddSerie(SerieType.Line).sampleDist = 1;
        else
            chart.AddSerie(SerieType.Line, tag).sampleDist = 1;
    }

    private void SetBarChart(BarChart chart, String name, String tag = null)
    {
        chart.title.show = true;
        chart.title.text = name;
        chart.tooltip.show = true;
        chart.legend.show = false;
        chart.xAxises[0].show = true;
        chart.xAxises[1].show = false;
        chart.yAxises[0].show = true;
        chart.yAxises[1].show = false;
        chart.xAxises[0].type = Axis.AxisType.Category;
        chart.yAxises[0].type = Axis.AxisType.Value;
        chart.xAxises[0].splitNumber = 10;
        chart.xAxises[0].boundaryGap = true;
        chart.RemoveData();

        if (tag == null)
            chart.AddSerie(SerieType.Bar);
        else
            chart.AddSerie(SerieType.Bar, tag);
    }
}
