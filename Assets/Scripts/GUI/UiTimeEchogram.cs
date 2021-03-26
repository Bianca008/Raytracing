using UnityEngine;
using UnityEngine.UI;

public class UiTimeEchogram
{
    public InputField MicrophoneInputField { get; private set; }

    public InputField FrequencyInputField { get; private set; }

    public Button ShowButton { get; private set; }

    public UiTimeEchogram()
    {
        var timeEchogramPanel = GameObject.Find("MenuObject").gameObject.transform.
                                       Find("Menu").
                                       Find("TabPanel").
                                       Find("TabPanels").
                                       Find("TimeEchogramPanel").
                                       gameObject;

        MicrophoneInputField = timeEchogramPanel.transform.Find("InputTime").
                               Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();
        MicrophoneInputField.characterValidation = InputField.CharacterValidation.Integer;
        FrequencyInputField = timeEchogramPanel.transform.Find("InputTime").
                              Find("FrequencyInputField").
                              gameObject.GetComponent<InputField>();
        FrequencyInputField.characterValidation = InputField.CharacterValidation.Integer;
        ShowButton = timeEchogramPanel.transform.Find("TimePlotButton").gameObject.GetComponent<Button>();
    }
}
