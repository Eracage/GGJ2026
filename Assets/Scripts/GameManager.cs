using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<AnimalController> m_Animals;

    private static GameManager m_Instance = null;

    public List<MaskData> playerFoundMasks;
    public MaskData playerCurrentMask;
    public float playerMaxStamina;
    public float playerCurStamina;

    public float startTime;

    public List<Gate> finishGates = new List<Gate>();

    public int totalKills = 0;
    public bool lastLevelWon = false;
    AudioClipPlayer audioClipPlayer = null;
    bool gateAudioPlayed = false;

    void Awake()
    {
        Debug.Log("Start");
        CreateInstance();
        m_Animals = new List<AnimalController>();
        finishGates = new List<Gate>();
        ResetLevel();
    }

    private void Start()
    {
        audioClipPlayer = GetComponent<AudioClipPlayer>();
    }

    void CreateInstance()
    {
        Debug.Log("CreateInstance1");
        if (GameManager.GetInstance() == null)
        {
        Debug.Log("CreateInstance2");
            DontDestroyOnLoad(gameObject);
            m_Instance = this;
            return;
        }
        Debug.Log("CreateInstance3");
        DestroyImmediate(gameObject);
        Debug.Log("CreateInstance4");
    }

    public void ResetLevel()
    {
        audioClipPlayer.audioSource.Stop();
        gateAudioPlayed = false;
        finishGates.Clear();
        lastLevelWon = AnimalCount() == 0;
        startTime = Time.time;
        m_Animals.Clear();
    }

    int AnimalCount()
    {
        int animalsAlive = 0;
        foreach (var animal in m_Animals)
            if (animal.isAlive())
                animalsAlive++;
        return animalsAlive;
    }

    void Update()
    {
        if (Time.time < startTime + 20)
            return;

        if (AnimalCount() == 0)
        {
            foreach (var gate in finishGates)
            {
                gate.openingState = OpeningStateEnum.Opening;
            }
            if (!gateAudioPlayed)
            {
                audioClipPlayer.audioSource.Play();
                gateAudioPlayed = true;
            }
            
            totalKills = m_Animals.Count;
        }
    }

    public static GameManager GetInstance()
    {

        Debug.Log("GetInstance");
        return m_Instance;
    }

}
