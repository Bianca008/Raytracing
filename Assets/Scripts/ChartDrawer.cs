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
        get => chart.enabled;
        set => chart.enabled = value;
    }

    private BarChart chart;

    public ChartDrawer(GameObject canvas)
    {
        ChartArea = canvas;
    }

    public void Draw(List<float> xTime, List<float> yPressure)
    {
        chart = ChartArea.GetComponent<BarChart>();

        if (chart == null)
        {
            chart = ChartArea.AddComponent<BarChart>();
        }

        chart.title.show = true;
        chart.title.text = "Time-Pressure chart";
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
        chart.AddSerie(SerieType.Bar);

        for (int index = 0; index < xTime.Count; ++index)
        {
            chart.AddXAxisData(xTime[index].ToString());
            chart.AddData(0, yPressure[index]);
        }
    }
}
