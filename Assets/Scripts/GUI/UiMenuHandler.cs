using UnityEngine;
using UnityEngine.UI;

public class UiMenuHandler
{
    public GameObject menuInput { get; set; }

    public GameObject menuBar { get; set; }

    public Button viewMenu { get; set; }

    public Button viewRay { get; set; }

    public UiMenuHandler()
    {
        GameObject principalMenuCanvas = GameObject.Find("MenuObject").
                               gameObject.transform.Find("PrincipalMenuCanvas").
                               Find("Panel").
                               gameObject;

        menuInput = GameObject.Find("MenuObject").gameObject.transform.Find("Menu").gameObject;
        menuBar = GameObject.Find("MenuObject").gameObject.transform.Find("PrincipalMenuCanvas").gameObject;
        viewMenu = principalMenuCanvas.transform.Find("MenuButton").GetComponent<Button>();
        viewRay = principalMenuCanvas.transform.Find("ViewRayButton").GetComponent<Button>();
    }

    public void AddListenerForMenuButton()
    {
        viewMenu.onClick.AddListener(() => { SetTransparentMenu(menuInput); });
    }

    private void SetTransparentMenu(GameObject menu)
    {
        CanvasGroup group = menu.GetComponent<CanvasGroup>();
        group.alpha = 1 - group.alpha;
        if (group.alpha == 0)
            menu.SetActive(false);
        else
            menu.SetActive(true);
        menuBar.SetActive(false);
        menuBar.SetActive(true);
    }

    public void AddListenerForRayMenuButton(GameObject menu)
    {
        viewRay.onClick.AddListener(() => { SetTransparentMenu(menu); });
    }
}
