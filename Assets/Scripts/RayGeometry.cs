using System;
using UnityEngine;

public class RayGeometry 
{
    private int numberOfRays;
    private int numberOfColissions;
    private int maxDistance;
    private Vector3 sourcePosition;
    private LineRenderer[] lines;

    public RayGeometry(Vector3 sourcePos, LineRenderer[] linesToDraw, int nrOfColissions = 3, int maxDist = 200)
    {
        sourcePosition = sourcePos;
        lines = linesToDraw;
        numberOfColissions = nrOfColissions;
        numberOfRays = lines.Length;
        maxDistance = maxDist;
    }

    public Vector3 GetCartesianCoordinates(double distance, double theta, double phi)
    {
        /*Sin and Cos are using radians.*/
        double x = distance * Math.Sin(phi) * Math.Cos(theta);
        double y = distance * Math.Sin(phi) * Math.Sin(theta);
        double z = distance * Math.Cos(phi);

        return new Vector3((float)x, (float)y, (float)z);
    }

    public void GenerateRays()
    {
        //!!!Var3 - Fibo Sphere!!!
        double goldenRatio = (1 + Math.Sqrt(5)) / 2;
        double goldenAngle = (2 - goldenRatio) * (2 * Math.PI);
        int indexRay = 0;

        for (int index = 0; index < numberOfRays; ++index)
        {
            double theta = Math.Asin(-1 + 2 * index / (numberOfRays + 1));
            double phi = goldenAngle * index;

            //Debug.Log(indexRay + " " + GetCartesianCoordinates(1, theta, phi));
            DrawPredictedReflectionPattern(sourcePosition,
               GetCartesianCoordinates(1, theta, phi),
               indexRay);
            ++indexRay;
        }
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int numberOfRay)
    {
        lines[numberOfRay].SetPosition(0, sourcePosition);
        int numberOfPoints = 1;

        double totalDistance = 0;
        int numberOfReflections = 0;

        while (totalDistance <= maxDistance &&
            numberOfReflections < numberOfColissions)
        {
            /*Raycast to detect reflection */
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
                ++numberOfReflections;
                totalDistance += hit.distance;
                lines[numberOfRay].positionCount = ++numberOfPoints;
                lines[numberOfRay].SetPosition(numberOfPoints - 1, hit.point);
            }
            else
            {
                position += direction * maxDistance;
                totalDistance += maxDistance;
            }
        }
    }
}
