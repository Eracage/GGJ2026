using UnityEngine;

public class SpawnObject : MonoBehaviour
{

    [SerializeField]
    Transform m_Location;
    [SerializeField]
    GameObject m_Object;
    public void Spawn()
    {
        Instantiate(m_Object, m_Location);
    }
}
