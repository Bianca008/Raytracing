using UnityEngine;
using UnityEngine.UI;

public class UiConfigurationInput
{
    public InputField numberOfReflections { get; private set; }

    public InputField maxDistance { get; private set; }

    public Dropdown frequencyStep { get; private set; }

    public Button setConfiguration { get; private set; }

    public Button soundButton { get; private set; }

    public UiConfigurationInput()
    {
        GameObject inputPanel = GameObject.Find("Menu").gameObject.transform.
                                Find("TabPanel").
                                Find("TabPanels").
                                Find("InputTabPanel").
                                gameObject;

        numberOfReflections = inputPanel.transform.Find("Input").Find("NumberOfReflectionsInputField").gameObject.GetComponent<InputField>();
        maxDistance = inputPanel.transform.Find("Input").Find("MaxDistanceInputField").GetComponent<InputField>();
        frequencyStep = inputPanel.transform.Find("Input").Find("FrequencyStepDropdown").gameObject.GetComponent<Dropdown>();
        soundButton = inputPanel.transform.Find("Input").Find("SoundButton").gameObject.GetComponent<Button>();
        setConfiguration = inputPanel.transform.Find("SetConfigurationButton").gameObject.GetComponent<Button>();
    }
}
