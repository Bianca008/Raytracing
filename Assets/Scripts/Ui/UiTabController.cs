using UnityEngine;
using UnityEngine.UI;

public class UiTabController
{
    private GameObject m_menuCanvas;

    public UiTabController(GameObject menu, 
        Button inputTabButton,
        Button timeEchogramButton,
        Button frequencyEchogramButton,
        Button impulseResponseButton)
    {
        m_menuCanvas = menu;

        AddListenerForButton(inputTabButton, 0);
        AddListenerForButton(timeEchogramButton, 1);
        AddListenerForButton(frequencyEchogramButton, 2);
        AddListenerForButton(impulseResponseButton, 3);

        InitializeTabs(0);
    }

    private void InitializeTabs(int indexVisibleTab)
    {
        GameObject tabPanels = m_menuCanvas.transform.Find("TabPanel").gameObject;
        GameObject panels = tabPanels.transform.Find("TabPanels").gameObject;        GameObject inputTabPanel = panels.transform.Find("InputTabPanel").gameObject;
        GameObject timeTabPanel = panels.transform.Find("TimeEchogramPanel").gameObject;
        GameObject frequencyTabPanel = panels.transform.Find("FrequencyEchogramPanel").gameObject;
        GameObject impulseResponePanel = panels.transform.Find("ImpulseResponsePanel").gameObject;

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
