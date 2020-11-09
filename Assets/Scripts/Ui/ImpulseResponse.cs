using UnityEngine.UI;

public class ImpulseResponse 
{
    public Button showButton { get; private set; }

    public InputField microphoneInputField { get; private set; }

    public ImpulseResponse(InputField microInputField, Button showBtn)
    {
        microphoneInputField = microInputField;
        showButton = showBtn;
    }
}
