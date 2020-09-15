using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class AcousticRay 
{
    public Vector3 Source
    {
        get;
        set;
    }

    public List<Vector3> ColissionPoints
    {
        get;
        set;
    }

    public List<AcousticMaterial> AcousticMaterials
    {
        get;
        set;
    }

    public AcousticRay(Vector3 source)
    {
        Source = source;
        ColissionPoints = new List<Vector3>();
        ColissionPoints.Add(Source);
        AcousticMaterials = new List<AcousticMaterial>();
        GameObject gameObject = new GameObject();
        AcousticMaterials.Add(gameObject.AddComponent<AcousticMaterial>());
    }
}
