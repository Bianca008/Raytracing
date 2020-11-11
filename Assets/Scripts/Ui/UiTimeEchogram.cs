using UnityEngine.UI;
public class UiTimeEchogram
{
    public InputField microphoneInputField { get; private set; }

    public InputField frequencyInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiTimeEchogram(
        InputField microphoneIf,
        InputField frequencyIf,
        Button showBtn)
    {
        this.showButton = showBtn;
        this.microphoneInputField = microphoneIf;
        this.frequencyInputField = frequencyIf;
    }
}
