using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaysDrawer
{
    public Dictionary<int, List<AcousticRay>> rays { get; set; }
    public LineRenderer[] lines { get; set; }

    public RaysDrawer()
    {
        lines = new LineRenderer[0];
        rays = new Dictionary<int, List<AcousticRay>>();
    }

    private void ResetLines()
    {
        foreach (var line in lines)
        {
            line.positionCount = 1;
        }
    }

    public void Draw(int numberOfMicro, int numberOfLine)
    {
        if (rays.ContainsKey(numberOfMicro) == false) return;
        else
            if (numberOfLine >= rays[numberOfMicro].Count) return;

        ResetLines();

        lines[numberOfLine].SetPosition(0,
            VectorConverter.Convert(rays[numberOfMicro][numberOfLine].source));

        var numberOfPoints = 1;
        var size = rays[numberOfMicro][numberOfLine].collisionPoints.Count;
        for (int indexPosition = 0; indexPosition < size; ++indexPosition)
        {
            lines[numberOfLine].positionCount = ++numberOfPoints;
            lines[numberOfLine].SetPosition(numberOfPoints - 1,
                VectorConverter.Convert(rays[numberOfMicro][numberOfLine].collisionPoints[indexPosition]));
        }

        lines[numberOfLine].positionCount = ++numberOfPoints;
        lines[numberOfLine].SetPosition(numberOfPoints - 1,
            VectorConverter.Convert(rays[numberOfMicro][numberOfLine].microphonePosition));
    }
}
