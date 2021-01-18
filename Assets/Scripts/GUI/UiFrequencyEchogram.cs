using UnityEngine;
using UnityEngine.UI;

public class UiFrequencyEchogram
{
    public InputField microphoneInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiFrequencyEchogram(GameObject menu)
    {
        GameObject frequencyEchogramPanel = menu.transform.Find("TabPanel").
                                            Find("TabPanels").
                                            Find("FrequencyEchogramPanel").
                                            gameObject;

        microphoneInputField = frequencyEchogramPanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();

        showButton = frequencyEchogramPanel.transform.Find("FrequencyPlotButton").gameObject.GetComponent<Button>();
    }
}