using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using SFB;

public class RayGenerator : MonoBehaviour
{
    public Material LineMaterial;

    private Solver solver;

    private UiConfigurationInput configurationInput;
    private UiRayMenu rayMenu;
    private UiMenuHandler menuHandler;
    private RaysDrawer intersectedRayDrawer;
    private AudioSource audioSource;
    private double maxFrequency;

    private void Start()
    {
        solver = new Solver(VectorConverter.Convert(transform.position));
        audioSource = GetComponent<AudioSource>();
        solver.CreateMicrophones();
        intersectedRayDrawer = new RaysDrawer();
        DrawMicrophones();
        InitializeUi();
    }

    private void Update()
    {
        Pressed();
    }

    private void DrawMicrophones()
    {
        GameObject go = new GameObject("Microphones");
        foreach (var microphone in solver.Microphones)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = VectorConverter.Convert(microphone.Center);
            sphere.transform.localScale = new UnityEngine.Vector3(microphone.Radius,
                microphone.Radius,
                microphone.Radius);
            sphere.AddComponent<BoxCollider>();
            sphere.transform.parent = go.transform;

            var textGo = new GameObject("MicrophoneText");
            textGo.transform.parent = sphere.transform;
            var text = textGo.AddComponent<TextMesh>();
            text.text = microphone.Id.ToString();
            var arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.font = arialFont;
            text.fontSize = 40;
            text.alignment = TextAlignment.Center;
            text.characterSize = 0.03f;
            text.transform.position = new Vector3(sphere.transform.position.x,
                sphere.transform.position.y + microphone.Radius * 2,
                sphere.transform.position.z);
        }
    }

    private void InitializeUi()
    {
        configurationInput = new UiConfigurationInput();
        AddListenerForSoundButton();
        configurationInput.SetConfiguration.onClick.AddListener(RunAlgorithm);

        var uiTabController = new UiTabController();
        var uiTimeEchogram = new UiTimeEchogram();
        var uiFrequencyEchogram = new UiFrequencyEchogram();
        var uiImpulseResponse = new UiImpulseResponse();

        var uiHandler = new UiHandler(uiTimeEchogram, uiFrequencyEchogram, uiImpulseResponse, solver.Frequencies);

        uiHandler.InitializeUi(solver.Rays,
            solver.Microphones,
            solver.Echograms,
            solver.FrequencyResponse,
            solver.ImpulseResponses);

        rayMenu = new UiRayMenu();
        menuHandler = new UiMenuHandler();
        menuHandler.AddListenerForMenuButton();
        menuHandler.AddListenerForRayMenuButton();
        rayMenu.AddListenerForShowButton(intersectedRayDrawer, LineMaterial);
        rayMenu.AddListenerForAllButton(intersectedRayDrawer, LineMaterial);
    }

    private void RunAlgorithm()
    {
        Debug.Log("Audio: " + audioSource.clip.name);
        solver.RunSolver(audioSource.clip.name, InputHandler.GetNumber(configurationInput.NumberOfReflections),
            InputHandler.GetNumber(configurationInput.NumberOfRays),
            InputHandler.GetNumber(configurationInput.MaxDistance),
            InputHandler.GetCheckedDropdownElement(configurationInput.FrequencyStep));

        intersectedRayDrawer.Rays = solver.Rays;
    }

    private void AddListenerForSoundButton()
    {
        configurationInput.SoundButton.onClick.AddListener(ShowDialog);
    }

    private void ShowDialog()
    {
        var extensions = new[] {
                                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                                };
        var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        Debug.Log(path[0]);
        StartCoroutine(LoadAudioClip(path[0]));
    }

    private IEnumerator LoadAudioClip(string path)
    {
        if (path.Length != 0)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip.name = Path.GetFileNameWithoutExtension(path);
                    audioSource.Play();
                    maxFrequency = SoundConvolver.GetMaxFrequency(audioSource);
                }
            }
            Debug.Log("---------------------File is loaded!----------------------");
        }
    }

    private void Pressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            var clickedPosition = new Vector3(0, 0, 0);
            if (Physics.Raycast(ray, out hit, 100))
            {
                clickedPosition = hit.transform.position;
                Debug.Log(hit.transform.position);
            }

            foreach (var microphone in solver.Microphones)
            {
                if (microphone.IsAroundMicro(VectorConverter.Convert(clickedPosition)) == true &&
                    SoundConvolver.ConvolvedSounds != null &&
                    SoundConvolver.ConvolvedSounds.Count > 0)
                {
                    float[] f = SoundConvolver.ConvolvedSounds[microphone.Id];
                    var clip = AudioClip.Create("testSound", f.Length, 1, 44100, false, false);
                    clip.SetData(f, 0);
                    Debug.Log("AM ajuns aici");
                    AudioSource.PlayClipAtPoint(clip, clickedPosition, 1.0f);
                }
            }
        }
    }

    public void ClearAll()
    {
        solver.ResetSolver();
        intersectedRayDrawer.Lines.Clear();
        intersectedRayDrawer.Rays.Clear();
        SoundConvolver.ConvolvedSounds.Clear();
    }
}
