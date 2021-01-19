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

    //public void DrawTimeChart()
    //{
    //    /* Set initial position.*/
    //    for (int indexLine = 0; indexLine < lines.Length; ++indexLine)
    //    {
    //        lines[indexLine].SetPosition(0, 
    //            VectorConverter.Convert(rays[indexLine].CollisionPoints[0]));
    //    }

    //    /* Add position for each colission.*/
    //    for (int indexLine = 0; indexLine < rays.Count; ++indexLine)
    //    {
    //        int numberOfPoints = 1;
    //        for (int indexPosition = 1; indexPosition < rays[indexLine].CollisionPoints.Count; ++indexPosition)
    //        {
    //            lines[indexLine].positionCount = ++numberOfPoints;
    //            lines[indexLine].SetPosition(numberOfPoints - 1, VectorConverter.Convert(rays[indexLine].CollisionPoints[indexPosition]));
    //        }
    //    }
    //}

    public void Draw(int numberOfMicro, int numberOfLine)
    {
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
