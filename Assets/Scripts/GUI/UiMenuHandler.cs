using UnityEngine;
using UnityEngine.UI;

public class UiMenuHandler
{
    public GameObject menuBar { get; set; }

    public Button viewMenu { get; set; }

    public Button viewRay { get; set; }

    public UiMenuHandler()
    {
        GameObject principalMenuCanvas = GameObject.Find("PrincipalMenuCanvas").
                               gameObject.transform.Find("Panel").
                               gameObject;

        menuBar = GameObject.Find("PrincipalMenuCanvas").gameObject;
        viewMenu = principalMenuCanvas.transform.Find("MenuButton").GetComponent<Button>();
        viewRay = principalMenuCanvas.transform.Find("ViewRayButton").GetComponent<Button>();
    }

    public void AddListenerForMenuButton()
    {
        viewMenu.onClick.AddListener(() => { SetTransparentMenu(GameObject.Find("Menu").gameObject); });
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
