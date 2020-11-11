using UnityEngine.UI;

public class UiFrequencyEchogram
{
    public InputField microphoneInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiFrequencyEchogram(InputField microphoneIf, Button showBtn)
    {
        showButton = showBtn;
        microphoneInputField = microphoneIf;
    }
}