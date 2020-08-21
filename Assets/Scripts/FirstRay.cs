using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRay : MonoBehaviour
{
    private Transform m_transform;
    private LineRenderer m_lineRenderer;

    private Ray m_ray;
    private RaycastHit m_hit;
    private Vector3 m_inDirection;
    public int m_numberOfReflections = 5;
    private int m_numberOfPoints;

    void Awake()
    {
        m_transform = this.GetComponent<Transform>();
        m_lineRenderer = this.GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.05f;
        m_lineRenderer.endWidth = 0.05f;
        m_lineRenderer.material.color = Color.blue;
    }

    void Update()
    {
        m_numberOfReflections = Mathf.Clamp(m_numberOfReflections, 1, m_numberOfReflections);
        m_ray = new Ray(m_transform.position, m_transform.forward);
        m_numberOfPoints = m_numberOfReflections;
        m_lineRenderer.positionCount = m_numberOfPoints;
        m_lineRenderer.SetPosition(0, m_transform.position);

        for (int index = 0; index <= m_numberOfReflections; index++)
        {
            //If the ray hasn't reflected yet  
            if (index == 0)
            {
                //Check if the ray has hit something  
                if (Physics.Raycast(m_ray.origin, m_ray.direction, out m_hit, 100)) 
                {
                    m_inDirection = Vector3.Reflect(m_ray.direction, m_hit.normal);
                    m_ray = new Ray(m_hit.point, m_inDirection);

                    if (m_numberOfReflections == 1)
                    {
                        m_lineRenderer.positionCount = ++m_numberOfPoints;
                    }
                    //set the position of the next vertex at the line renderer to be the same as the hit point  
                    m_lineRenderer.SetPosition(index + 1, m_hit.point);
                }
            }
            else  
            {
                if (Physics.Raycast(m_ray.origin, m_ray.direction, out m_hit, 100))
                {
                    m_inDirection = Vector3.Reflect(m_inDirection, m_hit.normal);
                    m_ray = new Ray(m_hit.point, m_inDirection);
                    m_lineRenderer.positionCount = ++m_numberOfPoints;

                    m_lineRenderer.SetPosition(index + 1, m_hit.point);
                }
            }
        }
    }
}
