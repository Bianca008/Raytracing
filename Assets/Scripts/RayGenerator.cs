using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SFB;

public class RayGenerator : MonoBehaviour
{
    public Material LineMaterial;

    private Solver m_solver;

    private UiConfigurationInput m_configurationInput;
    private UiRayMenu m_rayMenu;
    private UiMenuHandler m_menuHandler;
    private RaysDrawer m_intersectedRayDrawer;
    private LineRenderer[] m_intersectedLines;
    private AudioSource m_audioSource;
    private double m_maxFrequency;

    private void Start()
    {
        m_solver = new Solver(VectorConverter.Convert(transform.position));
        m_audioSource = GetComponent<AudioSource>();
        m_solver.CreateMicrophones();
        m_intersectedRayDrawer = new RaysDrawer();
        DrawMicrophones();
        InitializeUi();
    }

    private void Update()
    {
        Pressed();
    }

    private void DrawMicrophones()
    {
        foreach (var microphone in m_solver.Microphones)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = VectorConverter.Convert(microphone.center);
            sphere.transform.localScale = new UnityEngine.Vector3(microphone.radius,
                microphone.radius,
                microphone.radius);
            sphere.AddComponent<BoxCollider>();
        }
    }

    private void InitializeUi()
    {
        m_configurationInput = new UiConfigurationInput();
        AddListenerForSoundButton();
        m_configurationInput.setConfiguration.onClick.AddListener(RunAlgorithm);

        var uiTabController = new UiTabController();
        var uiTimeEchogram = new UiTimeEchogram();
        var uiFrequencyEchogram = new UiFrequencyEchogram();
        var uiImpulseResponse = new UiImpulseResponse();

        var uiHandler = new UiHandler(uiTimeEchogram, uiFrequencyEchogram, uiImpulseResponse, m_solver.Frequencies);

        uiHandler.InitializeUi(m_solver.Rays,
            m_solver.Microphones,
            m_solver.Echograms,
            m_solver.FrequencyResponse,
            m_solver.ImpulseResponses);

        m_rayMenu = new UiRayMenu();
        m_menuHandler = new UiMenuHandler();
        m_menuHandler.AddListenerForMenuButton();
        m_menuHandler.AddListenerForRayMenuButton(m_rayMenu.canvasMenu);
        m_rayMenu.AddListenerForShowButton(m_intersectedRayDrawer);
    }

    private void RunAlgorithm()
    {
        m_solver.RunSolver(m_audioSource.clip.name, InputHandler.GetNumber(m_configurationInput.numberOfReflections),
            InputHandler.GetNumber(m_configurationInput.numberOfRays),
            InputHandler.GetNumber(m_configurationInput.maxDistance),
            InputHandler.GetCheckedDropdownElement(m_configurationInput.frequencyStep));

        CreateDrawableRays();
    }

    private void CreateDrawableRays()
    {
        var count = 0;
        for (int index = 0; index < m_solver.Rays.Count; ++index)
            count += m_solver.Rays[index].Count;
        m_intersectedLines = LinesCreator.GenerateLines(count, transform, LineMaterial);
        m_intersectedRayDrawer.lines = m_intersectedLines;
        m_intersectedRayDrawer.rays = m_solver.Rays;
    }

    private void SetVisibilityForLoadingText()
    {
        Text loadingText = GameObject.Find("Menu").gameObject.transform.
                           Find("TabPanel").
                           Find("TabPanels").
                           Find("InputTabPanel").
                           Find("LoadingText").GetComponent<Text>();

        Color visibleColor = loadingText.color;
        visibleColor.a = 1 - visibleColor.a;
        loadingText.color = visibleColor;
    }

    private void AddListenerForSoundButton()
    {
        m_configurationInput.soundButton.onClick.AddListener(ShowDialog);
    }

    private void ShowDialog()
    {
        var extensions = new[] {
                                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                                };
        var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
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
                    m_audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    m_audioSource.clip.name = Path.GetFileNameWithoutExtension(path);
                    m_audioSource.Play();
                    m_maxFrequency = SoundConvolver.GetMaxFrequency(m_audioSource);
                }
            }
            Debug.Log("---------------------File is loaded!----------------------");
        }
    }

    private void Pressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 clickedPosition = new Vector3(0, 0, 0);
            if (Physics.Raycast(ray, out hit, 100))
            {
                clickedPosition = hit.transform.position;
                Debug.Log(hit.transform.position);
            }

            foreach (MicrophoneSphere microphone in m_solver.Microphones)
            {
                if (microphone.IsAroundMicro(VectorConverter.Convert(clickedPosition)) == true &&
                    SoundConvolver.convolvedSounds != null &&
                    SoundConvolver.convolvedSounds.Count > 0)
                {
                    float[] f = SoundConvolver.convolvedSounds[microphone.id];
                    var clip = AudioClip.Create("testSound", f.Length, 1, 44100, false, false);
                    clip.SetData(f, 0);
                    AudioSource.PlayClipAtPoint(clip, clickedPosition, 1.0f);
                }
            }
        }
    }
}
