using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaysDrawer
{
    public Dictionary<int, List<AcousticRay>> Rays { get; set; }

    public List<LineRenderer> Lines { get; set; }

    private GameObject go { get; set; }

    public RaysDrawer()
    {
        Lines = new List<LineRenderer>();
        Rays = new Dictionary<int, List<AcousticRay>>();
        go = new GameObject("Rays");
    }

    private void ResetLines()
    {
        GameObject.Destroy(go);
        go = new GameObject("Rays");
    }

    public void Draw(Material material, int microphoneNumber, int lineIndex, bool all = false)
    {
        if (all == false)
            ResetLines();

        if (Rays.ContainsKey(microphoneNumber) == false) return;
        if (lineIndex >= Rays[microphoneNumber].Count) return;

        var go = new GameObject("[m: " + microphoneNumber + ", l: " + lineIndex + "]");
        var renderedLine = go.AddComponent<LineRenderer>();
        go.transform.parent = this.go.transform;

        var currentRay = Rays[microphoneNumber][lineIndex];
        var hitPoints = new List<Vector3>();
        hitPoints.Add(VectorConverter.Convert(currentRay.Source));
        hitPoints.AddRange(currentRay.CollisionPoints.Select(VectorConverter.Convert));
        hitPoints.Add(VectorConverter.Convert(currentRay.MicrophonePosition));

        renderedLine.positionCount = hitPoints.Count;
        renderedLine.SetPositions(hitPoints.ToArray());

        renderedLine.startWidth = 0.01f;
        renderedLine.endWidth = 0.01f;
        if (material != null)
            renderedLine.material = material;
        Lines.Add(renderedLine);
    }

    public void DrawAll(Material material)
    {
        foreach (var item in Rays)
            for (int index = 0; index < item.Value.Count; ++index)
                Draw(material, item.Key, index, true);
    }
}
