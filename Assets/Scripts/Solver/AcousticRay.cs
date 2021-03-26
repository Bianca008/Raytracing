using System.Collections.Generic;
using System.Linq;
using Vector3 = System.Numerics.Vector3;

public class AcousticRay
{
    public Vector3 Source
    {
        get;
        set;
    }

    public Vector3 MicrophonePosition
    {
        get;
        set;
    }

    public List<Vector3> CollisionPoints { get; set; } = new List<Vector3>();

    public List<AcousticMaterial> AcousticMaterials { get; set; } = new List<AcousticMaterial>();

    public List<double> Distances { get; set; } = new List<double>();

    public AcousticRay(Vector3 source, Vector3 microphone)
    {
        this.Source = source;
        MicrophonePosition = microphone;
    }

    public AcousticRay TruncateRay(int position, Vector3 microphonePos)
    {
        if (position > CollisionPoints.Count || position > AcousticMaterials.Count)
            return this;

        var newRay = new AcousticRay(Source, microphonePos)
        {
            /*index, number of elements to copy*/
            CollisionPoints = CollisionPoints.GetRange(0, position),
            AcousticMaterials = AcousticMaterials.GetRange(0, position)
        };

        return newRay;
    }

    public float GetDistance()
    {
        if (CollisionPoints.Count == 0)
            return Vector3.Distance(Source, MicrophonePosition);

        var distance = Vector3.Distance(Source,
            CollisionPoints[0]);

        distance += CollisionPoints.Select((vec, index) =>
            index == CollisionPoints.Count - 1 ? 0 : Vector3.Distance(vec, CollisionPoints[index + 1])).Sum();

        distance += Vector3.Distance(CollisionPoints[CollisionPoints.Count - 1], MicrophonePosition);

        return distance;
    }
}
