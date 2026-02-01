using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeTrigger : MonoBehaviour
{
    [SerializeField]
    string m_NextLevel;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GetInstance().ResetLevel();
            SceneManager.LoadScene(m_NextLevel);
        }
    }
}
