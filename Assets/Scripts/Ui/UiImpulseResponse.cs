using UnityEngine;
using UnityEngine.UI;

public class UiImpulseResponse 
{
    public Button showButton { get; private set; }

    public InputField microphoneInputField { get; private set; }

    public UiImpulseResponse(GameObject menu)
    {
        GameObject impulseResponsePanel = menu.transform.Find("TabPanel").
                                          Find("TabPanels").
                                          Find("ImpulseResponsePanel").
                                          gameObject;

        microphoneInputField = impulseResponsePanel.transform.Find("NumberOfMicrophoneInputField").
                               gameObject.GetComponent<InputField>();

        showButton = impulseResponsePanel.transform.Find("ImpulseResponsePlotButton").gameObject.GetComponent<Button>();
    }
}
