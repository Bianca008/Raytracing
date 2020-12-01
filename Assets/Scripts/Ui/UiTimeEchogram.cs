using UnityEngine;
using UnityEngine.UI;

public class UiTimeEchogram
{
    public InputField microphoneInputField { get; private set; }

    public InputField frequencyInputField { get; private set; }

    public Button showButton { get; private set; }

    public UiTimeEchogram(GameObject menu)
    {
        GameObject timeEchogramPanel = menu.transform.Find("TabPanel").
           gameObject.transform.Find("TabPanels").
           gameObject.transform.Find("TimeEchogramPanel").
           gameObject;

        microphoneInputField = timeEchogramPanel.transform.Find("InputTime").
            gameObject.transform.Find("NumberOfMicrophoneInputField").
            gameObject.GetComponent<InputField>() as InputField;

        frequencyInputField = timeEchogramPanel.transform.Find("InputTime").
            gameObject.transform.Find("FrequencyInputField").
            gameObject.GetComponent<InputField>() as InputField;

        showButton = timeEchogramPanel.transform.Find("TimePlotButton").gameObject.GetComponent<Button>() as Button;
    }
}
