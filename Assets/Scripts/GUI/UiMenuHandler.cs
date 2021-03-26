using UnityEngine;
using UnityEngine.UI;

public class UiMenuHandler
{
    public GameObject MenuRays { get; set; }

    public GameObject MenuInput { get; set; }

    public GameObject MenuBar { get; set; }

    public Button ViewMenu { get; set; }

    public Button ViewRay { get; set; }

    public UiMenuHandler()
    {
        var principalMenuCanvas = GameObject.Find("MenuObject").
                               gameObject.transform.Find("PrincipalMenuCanvas").
                               Find("Panel").
                               gameObject;

        MenuRays = GameObject.Find("MenuObject").gameObject.transform.Find("RayMenu").gameObject;
        MenuInput = GameObject.Find("MenuObject").gameObject.transform.Find("Menu").gameObject;
        MenuBar = GameObject.Find("MenuObject").gameObject.transform.Find("PrincipalMenuCanvas").gameObject;
        ViewMenu = principalMenuCanvas.transform.Find("MenuButton").GetComponent<Button>();
        ViewRay = principalMenuCanvas.transform.Find("ViewRayButton").GetComponent<Button>();
        MenuInput.SetActive(false);
        MenuRays.SetActive(false);
    }

    public void AddListenerForMenuButton()
    {
        ViewMenu.onClick.AddListener(() => { SetTransparentMenu(MenuInput); });
    }

    private void SetTransparentMenu(GameObject menu)
    {
        var group = menu.GetComponent<CanvasGroup>();
        group.alpha = 1 - group.alpha;
        if (group.alpha == 0)
            menu.SetActive(false);
        else
            menu.SetActive(true);
        MenuBar.SetActive(false);
        MenuBar.SetActive(true);
    }

    public void AddListenerForRayMenuButton()
    {
        ViewRay.onClick.AddListener(() => { SetTransparentMenu(MenuRays); });
    }
}
