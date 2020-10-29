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
        Dictionary<int, List<Complex>> frequencyReponse,
        List<MicrophoneSphere> microphones)
    {
        for (int indexMicro = 0; indexMicro < microphones.Count; ++indexMicro)
        {
            DiscreteSignal attention;

            // load
            using (var stream = new FileStream(audioSource.clip.name + ".wav", FileMode.Open))
            {
                var waveFile = new WaveFile(stream);
                attention = waveFile[Channels.Left];
            }

            Debug.Log(frequencyReponse[microphones[indexMicro].Id].Count);
            NWaves.Transforms.RealFft value = new NWaves.Transforms.RealFft(frequencyReponse[0].Count);

            List<float> re = new List<float>();
            List<float> im = new List<float>();

            foreach (Complex response in frequencyReponse[0])
            {
                re.Add((float)response.Real);
                im.Add((float)response.Imaginary);
            }

            float[] outputArray = new float[re.Count];
            value.Inverse(re.ToArray(), im.ToArray(), outputArray);
            float maxi = outputArray.Max();

            for (int index = 0; index < outputArray.Length; ++index)
                outputArray[index] /= maxi;

            for (int index = 0; index < outputArray.Length; ++index)
                outputArray[index] *= 0.5f;

            DiscreteSignal impulseRespone = new DiscreteSignal(22050, outputArray);
            DiscreteSignal convolutionResult = Operation.Convolve(attention, impulseRespone);

            // save
            using (var stream = new FileStream("results/convolutionAttention" +
                microphones[indexMicro].Id.ToString() +
                ".wav", FileMode.Create))
            {
                var waveFile = new WaveFile(convolutionResult);
                waveFile.SaveTo(stream);
            }
        }
    }
}
