using UnityEngine;
using UnityEngine.UI;

public class UiRayMenu
{
    public GameObject canvasMenu { get; set; }

    public InputField rayNumber { get; set; }

    public InputField microphoneNumber { get; set; }

    public Button showButton { get; set; }

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
    }

    public void AddListenerForShowButton(RaysDrawer raysDrawer)
    {
        showButton.onClick.AddListener(() =>
        { raysDrawer.Draw(int.Parse(microphoneNumber.text), int.Parse(rayNumber.text)); });
    }
}
