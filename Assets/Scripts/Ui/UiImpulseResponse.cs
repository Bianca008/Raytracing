using UnityEngine;
using UnityEngine.UI;

public class UiImpulseResponse 
{
    public Button showButton { get; private set; }

    public InputField microphoneInputField { get; private set; }

    public UiImpulseResponse(GameObject menu)
    {
        GameObject frequencyEchogramPanel = menu.transform.Find("TabPanel").
         gameObject.transform.Find("TabPanels").
         gameObject.transform.Find("ImpulseResponsePanel").
         gameObject;

        microphoneInputField = frequencyEchogramPanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>() as InputField;

        showButton = frequencyEchogramPanel.transform.Find("ImpulseResponsePlotButton").gameObject.GetComponent<Button>() as Button;
    }
}
