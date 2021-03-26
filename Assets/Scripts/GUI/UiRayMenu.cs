using UnityEngine;
using UnityEngine.UI;

public class UiRayMenu
{
    public GameObject CanvasMenu { get; set; }

    public InputField RayNumber { get; set; }

    public InputField MicrophoneNumber { get; set; }

    public Button ShowButton { get; set; }

    public Button AllButton { get; set; }

    public UiRayMenu()
    {
        var rayMenu = GameObject.Find("MenuObject").
                              gameObject.transform.Find("RayMenu").
                              gameObject.transform.Find("Panel").
                              gameObject;

        CanvasMenu = GameObject.Find("RayMenu").gameObject;
        RayNumber = rayMenu.transform.Find("RayNumberInputField").GetComponent<InputField>();
        RayNumber.characterValidation = InputField.CharacterValidation.Integer;
        MicrophoneNumber = rayMenu.transform.Find("MicrophoneNumberInputField").GetComponent<InputField>();
        MicrophoneNumber.characterValidation = InputField.CharacterValidation.Integer;
        ShowButton = rayMenu.transform.Find("ShowButton").GetComponent<Button>();
        AllButton = rayMenu.transform.Find("AllButton").GetComponent<Button>();
    }

    public void AddListenerForShowButton(RaysDrawer raysDrawer, Material material)
    {
        ShowButton.onClick.AddListener(() =>
        { raysDrawer.Draw(material, int.Parse(MicrophoneNumber.text), int.Parse(RayNumber.text)); });
    }

    public void AddListenerForAllButton(RaysDrawer raysDrawer, Material material)
    {
        AllButton.onClick.AddListener(() =>
        { raysDrawer.DrawAll(material); });
    }
}
