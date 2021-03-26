using UnityEngine;
using UnityEngine.UI;

public class UiConfigurationInput
{
    public InputField NumberOfReflections { get; private set; }

    public InputField MaxDistance { get; private set; }

    public InputField NumberOfRays { get; private set; }

    public Dropdown FrequencyStep { get; private set; }

    public Button SetConfiguration { get; private set; }

    public Button SoundButton { get; private set; }

    public UiConfigurationInput()
    {
        GameObject inputPanel = GameObject.Find("MenuObject").gameObject.transform.
                                Find("Menu").
                                Find("TabPanel").
                                Find("TabPanels").
                                Find("InputTabPanel").
                                gameObject;

        NumberOfReflections = inputPanel.transform.Find("Input").Find("NumberOfReflectionsInputField").gameObject.GetComponent<InputField>();
        NumberOfReflections.characterValidation = InputField.CharacterValidation.Integer;
        MaxDistance = inputPanel.transform.Find("Input").Find("MaxDistanceInputField").GetComponent<InputField>();
        MaxDistance.characterValidation = InputField.CharacterValidation.Integer;
        NumberOfRays = inputPanel.transform.Find("Input").Find("NumberOfRaysInputField").gameObject.GetComponent<InputField>();
        NumberOfRays.characterValidation = InputField.CharacterValidation.Integer;
        FrequencyStep = inputPanel.transform.Find("Input").Find("FrequencyStepDropdown").gameObject.GetComponent<Dropdown>();
        SoundButton = inputPanel.transform.Find("Input").Find("SoundButton").gameObject.GetComponent<Button>();
        SetConfiguration = inputPanel.transform.Find("SetConfigurationButton").gameObject.GetComponent<Button>();
    }
}
