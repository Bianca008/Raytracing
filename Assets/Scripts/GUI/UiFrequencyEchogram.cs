using UnityEngine;
using UnityEngine.UI;

public class UiFrequencyEchogram
{
    public InputField microphoneInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiFrequencyEchogram()
    {
        GameObject frequencyEchogramPanel = GameObject.Find("Menu").gameObject.transform.
                                            Find("TabPanel").
                                            Find("TabPanels").
                                            Find("FrequencyEchogramPanel").
                                            gameObject;

        microphoneInputField = frequencyEchogramPanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();
        microphoneInputField.characterValidation = InputField.CharacterValidation.Integer;
        showButton = frequencyEchogramPanel.transform.Find("FrequencyPlotButton").gameObject.GetComponent<Button>();
    }
}