using UnityEngine;
using UnityEngine.UI;

public class UiMenuHandler
{
    public Button viewMenu { get; set; }

    public Button viewRay { get; set; }

    public UiMenuHandler()
    {
        GameObject principalMenuCanvas = GameObject.Find("PrincipalMenuCanvas").
                               gameObject.transform.Find("Panel").
                               gameObject;

        viewMenu = principalMenuCanvas.transform.Find("MenuButton").GetComponent<Button>();
        viewRay = principalMenuCanvas.transform.Find("ViewRayButton").GetComponent<Button>();
    }

    public void AddListenerForMenuButton(GameObject menu)
    {
        viewMenu.onClick.AddListener(() => { SetTransparentMenu(menu); });
    }

    private void SetTransparentMenu(GameObject menu)
    {
        CanvasGroup group = menu.GetComponent<CanvasGroup>();
        group.alpha = 1 - group.alpha;
    }
}
