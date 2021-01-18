using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaysDrawer
{
    private readonly Dictionary<int, List<AcousticRay>> m_rays;
    private readonly LineRenderer[] m_lines;

    public RaysDrawer(LineRenderer[] linesToDraw, Dictionary<int, List<AcousticRay>> rays)
    {
        m_lines = linesToDraw;
        this.m_rays = rays;
    }

    private void ResetLines()
    {
        foreach (var line in m_lines)
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
        
        m_lines[numberOfLine].SetPosition(0,
            VectorConverter.Convert(m_rays[numberOfMicro][numberOfLine].source));

        var numberOfPoints = 1;
        var size = m_rays[numberOfMicro][numberOfLine].collisionPoints.Count;
        for (int indexPosition = 0; indexPosition < size; ++indexPosition)
        {
            m_lines[numberOfLine].positionCount = ++numberOfPoints;
            m_lines[numberOfLine].SetPosition(numberOfPoints - 1,
                VectorConverter.Convert(m_rays[numberOfMicro][numberOfLine].collisionPoints[indexPosition]));
        }

        m_lines[numberOfLine].positionCount = ++numberOfPoints;
        m_lines[numberOfLine].SetPosition(numberOfPoints - 1,
            VectorConverter.Convert(m_rays[numberOfMicro][numberOfLine].microphonePosition));
    }
}
