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
        GameObject rayMenu = GameObject.Find("RayMenu").
                              gameObject.transform.Find("Panel").
                              gameObject;

        canvasMenu = GameObject.Find("RayMenu").gameObject;
        rayNumber = rayMenu.transform.Find("RayNumberInputField").GetComponent<InputField>();
        microphoneNumber = rayMenu.transform.Find("MicrophoneNumberInputField").GetComponent<InputField>();
        showButton = rayMenu.transform.Find("ShowButton").GetComponent<Button>();
    }

    public void AddListenerForShowButton(RaysDrawer raysDrawer)
    {
        /*TO DO: Please add validation for input*/
        //if (microphoneNumber.text.ToString().Trim().Length != 0 &&
        //    rayNumber.text.ToString().Trim().Length != 0)
            showButton.onClick.AddListener(() =>
            { raysDrawer.Draw(int.Parse(microphoneNumber.text), int.Parse(rayNumber.text)); });
    }
}
