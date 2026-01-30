using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    CharacterController m_CharacterController;
    InputAction m_MoveAction;
    [SerializeField]
    float m_MovementSpeed = 5;
    [SerializeField]
    float m_RotationSpeed = 5;
    Quaternion m_TargetRotation;
    bool m_IsInteracting = false;


    void Awake()
    {
        m_MoveAction = InputSystem.actions.FindAction("Move", true);
        m_CharacterController = GetComponent<CharacterController>();        
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Vector2 inputVal = m_MoveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputVal.x, 0, inputVal.y);
        
        if(direction.magnitude > 0)
        {
            m_TargetRotation.SetLookRotation(direction,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
        }
        
        if(!m_IsInteracting)
            m_CharacterController.Move(direction* m_MovementSpeed * Time.deltaTime);
        
    }
}
