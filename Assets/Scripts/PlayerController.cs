using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    BoxCollider m_InteractionCollider;

    CharacterController m_CharacterController;
    InputAction m_MoveAction;
    [SerializeField]
    float m_MovementSpeed = 5;
    [SerializeField]
    float m_RotationSpeed = 5;
    Quaternion m_TargetRotation;
    bool m_IsInteracting = false;
    [SerializeField]
    float m_InteractionZoneSize = 2;
        [SerializeField]
    float m_InteractionDistance = 2;

    bool m_DebugInteractionBox = false;

    void Awake()
    {   
        m_InteractionCollider.enabled = false;
        InputSystem.actions.FindAction("Interact", true).started += Interaction;
        m_MoveAction = InputSystem.actions.FindAction("Move", true);
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }

    void Interaction(InputAction.CallbackContext context)
    {
        LayerMask mask = LayerMask.GetMask("Animal");
        Collider[] hits;
        hits = Physics.OverlapBox(m_InteractionCollider.transform.position,m_InteractionCollider.transform.localScale,m_InteractionCollider.transform.rotation, mask);
        if(hits.Length == 0)
            return;
        
        int closest = 0;
        for(int i =0; i < hits.Length; i++)
        {
            if(Vector3.Distance(hits[i].transform.position, transform.position) < Vector3.Distance(hits[closest].transform.position, transform.position))
                closest = i;
        }

        InteractionData data = hits[closest].gameObject.GetComponent<AnimalController>().Interact();
        StartCoroutine(InteractionCooldown(data.duration));
    }

    void Movement()
    {
        if(m_IsInteracting)
            return;

        Vector2 inputVal = m_MoveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputVal.x, 0, inputVal.y);
        
        if(direction.magnitude > 0)
        {
            m_TargetRotation.SetLookRotation(direction,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
        }
        
        
        m_CharacterController.Move(direction* m_MovementSpeed * Time.deltaTime);
        
    }

    IEnumerator InteractionCooldown(float duration)
    {
        m_IsInteracting = true;
        yield return new WaitForSeconds(duration);
        m_IsInteracting = false;
    }

    IEnumerator InteractionDebugCooldown()
    {
        m_DebugInteractionBox = true;
        yield return new WaitForSeconds(0.3f);
        m_DebugInteractionBox = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //m_InteractionCollider.center,m_InteractionCollider.bounds.extents,m_InteractionCollider.transform.rotation, mask
        Gizmos.DrawWireCube(m_InteractionCollider.transform.position, m_InteractionCollider.transform.localScale);
        
    }

}
