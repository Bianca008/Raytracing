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
        //timeMagnitudeChart = ChartArea.GetComponent<BarChart>();
        //timePhaseChart = ChartArea.GetComponent<BarChart>();

        //if (timeMagnitudeChart == null)
        //{
        //    timeMagnitudeChart = ChartArea.AddComponent<BarChart>();
        //}

        //if(timePhaseChart == null)
        //{
        //    timePhaseChart = ChartArea.AddComponent<BarChart>();
        //}
        
        timeMagnitudeChart.title.show = true;
        timeMagnitudeChart.title.text = "Time-Magnitude chart";
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
        timeMagnitudeChart.AddSerie(SerieType.Bar);

        for (int index = 0; index < xTime.Count; ++index)
        {
            timeMagnitudeChart.AddXAxisData(xTime[index].ToString());
            timeMagnitudeChart.AddData(0, yMagnitude[index]);
        }

        timePhaseChart.title.show = true;
        timePhaseChart.title.text = "Time-Phase chart";
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
        timePhaseChart.AddSerie(SerieType.Bar);

        for (int index = 0; index < xTime.Count; ++index)
        {
            timePhaseChart.AddXAxisData(xTime[index].ToString());
            timePhaseChart.AddData(0, yPhase[index]);
        }
    }
}
