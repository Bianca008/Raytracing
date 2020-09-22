using System;
using System.Collections.Generic;
//using UnityEngine;

public class EnergyCalculator
{
    private double CurrentEnergy
    {
        get;
        set;
    }

    private int numberOfRays;
    private double startEnergy;

    public EnergyCalculator(int nrOfRays, double startE)
    {
        numberOfRays = nrOfRays;
        startEnergy = startE;
    }

    private double CalculateEnergyInPoint(System.Numerics.Vector3 position,
        double distance,
        double absorptionCoefficient)
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

        const double airAbsorption = 0.0013;
        double energy = startEnergy * Math.Exp(-airAbsorption * distance) * (1 - absorptionCoefficient);

        return energy;
    }

    private double GetDistance(System.Numerics.Vector3 start, System.Numerics.Vector3 end)
    {
        return Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2) + Math.Pow(start.Z - end.Z, 2));
    }

    public void CalculateEnergy(List<AcousticRay> rays)
    {
        for (int indexRay = 0; indexRay < rays.Count; ++indexRay)
        {
            CurrentEnergy = startEnergy / numberOfRays;
            double distance = 0;
            for (int indexPoint = 1; indexPoint < rays[indexRay].ColissionPoints.Count; ++indexPoint)
            {
                distance += GetDistance(rays[indexRay].ColissionPoints[indexPoint - 1],
                    rays[indexRay].ColissionPoints[indexPoint]);
                double energy = CalculateEnergyInPoint(rays[indexRay].ColissionPoints[indexPoint],
                    distance,
                    rays[indexRay].AcousticMaterials[indexPoint - 1].AbsorbtionCoefficient);
                // Debug.Log(energy + " " + indexRay + "  " + indexPoint);
            }
        }
    }
}
