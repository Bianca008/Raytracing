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

    private BarChart timePhaseBarChart;
    private BarChart timeMagnitudeBarChart;

    private LineChart timePhaseLineChart;
    private LineChart timeMagnitudeLineChart;
    private LineChart impulseResponseLineChart;

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
                    timeMagnitudeBarChart = barChart;
                    break;
                case "TimePhaseBarChart":
                    timePhaseBarChart = barChart;
                    break;
            }
        }

        SetBarChart(timeMagnitudeBarChart, "Time(1e+3)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xTime.Count; ++index)
        {
            timeMagnitudeBarChart.AddXAxisData(xTime[index].ToString());
            timeMagnitudeBarChart.AddData(0, yMagnitude[index]);
        }

        SetBarChart(timePhaseBarChart, "Time(1e+3)-Phase chart", "Phase");

        for (int index = 0; index < xTime.Count; ++index)
        {
            timePhaseBarChart.AddXAxisData(xTime[index].ToString());
            timePhaseBarChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawFrequencyChart(List<float> xFrequency, List<float> yMagnitude, List<float> yPhase)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            switch (lineChart.name)
            {
                case "FrequencyMagnitudeLineChart":
                    timeMagnitudeLineChart = lineChart;
                    break;
                case "FrequencyPhaseLineChart":
                    timePhaseLineChart = lineChart;
                    break;
            }
        }

        SetLineChart(timeMagnitudeLineChart, "Frequency(Hz)-Magnitude chart", "Magnitude");

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            timeMagnitudeLineChart.AddXAxisData(xFrequency[index].ToString());
            timeMagnitudeLineChart.AddData(0, yMagnitude[index]);
        }

        SetLineChart(timePhaseLineChart, "Frequency(Hz) - Phase chart", "Phase");

        for (int index = 0; index < xFrequency.Count; ++index)
        {
            timePhaseLineChart.AddXAxisData(xFrequency[index].ToString());
            timePhaseLineChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawImpulseResponseChart(List<float> xTime, List<float> yImpulseResponse)
    {
        foreach (var lineChart in chartArea.GetComponentsInChildren<LineChart>())
        {
            if (lineChart.name == "ImpulseResponseLineChart")
                impulseResponseLineChart = lineChart;
        }

        SetLineChart(impulseResponseLineChart, "Impulse response");
        var newXtime = AverageValues.CreateSmallerDataSet(xTime, 30);
        var newYimpulseResponse = AverageValues.CreateSmallerDataSet(yImpulseResponse, 30);
        for (int index = 0; index < newXtime.Count; ++index)
        {
            /*TODO: vezi unitati de masura.*/
            impulseResponseLineChart.AddXAxisData(newXtime[index].ToString());
            impulseResponseLineChart.AddData(0, newYimpulseResponse[index]);
        }
    }

    private void SetLineChart(LineChart chart, String name, String tag = null)
    {
        chart.ClearAxisData();
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
        chart.ClearAxisData();
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
