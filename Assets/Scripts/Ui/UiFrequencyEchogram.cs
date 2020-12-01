using UnityEngine;
using UnityEngine.UI;

public class UiFrequencyEchogram
{
    public InputField microphoneInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiFrequencyEchogram(GameObject menu)
    {
        GameObject frequencyEchogramPanel = menu.transform.Find("TabPanel").
         gameObject.transform.Find("TabPanels").
         gameObject.transform.Find("FrequencyEchogramPanel").
         gameObject;

        microphoneInputField = frequencyEchogramPanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>() as InputField;

        showButton = frequencyEchogramPanel.transform.Find("FrequencyPlotButton").gameObject.GetComponent<Button>() as Button;
    }
}