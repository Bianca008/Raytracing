using System;

public class PressureConverter 
{
    public static double ConvertIntensityToPressure(double intensity,
        double massDensity = 1.2041,
        double airSoundSpeed = 343.21)
    {
        return Math.Sqrt(intensity*massDensity*airSoundSpeed);
    } 
}
