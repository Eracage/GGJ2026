using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<AnimalController> m_Animals;

    private static GameManager m_Instance;

    public List<MaskData> playerFoundMasks;
    public MaskData playerCurrentMask;
    public float playerMaxStamina;
    public float playerCurStamina;

    public float startTime;

    public List<Gate> finishGates = new List<Gate>();

    public int totalKills = 0;
    public bool lastLevelWon = false;

    void Start()
    {
        CreateInstance();
        ResetLevel();
    }
    void Awake()
    {
        CreateInstance();
    }

    void CreateInstance()
    {
        if(GameManager.GetInstance())
            Destroy(this);
        m_Instance = this;
        DontDestroyOnLoad(this);
        m_Animals = new List<AnimalController>();
    }

    public void ResetLevel()
    {
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
            
            totalKills = m_Animals.Count;
        }
    }

    public static GameManager GetInstance()
    {
        return m_Instance;
    }

}
