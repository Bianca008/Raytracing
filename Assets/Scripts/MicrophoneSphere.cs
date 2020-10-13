using System;
using System.Numerics;

public class MicrophoneSphere
{
    public int Id
    {
        get;
        set;
    }

    public Vector3 Center
    {
        get;
        set;
    }

    public float Radius
    {
        get;
        set;
    }

    private static int id = -1;

    public MicrophoneSphere(Vector3 center, float radius)
    {
        GenerateId();
        Center = center;
        Radius = radius;
    }

    public bool LineIntersectionWithSphere(Vector3 begin, Vector3 end)
    {
        float a = (float)(Math.Pow(end.X - begin.X, 2) +
                          Math.Pow(end.Y - begin.Y, 2) +
                          Math.Pow(end.Z - begin.Z, 2));

        float b = -2 * ((end.X - begin.X) * (Center.X - begin.X) +
                        (end.Y - begin.Y) * (Center.Y - begin.Y) +
                        (end.Z - begin.Z) * (Center.Z - begin.Z));

        float c = (float)(Math.Pow(Center.X - begin.X, 2) +
                           Math.Pow(Center.Y - begin.Y, 2) +
                           Math.Pow(Center.Z - begin.Z, 2) -
                           Math.Pow(Radius, 2));

        if (Math.Pow(b, 2) - 4 * a * c >= 0)
            return true;

        return false;
    }

    private void GenerateId()
    {
        Id = ++id;
    }

}
