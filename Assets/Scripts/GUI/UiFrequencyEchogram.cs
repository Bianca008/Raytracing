using UnityEngine;
using UnityEngine.UI;

public class UiFrequencyEchogram
{
    public InputField MicrophoneInputField { get; private set; }

    public Button ShowButton { get; private set; }

    public UiFrequencyEchogram()
    {
        GameObject frequencyEchogramPanel = GameObject.Find("MenuObject").gameObject.transform.
                                            Find("Menu").
                                            Find("TabPanel").
                                            Find("TabPanels").
                                            Find("FrequencyEchogramPanel").
                                            gameObject;

        MicrophoneInputField = frequencyEchogramPanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();
        MicrophoneInputField.characterValidation = InputField.CharacterValidation.Integer;
        ShowButton = frequencyEchogramPanel.transform.Find("FrequencyPlotButton").gameObject.GetComponent<Button>();
    }
}