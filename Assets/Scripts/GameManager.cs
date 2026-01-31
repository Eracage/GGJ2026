using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;

    void Awake()
    {
        if(GameManager.GetInstance())
            Destroy(this);

        m_instance = this;
    }

    
    void Update()
    {
        
    }

    public static GameManager GetInstance()
    {
        return m_instance;
    }

}
