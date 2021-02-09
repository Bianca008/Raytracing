using UnityEngine;
using UnityEngine.UI;

public class UiTimeEchogram
{
    public InputField microphoneInputField { get; private set; }

    public InputField frequencyInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiTimeEchogram()
    {
        GameObject timeEchogramPanel = GameObject.Find("MenuObject").gameObject.transform.
                                       Find("Menu").
                                       Find("TabPanel").
                                       Find("TabPanels").
                                       Find("TimeEchogramPanel").
                                       gameObject;

        microphoneInputField = timeEchogramPanel.transform.Find("InputTime").
                               Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();
        microphoneInputField.characterValidation = InputField.CharacterValidation.Integer;
        frequencyInputField = timeEchogramPanel.transform.Find("InputTime").
                              Find("FrequencyInputField").
                              gameObject.GetComponent<InputField>();
        frequencyInputField.characterValidation = InputField.CharacterValidation.Integer;
        showButton = timeEchogramPanel.transform.Find("TimePlotButton").gameObject.GetComponent<Button>();
    }
}
