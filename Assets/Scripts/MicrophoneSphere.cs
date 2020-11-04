using System;
using System.Numerics;

public class MicrophoneSphere
{
    public int id
    {
        get;
        private set;
    }

    public Vector3 center
    {
        get;
        set;
    }

    public float radius
    {
        get;
        set;
    }

    private static int m_id = -1;

    ~MicrophoneSphere()
    {
        --m_id;
    }

    public MicrophoneSphere(Vector3 center, float radius)
    {
        GenerateId();
        this.center = center;
        this.radius = radius;
    }

    public bool LineIntersectionWithSphere(Vector3 begin, Vector3 end)
    {
        var a = (float)(Math.Pow(end.X - begin.X, 2) +
                        Math.Pow(end.Y - begin.Y, 2) +
                        Math.Pow(end.Z - begin.Z, 2));

        var b = -2 * ((end.X - begin.X) * (center.X - begin.X) +
                      (end.Y - begin.Y) * (center.Y - begin.Y) +
                      (end.Z - begin.Z) * (center.Z - begin.Z));

        var c = (float)(Math.Pow(center.X - begin.X, 2) +
                        Math.Pow(center.Y - begin.Y, 2) +
                        Math.Pow(center.Z - begin.Z, 2) -
                        Math.Pow(radius, 2));

        return Math.Pow(b, 2) - 4 * a * c >= 0;
    }

    private void GenerateId()
    {
        id = ++m_id;
    }

}
