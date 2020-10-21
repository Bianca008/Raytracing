using System;
using System.Numerics;

public class DegreesRadiansConverter 
{
    public static Vector3 TransformToPolarCoordinates(Vector3 position)
    {
        double distanceFromOrigin = Math.Sqrt(Math.Pow(position.X, 2) +
            Math.Pow(position.Y, 2) + 
            Math.Pow(position.Z, 2));

        double theta = Math.Atan(position.Y / position.X);
        double phi = Math.Acos(position.Z / distanceFromOrigin);

        return new Vector3((float)distanceFromOrigin, (float)theta, (float)phi);
    }

    public static Vector3 TransformToCartesianCoordinates(double distance, double theta, double phi)
    {
        /*Sin and Cos are using radians.*/
        double x = distance * Math.Sin(phi) * Math.Cos(theta);
        double y = distance * Math.Sin(phi) * Math.Sin(theta);
        double z = distance * Math.Cos(phi);

        return new Vector3((float)x, (float)y, (float)z);
    }
}
