using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [SerializeField]
    float m_MaxRotation = 360;
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, Random.Range(0, m_MaxRotation)));
    }
}
