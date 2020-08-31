using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FirstRay : MonoBehaviour
{
    public float maxStepDistance = 200;

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0,
            transform.position + transform.forward * 0.25f,
            transform.rotation,
            0.5f,
            EventType.Repaint);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        DrawPredictedReflectionPattern(transform.position + transform.forward * 0.75f, transform.forward);
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction)
    {
        Vector3 startingPosition = position;

        for (int index = 0; index <= 5; ++index)
        {
            /*Raycast to detect reflection */
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxStepDistance))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
            }
            else
            {
                position += direction * maxStepDistance;
            }

            Gizmos.color = Color.blue;
            /*Draw between last and actual position.*/
            Gizmos.DrawLine(startingPosition, position);

            startingPosition = position;
        }
    }
}
