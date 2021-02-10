using System.Collections.Generic;
using UnityEngine;

public class IRayCaster
{
    private readonly int m_MaxDistance;

    public System.Numerics.Vector3 position
    {
        get;
        set;
    }

    public System.Numerics.Vector3 direction
    {
        get;
        set;
    }

    public float totalDistance
    {
        get;
        set;
    }

    public int numberOfReflections
    {
        get;
        set;
    }

    private GameObject m_acousticMaterialsGameObject
    {
        get;
        set;
    }

    public IRayCaster(int maxDist)
    {
        m_MaxDistance = maxDist;
        m_acousticMaterialsGameObject = new GameObject("AcousticMaterials");
    }

    public void RayCast(List<AcousticRay> rays,
        int numberOfRay)
    {
        var ray = new Ray(VectorConverter.Convert(position), VectorConverter.Convert(direction));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_MaxDistance))
        {
            var acousticMaterial = hit.transform.GetComponent<AcousticMaterial>();
            direction = VectorConverter.Convert(UnityEngine.Vector3.Reflect(VectorConverter.Convert(direction), hit.normal));
            position = VectorConverter.Convert(hit.point);
            ++numberOfReflections;
            totalDistance += hit.distance;
            rays[numberOfRay].collisionPoints.Add(VectorConverter.Convert(hit.point));

            if (acousticMaterial != null)
                rays[numberOfRay].acousticMaterials.Add(acousticMaterial);
            else
            {
                var gameObject = new GameObject("AcousticMaterial");
                rays[numberOfRay].acousticMaterials.Add(gameObject.AddComponent<AcousticMaterial>());
                gameObject.transform.parent = m_acousticMaterialsGameObject.transform;
            }
        }
        else
        {
            position += direction * m_MaxDistance;
            totalDistance += m_MaxDistance;
        }
    }
}
