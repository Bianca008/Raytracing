using UnityEngine.UI;

public class FrequencyEchogram
{
    public InputField microphoneInputField { get; private set; }

    public Button showButton { get; private set; }

    public FrequencyEchogram(InputField microphoneIf, Button showBtn)
    {
        showButton = showBtn;
        microphoneInputField = microphoneIf;
    }
}