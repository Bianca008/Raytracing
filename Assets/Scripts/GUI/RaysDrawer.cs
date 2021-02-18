using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaysDrawer
{
    public Dictionary<int, List<AcousticRay>> rays { get; set; }

    public List<LineRenderer> lines { get; set; }

    private GameObject m_go { get; set; }

    public RaysDrawer()
    {
        lines = new List<LineRenderer>();
        rays = new Dictionary<int, List<AcousticRay>>();
        m_go = new GameObject("Rays");
    }

    private void ResetLines()
    {
        GameObject.Destroy(m_go);
        m_go = new GameObject("Rays");
    }

    public void Draw(Material material, int microphoneNumber, int lineIndex, bool all = false)
    {
        if (all == false)
            ResetLines();

        if (rays.ContainsKey(microphoneNumber) == false) return;
        if (lineIndex >= rays[microphoneNumber].Count) return;

        var go = new GameObject("[m: " + microphoneNumber + ", l: " + lineIndex + "]");
        var renderedLine = go.AddComponent<LineRenderer>();
        go.transform.parent = m_go.transform;

        var currentRay = rays[microphoneNumber][lineIndex];
        var hitPoints = new List<Vector3>();
        hitPoints.Add(VectorConverter.Convert(currentRay.source));
        hitPoints.AddRange(currentRay.collisionPoints.Select(VectorConverter.Convert));
        hitPoints.Add(VectorConverter.Convert(currentRay.microphonePosition));

        renderedLine.positionCount = hitPoints.Count;
        renderedLine.SetPositions(hitPoints.ToArray());

        renderedLine.startWidth = 0.01f;
        renderedLine.endWidth = 0.01f;
        if (material != null)
            renderedLine.material = material;
        lines.Add(renderedLine);
    }

    public void DrawAll(Material material)
    {
        foreach (var item in rays)
            for (int index = 0; index < item.Value.Count; ++index)
                Draw(material, item.Key, index, true);
    }
}
