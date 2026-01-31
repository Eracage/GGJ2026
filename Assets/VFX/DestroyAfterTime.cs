using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    float m_Time;
    void Start()
    {
        Destroy(gameObject, m_Time);
    }
}
