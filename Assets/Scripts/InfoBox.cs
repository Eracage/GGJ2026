using UnityEngine;
using TMPro;

public class InfoBox : MonoBehaviour
{
    [SerializeField]
    GameObject m_iconView;
    [SerializeField]
    SpriteRenderer[] m_Icons;

    [SerializeField]
    GameObject m_InfoBox;
    [SerializeField]
    TextMeshPro m_Text;
    [SerializeField]
    int m_IconCount = 0;

    private void Start()
    {
        Hide();
    }

    public void AddIcon(Sprite icon)
    {
        m_Icons[m_IconCount].sprite = icon;
        m_Icons[m_IconCount].gameObject.SetActive(true);
        m_IconCount++;
    }

    public void ShowIcons()
    {
        m_Text.gameObject.SetActive(false);
        m_InfoBox.SetActive(false);

        m_iconView.SetActive(true);
    }
    public void ShowText(string text)
    {
        m_iconView.SetActive(false);

        m_Text.text = text;
        m_Text.gameObject.SetActive(true);
        m_InfoBox.SetActive(true);
    }
    public void Hide()
    {
        m_iconView.SetActive(false);

        m_Text.gameObject.SetActive(false);
        m_InfoBox.SetActive(false);
    }
}
