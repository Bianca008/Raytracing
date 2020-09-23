using UnityEngine;

public class LinesCreator
{
    public static LineRenderer[] GenerateLines(int numberOfRays, Transform source, Material material)
    {
        LineRenderer[] lines = new LineRenderer[numberOfRays];

        for (int index = 0; index < numberOfRays; ++index)
            lines[index] = SetLineProperties(source, material);

        return lines;
    }
    private static LineRenderer SetLineProperties(Transform source, Material material)
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.positionCount = 1;
        line.transform.SetParent(source);
        if (material != null)
            line.material = material;

        return line;
    }
}
