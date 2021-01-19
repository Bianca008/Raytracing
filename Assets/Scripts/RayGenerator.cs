using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NWaves.Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Echogram = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Numerics.Complex>>;

public class RayGenerator : MonoBehaviour
{
    public int NumberOfRay = 1;
    public float InitialPower = 1;
    public int NumberOfMicrophone = 1;
    public int NumberOfRays = 0;
    public int IntersectedRays = 0;
    public int IntersectedRaysWithDuplicate = 0;
    public Material LineMaterial;
    public GameObject MenuCanvas;
    private Solver m_solver;
    private double m_maxFrequency;
    private UiConfigurationInput m_configurationInput;
    private UiRayMenu m_rayMenu;
    private UiMenuHandler m_menuHandler;
    private RaysDrawer m_intersectedRayDrawer;
    private LineRenderer[] m_intersectedLines;
    private AudioSource m_audioSource;

    private void Start()
    {
        m_solver = new Solver(VectorConverter.Convert(transform.position));
        m_audioSource = GetComponent<AudioSource>();
        m_solver.CreateMicrophones();
        m_intersectedRayDrawer = new RaysDrawer();
        //CreateDrawableRays();
        DrawMicrophones();
        InitializeUi();
    }

    private void Update()
    {
        //if (NumberOfRay <= IntersectedRays && NumberOfRay >= 1 &&
        //    NumberOfMicrophone <= m_microphones.Count && NumberOfMicrophone >= 1)
        //{
        //    m_intersectedRayDrawer.Draw(NumberOfMicrophone - 1, NumberOfRay - 1);
        //    IntersectedRays = m_rays[NumberOfMicrophone - 1].Count;
        //}
        //else
        //    Debug.Log("The number of ray or the number of microphone does not exist...");

        //if (Input.GetKey("i"))
        //    MenuCanvas.SetActive(false);
        //if (Input.GetKey("o"))
        //    MenuCanvas.SetActive(true);

        // Pressed();
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
        m_configurationInput = new UiConfigurationInput(MenuCanvas);
        AddListenerForSoundButton();
        m_configurationInput.setConfiguration.onClick.AddListener(RunAlgorithm);

        var uiTabController = new UiTabController(MenuCanvas);
        var uiTimeEchogram = new UiTimeEchogram(MenuCanvas);
        var uiFrequencyEchogram = new UiFrequencyEchogram(MenuCanvas);
        var uiImpulseResponse = new UiImpulseResponse(MenuCanvas);

        var uiHandler = new UiHandler(MenuCanvas, uiTimeEchogram, uiFrequencyEchogram, uiImpulseResponse, m_solver.Frequencies);

        uiHandler.InitializeUi(m_solver.Rays,
            m_solver.Microphones,
            m_solver.Echograms,
            m_solver.FrequencyResponse,
            m_solver.ImpulseResponses);

        m_rayMenu = new UiRayMenu();
        m_menuHandler = new UiMenuHandler();
        m_menuHandler.AddListenerForMenuButton(MenuCanvas);
        m_menuHandler.AddListenerForRayMenuButton(m_rayMenu.canvasMenu);
        m_rayMenu.AddListenerForShowButton(m_intersectedRayDrawer);
    }

    private void RunAlgorithm()
    {
        m_solver.RunSolver(m_audioSource.clip.name, InputHandler.GetNumber(m_configurationInput.numberOfReflections),
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
        m_intersectedRayDrawer.m_lines = m_intersectedLines;
        m_intersectedRayDrawer.m_rays = m_solver.Rays;
    }

    private void SetVisibilityForLoadingText()
    {
        Text loadingText = MenuCanvas.transform.Find("TabPanel").
                        gameObject.transform.Find("TabPanels").
                        gameObject.transform.Find("InputTabPanel").
                        gameObject.transform.Find("LoadingText").GetComponent<Text>();

        Color visibleColor = loadingText.color;
        visibleColor.a = 1 - visibleColor.a;
        loadingText.color = visibleColor;
    }


    //private void Pressed()
    //{
    //    if (Input.GetMouseButtonDown(0) == false)
    //        return;

    //    var clickedPosition = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

    //    Debug.Log("clicked: " + clickedPosition);
    //    //Debug.DrawLine(Camera.main.transform.position, clickedPosition);

    //    foreach (MicrophoneSphere microphone in m_microphones)
    //    {
    //        if (microphone.IsAroundMicro(VectorConverter.Convert(clickedPosition)) == true && SoundConvolver.convolvedSounds.Count > 0)
    //        {
    //            float[] f = SoundConvolver.convolvedSounds[microphone.id];
    //            var clip = AudioClip.Create("testSound", f.Length, 1, 44100, false, false);
    //            clip.SetData(f, 0);
    //            AudioSource.PlayClipAtPoint(clip, clickedPosition, 1.0f);
    //        }
    //    }
    //}

    private void AddListenerForSoundButton()
    {
        m_configurationInput.soundButton.onClick.AddListener(ShowDialog);
    }

    private void ShowDialog()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with wav", "", "wav");
        StartCoroutine(LoadAudioClip(path));
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
}
