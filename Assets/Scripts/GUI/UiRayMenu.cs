using UnityEngine;
using UnityEngine.UI;

public class UiRayMenu
{
    public GameObject canvasMenu { get; set; }

    public InputField rayNumber { get; set; }

    public InputField microphoneNumber { get; set; }

    public Button showButton { get; set; }

    public Button allButton { get; set; }

    public UiRayMenu()
    {
        GameObject rayMenu = GameObject.Find("MenuObject").
                              gameObject.transform.Find("RayMenu").
                              gameObject.transform.Find("Panel").
                              gameObject;

        canvasMenu = GameObject.Find("RayMenu").gameObject;
        rayNumber = rayMenu.transform.Find("RayNumberInputField").GetComponent<InputField>();
        rayNumber.characterValidation = InputField.CharacterValidation.Integer;
        microphoneNumber = rayMenu.transform.Find("MicrophoneNumberInputField").GetComponent<InputField>();
        microphoneNumber.characterValidation = InputField.CharacterValidation.Integer;
        showButton = rayMenu.transform.Find("ShowButton").GetComponent<Button>();
        allButton = rayMenu.transform.Find("AllButton").GetComponent<Button>();
    }

    public void AddListenerForShowButton(RaysDrawer raysDrawer, Material material)
    {
        showButton.onClick.AddListener(() =>
        { raysDrawer.Draw(material, int.Parse(microphoneNumber.text), int.Parse(rayNumber.text)); });
    }

    public void AddListenerForAllButton(RaysDrawer raysDrawer, Material material)
    {
        allButton.onClick.AddListener(() =>
        { raysDrawer.DrawAll(material); });
    }
}
