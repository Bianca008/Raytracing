public class AcousticMaterial 
{
    public float AbsorbtionCoefficient
    {
        get;
        set;
    }

    public AcousticMaterial()
    {

    }

    public AcousticMaterial(float absorbtionCoeffcient)
    {
        AbsorbtionCoefficient = absorbtionCoeffcient;
    }
}
