using System.Collections.Generic;
using System.Data.Common;
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

    public AcousticRay TruncateRay(int position)
    {
        AcousticRay newRay = new AcousticRay(Source);
        //index, nr de elemente ce le vreau copiate
        newRay.ColissionPoints = ColissionPoints.GetRange(0, position + 1);
        newRay.AcousticMaterials = AcousticMaterials.GetRange(0, position + 1);

        return newRay;
    }
}
