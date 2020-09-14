using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public double startEnergy = 25;
    private int numberOfColissions = 3;
    private int maxDistance = 200;
    private readonly int numberOfRays = 3;

    private LineRenderer[] lines;
    private RayGeometry rayGeometryGenerator;
    private RaysDrawer rayDrawer;
    private EnergyCalculator energyCalculator;

    private void Start()
    {
        lines = LinesCreator.GenerateLines(numberOfRays, transform);
        rayGeometryGenerator = new RayGeometry(VectorConverter.Convert(transform.position),
            numberOfRays,
            numberOfColissions,
            maxDistance);
        rayGeometryGenerator.GenerateRays();
        rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.LinePosistions);
        energyCalculator = new EnergyCalculator(numberOfRays, startEnergy);
    }

    private void Update()
    {
        rayGeometryGenerator.GenerateRays();
        rayDrawer.Draw();
        energyCalculator.CalculateEnergy(rayGeometryGenerator.LinePosistions);
    }
}
