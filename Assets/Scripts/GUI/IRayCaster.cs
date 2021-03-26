using System.Collections.Generic;
using UnityEngine;

public class RayCaster
{
    private readonly int maxDistance;

    public System.Numerics.Vector3 Position
    {
        get;
        set;
    }

    public System.Numerics.Vector3 Direction
    {
        get;
        set;
    }

    public float TotalDistance
    {
        get;
        set;
    }

    public int NumberOfReflections
    {
        get;
        set;
    }

    private GameObject AcousticMaterialsGameObject
    {
        get;
        set;
    }

    public RayCaster(int maxDist)
    {
        maxDistance = maxDist;
        AcousticMaterialsGameObject = new GameObject("AcousticMaterials");
    }

    public void RayCast(List<AcousticRay> rays,
        int numberOfRay)
    {
        var ray = new Ray(VectorConverter.Convert(Position), VectorConverter.Convert(Direction));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            var acousticMaterial = hit.transform.GetComponent<AcousticMaterial>();
            Direction = VectorConverter.Convert(UnityEngine.Vector3.Reflect(VectorConverter.Convert(Direction), hit.normal));
            Position = VectorConverter.Convert(hit.point);
            ++NumberOfReflections;
            TotalDistance += hit.distance;
            rays[numberOfRay].CollisionPoints.Add(VectorConverter.Convert(hit.point));

            if (acousticMaterial != null)
                rays[numberOfRay].AcousticMaterials.Add(acousticMaterial);
            else
            {
                var gameObject = new GameObject("AcousticMaterial");
                rays[numberOfRay].AcousticMaterials.Add(gameObject.AddComponent<AcousticMaterial>());
                gameObject.transform.parent = AcousticMaterialsGameObject.transform;
            }
        }
        else
        {
            Position += Direction * maxDistance;
            TotalDistance += maxDistance;
        }
    }
}
