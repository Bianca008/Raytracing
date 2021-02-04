using UnityEngine;
using UnityEngine.UI;

public class UiConfigurationInput
{
    public InputField numberOfReflections { get; private set; }

    public InputField maxDistance { get; private set; }

    public InputField numberOfRays { get; private set; }

    public Dropdown frequencyStep { get; private set; }

    public Button setConfiguration { get; private set; }

    public Button soundButton { get; private set; }

    public UiConfigurationInput()
    {
        GameObject inputPanel = GameObject.Find("Menu").gameObject.transform.
                                Find("Menu").
                                Find("TabPanel").
                                Find("TabPanels").
                                Find("InputTabPanel").
                                gameObject;

        numberOfReflections = inputPanel.transform.Find("Input").Find("NumberOfReflectionsInputField").gameObject.GetComponent<InputField>();
        numberOfReflections.characterValidation = InputField.CharacterValidation.Integer;
        maxDistance = inputPanel.transform.Find("Input").Find("MaxDistanceInputField").GetComponent<InputField>();
        maxDistance.characterValidation = InputField.CharacterValidation.Integer;
        numberOfRays = inputPanel.transform.Find("Input").Find("NumberOfRaysInputField").gameObject.GetComponent<InputField>();
        numberOfRays.characterValidation = InputField.CharacterValidation.Integer;
        frequencyStep = inputPanel.transform.Find("Input").Find("FrequencyStepDropdown").gameObject.GetComponent<Dropdown>();
        soundButton = inputPanel.transform.Find("Input").Find("SoundButton").gameObject.GetComponent<Button>();
        setConfiguration = inputPanel.transform.Find("SetConfigurationButton").gameObject.GetComponent<Button>();
    }
}
