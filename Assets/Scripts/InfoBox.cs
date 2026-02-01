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

    [SerializeField]
    MaskData masklessPlayer;

    private void Start()
    {
        Hide();
    }

    public void setAnimals(Sprite animalSprite)
    {
        foreach (var mask in m_Icons)
        {
            setAnimal(mask, animalSprite);
        }
    }

    public void setAnimal(SpriteRenderer parentSR, Sprite sprite)
    {
        var spriteRenderer = parentSR.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public void AddMaskless()
    {
        setAnimal(m_Icons[m_IconCount], masklessPlayer.playerMask);
        m_Icons[m_IconCount].sprite = masklessPlayer.defaultMask;
        m_Icons[m_IconCount].gameObject.SetActive(true);
        m_IconCount++;
    }

    public void AddIcon(Sprite icon, Color color)
    {
        m_Icons[m_IconCount].sprite = icon;
        m_Icons[m_IconCount].gameObject.SetActive(true);
        m_Icons[m_IconCount].color = color;
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
        Debug.Log("Show Text: " + text);
        m_iconView.SetActive(false);

        m_Text.text = text;
        m_Text.gameObject.SetActive(true);
        m_InfoBox.SetActive(true);
    }
    public void Hide()
    {
        Debug.Log("Hide Text");
        m_iconView.SetActive(false);

        m_Text.gameObject.SetActive(false);
        m_InfoBox.SetActive(false);
    }
}
