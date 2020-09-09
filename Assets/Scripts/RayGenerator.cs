using System;
using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public double startEnergy = 10;
    public double absorptionCoefficient = 0.2;
    public double minimumEnergy = 5;

    private float maxStepDistance = 200;
    private int numberOfColissions = 3;
    private int maxDistance = 200;
    private readonly int numberOfRays = 1;

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

    public Tuple<double, double, double> GetPolarCoordinates(Vector3 position)
    {
        double distanceFromOrigin = Math.Sqrt(Math.Pow(position.x, 2) +
                                    Math.Pow(position.y, 2) +
                                    Math.Pow(position.z, 2));
        double theta = Math.Atan(position.y / position.x);
        double phi = Math.Acos(position.z / distanceFromOrigin);

        /*This transformation is in radians.*/
        return Tuple.Create(distanceFromOrigin, theta, phi);
    }

    public Vector3 GetCartesianCoordinates(double distance, double theta, double phi)
    {
        /*Sin and Cos are using radians.*/
        double x = distance * Math.Sin(phi) * Math.Cos(theta);
        double y = distance * Math.Sin(phi) * Math.Sin(theta);
        double z = distance * Math.Cos(phi);

        return new Vector3((float)x, (float)y, (float)z);
    }

    private void GenerateRays()
    {
        //!!!Var3 - Fibo Sphere!!!
        double goldenRatio = (1 + Math.Sqrt(5)) / 2;
        double goldenAngle = (2 - goldenRatio) * (2 * Math.PI);
        int indexRay = 0;

        for (int index = 0; index < numberOfRays; ++index)
        {
            double theta = Math.Asin(-1 + 2 * index / (numberOfRays + 1));
            double phi = goldenAngle * index;

            Debug.Log(indexRay + " " + GetCartesianCoordinates(1, theta, phi));
            DrawPredictedReflectionPattern(transform.position,
               GetCartesianCoordinates(1, theta, phi),
               indexRay);
            ++indexRay;
        }
    }

    private void MoveSource()
    {
        float speed = 2;
        Vector3 pos = transform.position;

        if (Input.GetKey("w"))
            pos.z += speed * Time.deltaTime;
        if (Input.GetKey("s"))
            pos.z -= speed * Time.deltaTime;
        if (Input.GetKey("d"))
            pos.x += speed * Time.deltaTime;
        if (Input.GetKey("a"))
            pos.x -= speed * Time.deltaTime;

        transform.position = pos;
    }

    private void Update()
    {
        GenerateRays();
        MoveSource();
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int numberOfRay)
    {
        lines[numberOfRay].SetPosition(0, transform.position);
        int numberOfPoints = 1;

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
                lines[numberOfRay].SetPosition(numberOfPoints - 1, hit.point);
            }
            else
            {
                position += direction * maxStepDistance;
                totalDistance += maxStepDistance;
            }

            /*Recalculate current energy of the ray.*/
            CurrentEnergy = CalculateEnergyInPoint(position, totalDistance);
        }
    }

    private LineRenderer SetLineProperties()
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.positionCount = 1;
        line.transform.SetParent(transform);

        return line;
    }
}
