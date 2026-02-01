using System;
using System.Collections;
using System.Collections.Generic;
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
    Animator m_Animator;
    InputAction m_SprintAction;
    InputAction m_MoveAction;
    [SerializeField]
    float m_MovementSpeed = 5;
    float backwardsFactor = 0.5f;
    bool m_IsMoving = false;
    bool m_IsSprinting = false;

    [SerializeField]
    float m_RotationSpeed = 5;
    Quaternion m_TargetRotation;
    bool m_IsInteracting = false;
    Collider m_currentInteractTarget = null;
    InteractionData m_currentInteraction = null;
    [SerializeField]
    MaskData maskless = null;

    bool m_DebugInteractionBox = false;
    List<string> m_AttackAnimInteractions = new List<string>()
    {
        "attack",
        "having fun",
        "play with food",
    };

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
        m_Animator = GetComponentInChildren<Animator>();

        if (!model)
            model = transform;
        if (!playerCamera)
            playerCamera = Camera.main;
    }

    private void Start()
    {
        GameManager.GetInstance().playerMaxStamina = maxStamina;
    }

    void Update()
    {
        Movement();
        CameraPos();
        ScanInteraction(false);
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (m_IsMoving)
        {
            m_Animator.SetBool("IsMoving", true);
        }
        else
        {
            m_Animator.SetBool("IsMoving", false);
        }

        if (m_IsSprinting)
        {
            m_Animator.SetFloat("AnimSpeedMult", 3);
        }
        else
        {
            m_Animator.SetFloat("AnimSpeedMult", 1);
        }
    }

    void CameraPos()
    {
        playerCamera.transform.localPosition = m_cameraPos;
        playerCamera.transform.LookAt(transform.position + new Vector3(0, 1.2f, 0), Vector3.up);
    }

    void ScanInteraction(bool andActivate)
    {
        if (m_IsInteracting)
        {
            if (m_currentInteractTarget)
            {
                m_currentInteractTarget.GetComponent<IInteractable>().Highlight(false, null);
                m_currentInteractTarget = null;
            }
            return;
        }
        LayerMask mask = LayerMask.GetMask("Animal");
        Collider[] hits;
        hits = Physics.OverlapBox(m_InteractionCollider.transform.position, m_InteractionCollider.transform.localScale, m_InteractionCollider.transform.rotation, mask);
        if (hits.Length == 0)
        {
            if (m_currentInteractTarget)
            {
                m_currentInteractTarget.GetComponent<IInteractable>().Highlight(false, null);
                m_currentInteractTarget = null;
            }
            return;
        }

        int closest = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            var closestpos = hits[closest].transform.position;
            var hitpos = hits[i].transform.position;
            var targetpos = (m_InteractionCollider.transform.position + transform.position) * 0.5f;
            if (Vector3.Distance(hitpos, (m_InteractionCollider.transform.position + transform.position) * 0.5f) < Vector3.Distance(hits[closest].transform.position, transform.position))
                closest = i;
        }
        m_currentInteractTarget = hits[closest];
        m_currentInteraction = m_currentInteractTarget.GetComponent<IInteractable>().Interact(andActivate);

        if (!andActivate)
            m_currentInteractTarget.GetComponent<IInteractable>().Highlight(true, m_currentInteraction);
    }

    void Interaction(InputAction.CallbackContext context)
    {
        ScanInteraction(true);

        if (m_AttackAnimInteractions.Contains(m_currentInteraction?.interactionName.ToLower()))
        {
            m_Animator.SetTrigger("Kill");
        }

        if (m_currentInteraction == null)
        {
            return;
        }
        StartCoroutine(InteractionCooldown(m_currentInteraction.duration));
    }

    void Movement()
    {
        if (m_IsInteracting)
            return;

        Vector2 inputVal = m_MoveAction.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(inputVal.x, 0, inputVal.y);
        var sprintPressed = m_SprintAction.IsPressed();

        var forward = inputDirection.z;
        if (forward > 0)
        {
            m_IsMoving = true;
        }
        else
        {
            m_IsMoving = false;
        }

        var side = inputDirection.x;

        if (forward > 0)
        {
            m_Animator.SetBool("IsMoving", true);
        }
        else
        {
            m_Animator.SetBool("IsMoving", false);
        }

        if (Mathf.Abs(side) > 0)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * m_RotationSpeed * side);
        }

        if (Mathf.Abs(forward) > 0)
        {
            if (sprintPressed && currentStamina > Time.deltaTime * sprintCostStamina)
            {
                m_IsSprinting = true;

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
                m_IsSprinting = false;

                if (!sprintPressed)
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
            m_IsSprinting = false;

            if (currentStamina < maxStamina)
            {
                currentStamina += Time.deltaTime * staminaRegenPerSecond;
            }
            else
            {
                currentStamina = maxStamina;
            }
        }
        GameManager.GetInstance().playerCurStamina = currentStamina;


        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void TryAlertAnimals(MaskData currentMask, MaskData interactionMask = null)
    {
        if (!currentMask)
            currentMask = maskless;
        float visionRange = 200.0f;
        foreach (AnimalController animal in GameManager.GetInstance().m_Animals)
        {
            if (!animal.isAlive())
                continue;

            Ray ray = new Ray(transform.position, animal.headPosition.transform.position - transform.position);
            if (!Physics.Raycast(ray, visionRange, LayerMask.GetMask("Obstacle")))
            {
                animal.GetAlert(currentMask, interactionMask);
            }
        }
    }

    IEnumerator InteractionCooldown(float duration)
    {
        m_IsInteracting = true;
        TryAlertAnimals(GameManager.GetInstance().playerCurrentMask, m_currentInteraction.mask);

        yield return new WaitForSeconds(duration);
        if (m_currentInteraction.mask)
        {
            GameManager.GetInstance().playerFoundMasks.Add(m_currentInteraction.mask);
        }

        TryAlertAnimals(GameManager.GetInstance().playerCurrentMask, m_currentInteraction.mask);
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
