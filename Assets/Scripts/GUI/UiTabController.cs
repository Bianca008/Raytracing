using UnityEngine;
using UnityEngine.UI;

public class UiTabController
{
    private GameObject menuCanvas;

    public UiTabController()
    {
        menuCanvas = GameObject.Find("MenuObject").gameObject.transform.Find("Menu").gameObject;
        var tabButtonsPanel = menuCanvas.transform.
                                     Find("TabPanel").
                                     Find("TabButtonsPanel").
                                     gameObject;
        var inputTabButton = tabButtonsPanel.transform.Find("InputTabButton").gameObject.GetComponent<Button>();
        var timeEchogramButton = tabButtonsPanel.transform.Find("TimeEchogramTabButton").gameObject.GetComponent<Button>();
        var frequencyEchogramButton = tabButtonsPanel.transform.Find("FrequencyEchogramTabButton").gameObject.GetComponent<Button>();
        var impulseResponseButton = tabButtonsPanel.transform.Find("ImpulseResponseTabButton").gameObject.GetComponent<Button>();

        AddListenerForButton(inputTabButton, 0);
        AddListenerForButton(timeEchogramButton, 1);
        AddListenerForButton(frequencyEchogramButton, 2);
        AddListenerForButton(impulseResponseButton, 3);

        InitializeTabs(0);
    }

    private void InitializeTabs(int indexVisibleTab)
    {
        var tabPanels = menuCanvas.transform.Find("TabPanel").gameObject;
        var panels = tabPanels.transform.Find("TabPanels").gameObject;
        var inputTabPanel = panels.transform.Find("InputTabPanel").gameObject;
        var timeTabPanel = panels.transform.Find("TimeEchogramPanel").gameObject;
        var frequencyTabPanel = panels.transform.Find("FrequencyEchogramPanel").gameObject;
        var impulseResponePanel = panels.transform.Find("ImpulseResponsePanel").gameObject;

        inputTabPanel.SetActive(false);
        timeTabPanel.SetActive(false);
        frequencyTabPanel.SetActive(false);
        impulseResponePanel.SetActive(false);

        switch(indexVisibleTab)
        {
            case 0:
                inputTabPanel.SetActive(true);
                break;
            case 1:
                timeTabPanel.SetActive(true);
                break;
            case 2:
                frequencyTabPanel.SetActive(true);
                break;
            case 3:
                impulseResponePanel.SetActive(true);
                break;
        }
    }

    private void AddListenerForButton(Button button, int indexTab)
    {
        button.onClick.AddListener(() =>
        {
            InitializeTabs(indexTab);
        });
    }
}
