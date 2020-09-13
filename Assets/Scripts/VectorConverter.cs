using UnityEngine;
using System.Numerics;

public class VectorConverter
{
    public static System.Numerics.Vector3 Convert(UnityEngine.Vector3 vector)
    {
        return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
    }

    public static UnityEngine.Vector3 Convert(System.Numerics.Vector3 vector)
    {
        return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
    }
}
