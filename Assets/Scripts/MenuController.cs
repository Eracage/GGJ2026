using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject m_MenuPanel;
    [SerializeField]
    GameObject m_CreditsPanel;

    void Start()
    {

    }
    void Update()
    {

    }

    public void GoToCredits()
    {
        m_MenuPanel.SetActive(false);
        m_CreditsPanel.SetActive(true);
    }
    public void GoToMenu()
    {
        m_MenuPanel.SetActive(true);
        m_CreditsPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
