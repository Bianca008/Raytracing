using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FirstRay : MonoBehaviour
{
    public double startEnergy = 10;
    public double absorptionCoefficient = 0.2;
    public double minimumEnergy = 5;
    private float maxStepDistance = 200;
    private int numberOfColissions = 3;
    private int maxDistance = 200;
    private readonly int numberOfRays = 18;

    private LineRenderer[] lines;

    private void Start()
    {
        lines = new LineRenderer[numberOfRays];

        for (int index = 0; index < numberOfRays; ++index)
            lines[index] = SetLineProperties();
    }

    private double CurrentEnergy
    {
        get;
        set;
    }

    private double CalculateEnergyInPoint(Vector3 position, double distance)
    {
        /*
         E = Ef/ Nr D_theta_phi e^-gama_d prod(1-alfa_i)

        Ef - total energy emitted by the sound
        Nr - number of rays
        D_theta_phi - source directively; used D_theta_phi = 1
        gama -air absorbtion
        d - distance path traveld by the ray
        alfa_i - absorbtion coefficient at surface i
         */

        double airAbsorption = 0.0013;
        double energy = startEnergy * Math.Exp(-airAbsorption * distance) * (1 - absorptionCoefficient);

        return energy;
    }

    private void Update()
    {
        float angle = 0;
        float angleStep = 360 / numberOfRays;

        for (int index = 0; index < numberOfRays; ++index)
        {
            DrawPredictedReflectionPattern(transform.position, Quaternion.AngleAxis(angle, transform.up) * transform.forward, index);
            angle += angleStep;
        }
        //DrawPredictedReflectionPattern(transform.position, transform.forward, 0);
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int numberOfRay)
    {
        lines[numberOfRay].SetPosition(0, transform.position);
        int numberOfPoints = 1;

        Vector3 startingPosition = position;
        double totalDistance = 0;

        CurrentEnergy = startEnergy;
        int numberOfReflections = 0;

        while (CurrentEnergy >= minimumEnergy &&
            totalDistance <= maxDistance && 
            numberOfReflections < numberOfColissions)
        {
            /*Raycast to detect reflection */
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxStepDistance))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
                ++numberOfReflections;
                totalDistance += hit.distance;
                lines[numberOfRay].positionCount = ++numberOfPoints;
                lines[numberOfRay].SetPosition(numberOfPoints-1, hit.point);
            }
            else
            {
                position += direction * maxStepDistance;
                totalDistance += maxStepDistance;
            }
            startingPosition = position;

            /*Recalculate current energy of the ray.*/
            CurrentEnergy = CalculateEnergyInPoint(position, totalDistance);
            Debug.Log(CurrentEnergy + " " + totalDistance + " " + numberOfReflections + " ");
        }
    }

    private LineRenderer SetLineProperties()
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        //line = this.GetComponent<LineRenderer>();
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.positionCount = 1;
        line.transform.SetParent(transform);

        return line;
    }
}
