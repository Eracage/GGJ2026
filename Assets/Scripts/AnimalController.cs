using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalController : MonoBehaviour, IInteractable
{
    public Sprite headSprite;
    public GameObject headPosition;
    public SpriteRenderer headPrefab;
    public MaskData mask = null;
    public GameObject goreEffect;
    GameObject m_goreEffect;
    public GameObject model;

    enum State
    {
        Loitering,
        Wandering,
        Informing,
        Interacting,
        Attacing,
        Lootable,
        Deceased,
    }
    [Header("General")]
    State m_CurrentState = State.Loitering;
    [SerializeField]
    InteractionData m_InteractionData;
    [SerializeField]
    InteractionData m_interactAttack;
    [SerializeField]
    InteractionData m_interactMask;
    [SerializeField]
    InteractionData m_interactPlay;
    NavMeshAgent m_NavAgent;
    Vector3 m_TargetPosition;

    [Header("Loitering")]
    [SerializeField]
    float m_MaxLoiterCooldown = 10;
    [SerializeField]
    float m_MinLoiterCooldown = 2;
    [SerializeField]
    float m_MaxLoiterRange = 5;
    float m_NextLoiterTime = 0;
    [Space]
    [Header("Wandering")]
    [SerializeField]
    float m_MinWanderDelay = 7;
    [SerializeField]
    float m_MaxWanderDelay = 30;
    [SerializeField]
    int m_WanderStuckTimeout = 10;
    [SerializeField]
    float m_WanderTimeForReturn = 15.0f;

    float m_NextWanderDelay;
    Vector3 m_WanderStart;
    [Space]
    [Header("Debug")]
    public bool debugMaterialChange = false;
    public Material testNormalMaterial;
    public Material testInteractMaterial;

    void Start()
    {
        GameManager.GetInstance().m_Animals.Add(this);
        m_NavAgent = GetComponent<NavMeshAgent>();

        m_TargetPosition = transform.position;
        m_NextLoiterTime = Random.Range(0, m_MaxLoiterCooldown);
        m_NextWanderDelay = Random.Range(m_MinWanderDelay, m_MaxWanderDelay);
        StartCoroutine(StartWander(m_NextWanderDelay));
    }

    void Update()
    {
        switch (m_CurrentState)
        {
            case State.Loitering:
                LoiterUpdate();
                break;
            case State.Wandering:
                WanderUpdate();
                break;
            case State.Informing:
                InformingUpdate();
                break;
            case State.Interacting:
                InteractionUpdate();
                break;
            case State.Attacing:
                break;
            case State.Lootable:
                break;
            case State.Deceased:
                break;

        }
    }

    void InteractionUpdate()
    {

    }

    void LoiterUpdate()
    {
        if (Time.time < m_NextLoiterTime)
            return;

        //Debug.Log("Animal " + gameObject.name + " loitering!");

        //If far from animal go to animal else move to random
        float closestDistance = float.PositiveInfinity;
        int closest = 0;
        for (int i = 0; i < GameManager.GetInstance().m_Animals.Count; i++)
        {
            if (GameManager.GetInstance().m_Animals[i] == this)
                continue;
            float distance = Vector3.Distance(GameManager.GetInstance().m_Animals[i].transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = i;
            }
        }
        if (closestDistance > m_MaxLoiterRange)
        {
            m_NextLoiterTime = Time.time + Random.Range(m_MinLoiterCooldown, m_MaxLoiterCooldown);
            m_TargetPosition = transform.position - (transform.position - GameManager.GetInstance().m_Animals[closest].transform.position);
            SetNavDest(m_TargetPosition);
            Debug.Log("Animal " + gameObject.name + " is too far away and needs to return to back!");
            return;
        }

        Vector3 randomPos = new Vector3(Random.Range(1.0f, 3.5f) * ((Random.Range(0, 2) < 1) ? -1 : 1), 0, Random.Range(1.0f, 3.5f) * ((Random.Range(0, 2) < 1) ? -1 : 1));
        m_TargetPosition = transform.position + randomPos;
        m_NextLoiterTime = Time.time + Random.Range(m_MinLoiterCooldown, m_MaxLoiterCooldown);
        SetNavDest(m_TargetPosition);
    }
    void WanderUpdate()
    {

    }

    void InformingUpdate()
    {

    }

    public InteractionData AvailableInteraction(bool activated = false)
    {
        switch (m_CurrentState)
        {
            case State.Loitering:
            case State.Wandering:
            case State.Informing:
            case State.Interacting:
            case State.Attacing:
                if (activated)
                {
                    m_NavAgent.enabled = false;
                    StartCoroutine(TestChangeColor());
                    if (mask)
                    {
                        StartCoroutine(Attack(State.Lootable));
                    }
                    else
                    {
                        StartCoroutine(Attack(State.Deceased));
                    }
                }
                return m_interactAttack;
            case State.Lootable:
                if (activated)
                {
                    if (mask)
                    {
                        StartCoroutine(Attack(State.Deceased));
                        m_interactMask.mask = mask;
                    }
                    else
                    {
                        Debug.Log("Lootable without mask, why not Deceased?", this);
                    }
                }
                return m_interactMask;
            case State.Deceased:
                if (activated)
                {
                    StartCoroutine(Attack(State.Deceased));
                }
                return m_interactPlay;
        }
        if (activated)
        {
            StartCoroutine(TestChangeColor());
        }
        Debug.Log("Unhandled state: " + m_CurrentState.ToString(), this);
        return m_InteractionData;
    }

    public InteractionData Interact()
    {
        return AvailableInteraction(true);
    }

    IEnumerator Unmask()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator Attack(State nextState)
    {
        m_CurrentState = State.Interacting;
        yield return new WaitForSeconds(2);
        BloodSplatter();
        m_CurrentState = nextState;
    }

    void BloodSplatter()
    {
        if (model)
        {
            GameObject.DestroyImmediate(model);
        }
        GetComponent<Collider>().isTrigger = true;
        GameObject.Destroy(m_goreEffect, 1);
        m_goreEffect = Instantiate(goreEffect, transform.position, Quaternion.identity, null);
    }

    IEnumerator TestChangeColor()
    {
        if (!debugMaterialChange)
            yield break;

        if (model)
            model.GetComponent<MeshRenderer>().material = testInteractMaterial;

        yield return new WaitForSeconds(3.0f);

        if (model)
            model.GetComponent<MeshRenderer>().material = testInteractMaterial;
    }

    IEnumerator StartWander(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m_CurrentState != State.Loitering)
            yield break;

        Debug.Log("Animal " + gameObject.name + " started wandering!");
        m_CurrentState = State.Wandering;
        m_WanderStart = transform.position;

        float longestDistance = float.PositiveInfinity;
        int longest = 0;
        for (int i = 0; i < GameManager.GetInstance().m_Animals.Count; i++)
        {
            if (GameManager.GetInstance().m_Animals[i] == this)
                continue;
            float distance = Vector3.Distance(GameManager.GetInstance().m_Animals[i].transform.position, transform.position);
            if (distance > longestDistance)
            {
                longestDistance = distance;
                longest = i;
            }
        }
        m_TargetPosition = transform.position - ((transform.position - GameManager.GetInstance().m_Animals[longest].transform.position) / 3);
        SetNavDest(m_TargetPosition);
        System.TimeSpan timeout = new System.TimeSpan(0, 0, 10);
        yield return new WaitUntil(IsOnTarget, timeout, ReturnFromWander);
        if (m_CurrentState != State.Loitering)
            yield break;
        ReturnFromWander();
    }

    void SetNavDest(Vector3 destination)
    {
        if (m_NavAgent && m_NavAgent.enabled)
            m_NavAgent.SetDestination(m_TargetPosition);
    }

    void ReturnFromWander()
    {
        if (m_CurrentState != State.Wandering)
            return;

        Debug.Log("Animal " + gameObject.name + " is returning from wandering!");
        m_NextLoiterTime = Time.time + m_WanderTimeForReturn;
        SetNavDest(m_WanderStart);
        m_CurrentState = State.Loitering;
    }

    bool IsOnTarget()
    {
        return Vector3.Distance(transform.position, m_TargetPosition) < 1.0f;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, m_TargetPosition);
    }

}