using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaysDrawer
{
    private readonly Dictionary<int, List<AcousticRay>> m_Rays;
    private readonly LineRenderer[] m_Lines;

    public RaysDrawer(LineRenderer[] linesToDraw, Dictionary<int, List<AcousticRay>> rays)
    {
        m_Lines = linesToDraw;
        this.m_Rays = rays;
    }

    private void ResetLines()
    {
        foreach (var line in m_Lines)
        {
            line.positionCount = 1;
        }
    }

    //public void Draw()
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
        
        m_Lines[numberOfLine].SetPosition(0,
            VectorConverter.Convert(m_Rays[numberOfMicro][numberOfLine].source));

        var numberOfPoints = 1;
        var size = m_Rays[numberOfMicro][numberOfLine].collisionPoints.Count;
        for (int indexPosition = 0; indexPosition < size; ++indexPosition)
        {
            m_Lines[numberOfLine].positionCount = ++numberOfPoints;
            m_Lines[numberOfLine].SetPosition(numberOfPoints - 1,
                VectorConverter.Convert(m_Rays[numberOfMicro][numberOfLine].collisionPoints[indexPosition]));
        }

        m_Lines[numberOfLine].positionCount = ++numberOfPoints;
        m_Lines[numberOfLine].SetPosition(numberOfPoints - 1,
            VectorConverter.Convert(m_Rays[numberOfMicro][numberOfLine].microphonePosition));
    }
}
