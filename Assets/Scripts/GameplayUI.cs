using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameplayUI : MonoBehaviour
{
    [SerializeField]
    Sprite m_SlotBaseSprite;
    [SerializeField]
    Sprite m_SlotSelectedSprite;

    [SerializeField]
    Image[] m_Slots;
    [SerializeField]
    Image[] m_Faces;
    [SerializeField]
    Image[] m_Masks;

    [SerializeField]
    GameObject m_MaskPanel;
    int m_SelectedIndex = 2;

    void Start()
    {
        InputSystem.actions.FindAction("Next", true).started += SelectNext;
        InputSystem.actions.FindAction("Previous", true).started += SelectPrevious;
        m_Slots[m_SelectedIndex].sprite = m_SlotSelectedSprite;

    }

    void SelectNext(InputAction.CallbackContext context)
    {
        if (m_SelectedIndex < 4)
        {
            m_Slots[m_SelectedIndex].sprite = m_SlotBaseSprite;
            m_SelectedIndex++;
            m_Slots[m_SelectedIndex].sprite = m_SlotSelectedSprite;
        }
    }
    void SelectPrevious(InputAction.CallbackContext context)
    {
        if (m_SelectedIndex > 0)
        {
            m_Slots[m_SelectedIndex].sprite = m_SlotBaseSprite;
            m_SelectedIndex--;
            m_Slots[m_SelectedIndex].sprite = m_SlotSelectedSprite;
        }
    }

    public void Show()
    {
        m_MaskPanel.SetActive(true);
    }
    public void Hide()
    {
        m_MaskPanel.SetActive(false);
    }
}
