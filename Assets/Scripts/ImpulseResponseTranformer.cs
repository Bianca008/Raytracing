using System.Collections.Generic;
using System.Linq;
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

        var fft = new RealFft(frequencyResponse.Count);
        var outputArray = new float[re.Count];
        fft.Inverse(re.ToArray(), im.ToArray(), outputArray);
        var maxi = outputArray.Max();

        //for (int index = 0; index < outputArray.Length; ++index)
        //    outputArray[index] /= maxi;

        //for (int index = 0; index < outputArray.Length; ++index)
        //    outputArray[index] *= 0.6f;

        var impulseResponse = new DiscreteSignal(22050, outputArray);

        return impulseResponse;
    }
}
