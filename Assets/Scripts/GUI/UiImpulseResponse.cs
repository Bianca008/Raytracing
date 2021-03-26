using UnityEngine;
using UnityEngine.UI;

public class UiImpulseResponse 
{
    public Button ShowButton { get; private set; }

    public InputField MicrophoneInputField { get; private set; }

    public UiImpulseResponse()
    {
        GameObject impulseResponsePanel = GameObject.Find("MenuObject").gameObject.transform.
                                          Find("Menu").
                                          Find("TabPanel").
                                          Find("TabPanels").
                                          Find("ImpulseResponsePanel").
                                          gameObject;

        MicrophoneInputField = impulseResponsePanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();
        MicrophoneInputField.characterValidation = InputField.CharacterValidation.Integer;
        ShowButton = impulseResponsePanel.transform.Find("ImpulseResponsePlotButton").gameObject.GetComponent<Button>();
    }
}
