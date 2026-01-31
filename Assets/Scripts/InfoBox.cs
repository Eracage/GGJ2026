using UnityEngine;
using TMPro;

public class InfoBox : MonoBehaviour
{
    [SerializeField]
    GameObject m_InfoBox;
    [SerializeField]
    SpriteRenderer[] m_Icons;

    [SerializeField]
    TextMeshPro m_Text;
    [SerializeField]
    int m_IconCount = 0;

    public void AddIcon(Sprite icon)
    {
        m_Icons[m_IconCount].sprite = icon;
        m_Icons[m_IconCount].gameObject.SetActive(true);
        m_IconCount++;
    }

    public void ShowIcons(Sprite icon)
    {
        m_Text.gameObject.SetActive(false);
        for (int i = 0; i < m_IconCount; i++)
        {
            m_Icons[i].gameObject.SetActive(true);
        }
        m_InfoBox.SetActive(true);
    }
    public void ShowText(string text)
    {
        foreach (var sr in m_Icons)
        {
            sr.gameObject.SetActive(false);
        }
        m_Text.text = text;
        m_Text.gameObject.SetActive(true);
        m_InfoBox.SetActive(true);
    }
    public void Hide()
    {
        m_Text.gameObject.SetActive(false);
        foreach (var sr in m_Icons)
        {
            sr.gameObject.SetActive(false);
        }
        m_InfoBox.SetActive(false);
    }
}
