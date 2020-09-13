using System.Collections.Generic;
using UnityEngine;

public class RaysDrawer
{
    private List<List<System.Numerics.Vector3>> linePositions;
    public LineRenderer[] lines;

    public RaysDrawer(LineRenderer[] linesToDraw, List<List<System.Numerics.Vector3>> linePos)
    {
        lines = linesToDraw;
        linePositions = linePos;
    }

    public void Draw()
    {
        /* Set initial position.*/
        for (int indexLine = 0; indexLine < lines.Length; ++indexLine)
        {
            lines[indexLine].SetPosition(0, VectorConverter.Convert(linePositions[indexLine][0]));
        }

        /* Add position for each colission.*/
        for (int indexLine = 0; indexLine < linePositions.Count; ++indexLine)
        {
            int numberOfPoints = 1;
            for (int indexPosition = 1; indexPosition < linePositions[indexLine].Count; ++indexPosition)
            {
                lines[indexLine].positionCount = ++numberOfPoints;
                lines[indexLine].SetPosition(numberOfPoints - 1, VectorConverter.Convert(linePositions[indexLine][indexPosition]));
            }
        }
    }
}
