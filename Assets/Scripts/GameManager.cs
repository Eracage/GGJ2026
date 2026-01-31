using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<AnimalController> m_Animals;

    private static GameManager m_Instance;

    void Start()
    {
        CreateInstance();
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

    void Update()
    {
    }

    public static GameManager GetInstance()
    {
        return m_Instance;
    }

}
