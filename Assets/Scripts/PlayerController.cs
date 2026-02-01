using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    BoxCollider m_InteractionCollider;

    [SerializeField]
    Vector3 m_cameraPos = new Vector3(0.0f, 2.0f, -3.0f);

    CharacterController m_CharacterController;
	InputAction m_SprintAction;
	InputAction m_MoveAction;
    [SerializeField]
    float m_MovementSpeed = 5;
    float backwardsFactor = 0.5f;

    [SerializeField]
    float m_RotationSpeed = 5;
    Quaternion m_TargetRotation;
    bool m_IsInteracting = false;

    bool m_DebugInteractionBox = false;


    public float sprintFactor = 2f;
	public float maxStamina = 100;
	
    public float staminaRegenPerSecond = 10;
	public float staminaRegenFactorWhenWalking = 0.25f;
	public float sprintCostStamina = 20;	
	public float currentStamina = 100;


	public Transform model;
    public Camera playerCamera;

    void Awake()
    {
        m_InteractionCollider.enabled = false;
        InputSystem.actions.FindAction("Interact", true).started += Interaction;
        m_MoveAction = InputSystem.actions.FindAction("Move", true);
		m_SprintAction = InputSystem.actions.FindAction("Sprint", true);
		m_CharacterController = transform.GetComponent<CharacterController>();

        if (!model)
            model = transform;
        if (!playerCamera)
            playerCamera = Camera.main;
    }

    void Update()
    {
        Movement();
        CameraPos();
    }

    void CameraPos()
    {
        playerCamera.transform.localPosition = m_cameraPos;
        playerCamera.transform.LookAt(transform.position + new Vector3(0, 1.2f, 0),Vector3.up);
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
            var closestpos = hits[closest].transform.position;
            var hitpos = hits[i].transform.position;
            var targetpos = (m_InteractionCollider.transform.position + transform.position) * 0.5f;
            if (Vector3.Distance(hitpos, (m_InteractionCollider.transform.position + transform.position) * 0.5f) < Vector3.Distance(hits[closest].transform.position, transform.position))
                closest = i;
        }

        InteractionData data = hits[closest].gameObject.GetComponent<IInteractable>().Interact();
        StartCoroutine(InteractionCooldown(data.duration));
    }

    void Movement()
    {
		if (m_IsInteracting)
			return;

		Vector2 inputVal = m_MoveAction.ReadValue<Vector2>();
		Vector3 inputDirection = new Vector3(inputVal.x, 0, inputVal.y);
        var isSprinting = m_SprintAction.IsPressed();

		var forward = inputDirection.z;
		var side = inputDirection.x;


		if (Mathf.Abs(side) > 0)
		{
			transform.Rotate(Vector3.up, Time.deltaTime * m_RotationSpeed * side);
            
		}		

        if (Mathf.Abs(forward) > 0)
        {

            if (isSprinting && currentStamina > Time.deltaTime * sprintCostStamina)
            {
                currentStamina -= Time.deltaTime * sprintCostStamina;
                if (forward < 0)
                {
					m_CharacterController.Move(transform.forward * m_MovementSpeed * Time.deltaTime * forward * sprintFactor * backwardsFactor);
				}
                else
                {
					m_CharacterController.Move(transform.forward * m_MovementSpeed * Time.deltaTime * forward * sprintFactor);
				}
                    
			}
            else
            {
                if (!isSprinting)
                {
                    if (currentStamina < maxStamina)
                    {
                        currentStamina += Time.deltaTime * staminaRegenPerSecond * staminaRegenFactorWhenWalking;
                    }
                    else
                    {
                        currentStamina = maxStamina;
                    }
                }
				if (forward < 0)
				{
					m_CharacterController.Move(transform.forward * m_MovementSpeed * Time.deltaTime * forward * backwardsFactor);
				}
				else
				{
					m_CharacterController.Move(transform.forward * m_MovementSpeed * Time.deltaTime * forward);
				}
			}
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += Time.deltaTime * staminaRegenPerSecond;
            }
            else
            {
                currentStamina = maxStamina;
            }
        }

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
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
