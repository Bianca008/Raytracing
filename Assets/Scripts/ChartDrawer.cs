﻿using System;
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

    private BarChart m_timePhaseBarChart;
    private BarChart m_timeMagnitudeBarChart;

    private LineChart m_timePhaseLineChart;
    private LineChart m_timeMagnitudeLineChart;
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
                case "TimeMagnitudeBarChart":
                    m_timeMagnitudeBarChart = barChart;
                    break;
                case "TimePhaseBarChart":
                    m_timePhaseBarChart = barChart;
                    break;
            }
        }

        SetBarChart(m_timeMagnitudeBarChart, "Time(1e+3)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_timeMagnitudeBarChart.AddXAxisData(xTime[index].ToString());
            m_timeMagnitudeBarChart.AddData(0, yMagnitude[index]);
        }

        SetBarChart(m_timePhaseBarChart, "Time(1e+3)-Phase chart", "Phase");

        for (int index = 0; index < xTime.Count; ++index)
        {
            m_timePhaseBarChart.AddXAxisData(xTime[index].ToString());
            m_timePhaseBarChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawFrequencyChart(List<float> xFrequency, List<float> yMagnitude, List<float> yPhase)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            switch (lineChart.name)
            {
                case "FrequencyMagnitudeLineChart":
                    m_timeMagnitudeLineChart = lineChart;
                    break;
                case "FrequencyPhaseLineChart":
                    m_timePhaseLineChart = lineChart;
                    break;
            }
        }

        SetLineChart(m_timeMagnitudeLineChart, "Frequency(Hz)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            m_timeMagnitudeLineChart.AddXAxisData(xFrequency[index].ToString());
            m_timeMagnitudeLineChart.AddData(0, yMagnitude[index]);
        }

        SetLineChart(m_timePhaseLineChart, "Frequency(Hz) - Phase chart", "Phase");

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            m_timePhaseLineChart.AddXAxisData(xFrequency[index].ToString());
            m_timePhaseLineChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawImpulseResponseChart(List<float> xTime, List<float> yImpulseResponse)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            if (lineChart.name == "ImpulseResponseLineChart")
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
