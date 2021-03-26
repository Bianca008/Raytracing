using System;
using System.Numerics;

public class MicrophoneSphere
{
    public int Id
    {
        get;
        private set;
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

    private static int _id = -1;

    ~MicrophoneSphere()
    {
        --_id;
    }

    public MicrophoneSphere(Vector3 center, float radius)
    {
        GenerateId();
        this.Center = center;
        this.Radius = radius;
    }

    public bool LineIntersectionWithSphere(Vector3 begin, Vector3 end)
    {
        var a = (float)(Math.Pow(end.X - begin.X, 2) +
                        Math.Pow(end.Y - begin.Y, 2) +
                        Math.Pow(end.Z - begin.Z, 2));

        var b = -2 * ((end.X - begin.X) * (Center.X - begin.X) +
                      (end.Y - begin.Y) * (Center.Y - begin.Y) +
                      (end.Z - begin.Z) * (Center.Z - begin.Z));

        var c = (float)(Math.Pow(Center.X - begin.X, 2) +
                        Math.Pow(Center.Y - begin.Y, 2) +
                        Math.Pow(Center.Z - begin.Z, 2) -
                        Math.Pow(Radius, 2));

        return Math.Pow(b, 2) - 4 * a * c >= 0;
    }

    public bool IsAroundMicro(Vector3 position)
    {
        return !(Vector3.Distance(Center, position) > Radius);
    }

    private void GenerateId()
    {
        Id = ++_id;
    }

}
