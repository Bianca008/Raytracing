using System.Collections.Generic;
using UnityEngine;
using XCharts;

public class ChartDrawer
{
    public GameObject ChartArea
    {
        get;
        set;
    }

    public bool Enable
    {
        get => timeMagnitudeChart.enabled;
        set => timeMagnitudeChart.enabled = value;
    }

    private BarChart timePhaseChart;
    private BarChart timeMagnitudeChart;

    private LineChart timePhaseLineChart;
    private LineChart timeMagnitudeLineChart;

    public ChartDrawer(GameObject canvas)
    {
        ChartArea = canvas;
    }

    public void Draw(List<float> xTime, List<float> yMagnitude, List<float> yPhase)
    {
        foreach(BarChart barChart in ChartArea.GetComponentsInChildren<BarChart>())
        {
            if (barChart.name == "TimeMagnitude")
                timeMagnitudeChart = barChart;
            if (barChart.name == "TimePhase")
                timePhaseChart = barChart;
        }
        
        timeMagnitudeChart.title.show = true;
        timeMagnitudeChart.title.text = "Time(1e+3)-Magnitude chart";
        timeMagnitudeChart.tooltip.show = true;
        timeMagnitudeChart.legend.show = false;
        timeMagnitudeChart.xAxises[0].show = true;
        timeMagnitudeChart.xAxises[1].show = false;
        timeMagnitudeChart.yAxises[0].show = true;
        timeMagnitudeChart.yAxises[1].show = false;
        timeMagnitudeChart.xAxises[0].type = Axis.AxisType.Category;
        timeMagnitudeChart.yAxises[0].type = Axis.AxisType.Value;
        timeMagnitudeChart.xAxises[0].splitNumber = 10;
        timeMagnitudeChart.xAxises[0].boundaryGap = true;
        timeMagnitudeChart.RemoveData();
        timeMagnitudeChart.AddSerie(SerieType.Bar, "Magnitude");

        for (int index = 0; index < xTime.Count; ++index)
        {
            timeMagnitudeChart.AddXAxisData(xTime[index].ToString());
            timeMagnitudeChart.AddData(0, yMagnitude[index]);
        }

        timePhaseChart.title.show = true;
        timePhaseChart.title.text = "Time(1e+3)-Phase chart";
        timePhaseChart.tooltip.show = true;
        timePhaseChart.legend.show = false;
        timePhaseChart.xAxises[0].show = true;
        timePhaseChart.xAxises[1].show = false;
        timePhaseChart.yAxises[0].show = true;
        timePhaseChart.yAxises[1].show = false;
        timePhaseChart.xAxises[0].type = Axis.AxisType.Category;
        timePhaseChart.yAxises[0].type = Axis.AxisType.Value;
        timePhaseChart.xAxises[0].splitNumber = 10;
        timePhaseChart.xAxises[0].boundaryGap = true;
        timePhaseChart.RemoveData();
        timePhaseChart.AddSerie(SerieType.Bar, "Phase");

        for (int index = 0; index < xTime.Count; ++index)
        {
            timePhaseChart.AddXAxisData(xTime[index].ToString());
            timePhaseChart.AddData(0, yPhase[index]);
        }
    }

    public void DrawFrequencieChart(List<float> xFrquencies, List<float> yMagnitude, List<float> yPhase)
    {
        foreach (LineChart lineChart in ChartArea.GetComponentsInChildren<LineChart>())
        {
            if (lineChart.name == "FrequencyMagnitude")
                timeMagnitudeLineChart = lineChart;
            if (lineChart.name == "FrequencyPhase")
                timePhaseLineChart = lineChart;
        }

        timeMagnitudeLineChart.title.show = true;
        timeMagnitudeLineChart.title.text = "Frequency(Hz)-Magnitude chart";
        timeMagnitudeLineChart.tooltip.show = true;
        timeMagnitudeLineChart.legend.show = false;
        timeMagnitudeLineChart.xAxises[0].show = true;
        timeMagnitudeLineChart.xAxises[1].show = false;
        timeMagnitudeLineChart.yAxises[0].show = true;
        timeMagnitudeLineChart.yAxises[1].show = false;
        timeMagnitudeLineChart.xAxises[0].type = Axis.AxisType.Category;
        timeMagnitudeLineChart.yAxises[0].type = Axis.AxisType.Value;
        timeMagnitudeLineChart.xAxises[0].splitNumber = 10;
        timeMagnitudeLineChart.xAxises[0].boundaryGap = true;
        timeMagnitudeLineChart.RemoveData();
        timeMagnitudeLineChart.AddSerie(SerieType.Line, "Magnitude");

        for (int index = 0; index < xFrquencies.Count; ++index)
        {
            timeMagnitudeLineChart.AddXAxisData(xFrquencies[index].ToString());
            timeMagnitudeLineChart.AddData(0, yMagnitude[index]);
        }

        timePhaseLineChart.title.show = true;
        timePhaseLineChart.title.text = "Frequency(Hz)-Phase chart";
        timePhaseLineChart.tooltip.show = true;
        timePhaseLineChart.legend.show = false;
        timePhaseLineChart.xAxises[0].show = true;
        timePhaseLineChart.xAxises[1].show = false;
        timePhaseLineChart.yAxises[0].show = true;
        timePhaseLineChart.yAxises[1].show = false;
        timePhaseLineChart.xAxises[0].type = Axis.AxisType.Category;
        timePhaseLineChart.yAxises[0].type = Axis.AxisType.Value;
        timePhaseLineChart.xAxises[0].splitNumber = 10;
        timePhaseLineChart.xAxises[0].boundaryGap = true;
        timePhaseLineChart.RemoveData();
        timePhaseLineChart.AddSerie(SerieType.Line, "Phase");

        for (int index = 0; index < xFrquencies.Count; ++index)
        {
            timePhaseLineChart.AddXAxisData(xFrquencies[index].ToString());
            timePhaseLineChart.AddData(0, yPhase[index]);
        }
    }
}
