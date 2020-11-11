using UnityEngine.UI;

public class UiImpulseResponse 
{
    public Button showButton { get; private set; }

    public InputField microphoneInputField { get; private set; }

    public UiImpulseResponse(InputField microInputField, Button showBtn)
    {
        microphoneInputField = microInputField;
        showButton = showBtn;
    }
}
