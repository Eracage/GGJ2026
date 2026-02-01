using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_Text;

    void Start()
    {
        if (GameManager.GetInstance())
        {
            m_Text.text = "You win!";
        }
        else
        {
            m_Text.text = "You lose!";
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
