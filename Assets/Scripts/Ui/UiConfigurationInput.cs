﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiConfigurationInput 
{
    public InputField numberOfReflections { get; private set; }

    public InputField maxDistance { get; private set; }

    public Dropdown frequencyStep { get; private set; }

    public Button setConfiguration { get; private set; }

    public UiConfigurationInput(GameObject menu)
    {
        GameObject inputPanel = menu.transform.Find("TabPanel").
                                gameObject.transform.Find("TabPanels").
                                gameObject.transform.Find("InputTabPanel").
                                gameObject;

        numberOfReflections = inputPanel.transform.Find("Input").Find("NumberOfReflectionsInputField").gameObject.GetComponent<InputField>();
        maxDistance = inputPanel.transform.Find("Input").Find("MaxDistanceInputField").GetComponent<InputField>();
        frequencyStep = inputPanel.transform.Find("Input").Find("FrequencyStepDropdown").gameObject.GetComponent<Dropdown>();
        setConfiguration = inputPanel.transform.Find("SetConfigurationButton").gameObject.GetComponent<Button>();
    }
}
