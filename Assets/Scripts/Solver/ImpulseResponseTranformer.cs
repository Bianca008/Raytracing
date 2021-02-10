using System.Collections.Generic;
using System.Numerics;
using NWaves.Signals;
using NWaves.Transforms;

public class ImpulseResponseTranformer
{
    public static DiscreteSignal Transform(
        List<Complex> frequencyResponse)
    {
        var re = new List<float>();
        var im = new List<float>();

        foreach (var response in frequencyResponse)
        {
            re.Add((float)response.Real);
            im.Add((float)response.Imaginary);
        }

        if (re.Count == 0 && im.Count == 0)
            return new DiscreteSignal(22050, new float[] { 0, 0 });

        var fft = new RealFft(frequencyResponse.Count);
        var outputArray = new float[re.Count];
        fft.Inverse(re.ToArray(), im.ToArray(), outputArray);

        var impulseResponse = new DiscreteSignal(22050, outputArray);

        return impulseResponse;
    }
}
