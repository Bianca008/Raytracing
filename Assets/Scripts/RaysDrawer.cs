using System.Collections.Generic;
using UnityEngine;

public class RaysDrawer
{
    private readonly List<AcousticRay> rays;
    private readonly LineRenderer[] lines;

    public RaysDrawer(LineRenderer[] linesToDraw, List<AcousticRay> rays)
    {
        lines = linesToDraw;
        this.rays = rays;
    }

    public void Draw()
    {
        /* Set initial position.*/
        for (int indexLine = 0; indexLine < lines.Length; ++indexLine)
        {
            lines[indexLine].SetPosition(0, 
                VectorConverter.Convert(rays[indexLine].ColissionPoints[0]));
        }

        /* Add position for each colission.*/
        for (int indexLine = 0; indexLine < rays.Count; ++indexLine)
        {
            int numberOfPoints = 1;
            for (int indexPosition = 1; indexPosition < rays[indexLine].ColissionPoints.Count; ++indexPosition)
            {
                lines[indexLine].positionCount = ++numberOfPoints;
                lines[indexLine].SetPosition(numberOfPoints - 1, VectorConverter.Convert(rays[indexLine].ColissionPoints[indexPosition]));
            }
        }
    }
}
