using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class SoundConvolver 
{
    public static void ConvolveSound(
        AudioSource audioSource,
        Dictionary<int, List<Complex>> frequencyResponse,
        List<MicrophoneSphere> microphones)
    {
        foreach (var microphone in microphones)
        {
            DiscreteSignal discreteSignal;

            // load
            using (var stream = new FileStream(audioSource.clip.name + ".wav", FileMode.Open))
            {
                var waveFile = new WaveFile(stream);
                discreteSignal = waveFile[Channels.Left];
            }

            Debug.Log(frequencyResponse[microphone.id].Count);
            var value = new NWaves.Transforms.RealFft(frequencyResponse[0].Count);

            var re = new List<float>();
            var im = new List<float>();

            foreach (var response in frequencyResponse[0])
            {
                re.Add((float)response.Real);
                im.Add((float)response.Imaginary);
            }

            var outputArray = new float[re.Count];
            value.Inverse(re.ToArray(), im.ToArray(), outputArray);
            var maxi = outputArray.Max();

            for (int index = 0; index < outputArray.Length; ++index)
                outputArray[index] /= maxi;

            for (int index = 0; index < outputArray.Length; ++index)
                outputArray[index] *= 0.5f;

            var impulseResponse = new DiscreteSignal(22050, outputArray);
            var convolutionResult = Operation.Convolve(discreteSignal, impulseResponse);

            // save
            using (var stream = new FileStream("results/convolutionAttention" +
                                               microphone.id.ToString() +
                                               ".wav", FileMode.Create))
            {
                var waveFile = new WaveFile(convolutionResult);
                waveFile.SaveTo(stream);
            }
        }
    }
}
