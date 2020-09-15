using UnityEngine;

public class RayGenerator : MonoBehaviour
{
    public double StartEnergy = 25;
    private int numberOfColissions = 3;
    private readonly int maxDistance = 200;
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
        rayDrawer = new RaysDrawer(lines, rayGeometryGenerator.LinePositions);
        energyCalculator = new EnergyCalculator(numberOfRays, StartEnergy);
    }

    private void Update()
    {
        rayDrawer.Draw();
        energyCalculator.CalculateEnergy(rayGeometryGenerator.LinePositions, rayGeometryGenerator.AcousticMaterials);
    }
}
