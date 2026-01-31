using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalController : MonoBehaviour, IInteractable
{
    enum State
    {
        Loitering,
        Wandering,
        Informing,
        Interacting,
        Attacing,
        Deceased
    }
    [Header("General")]
    State m_CurrentState = State.Loitering;
    [SerializeField]
    InteractionData m_InteractionData;
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
            m_NavAgent.SetDestination(m_TargetPosition);
            Debug.Log("Animal " + gameObject.name + " is too far away and needs to return to back!");
            return;
        }

        Vector3 randomPos = new Vector3(Random.Range(1.0f, 3.5f) * ((Random.Range(0, 2) < 1) ? -1 : 1), 0, Random.Range(1.0f, 3.5f) * ((Random.Range(0, 2) < 1) ? -1 : 1));
        m_TargetPosition = transform.position + randomPos;
        m_NextLoiterTime = Time.time + Random.Range(m_MinLoiterCooldown, m_MaxLoiterCooldown);
        m_NavAgent.SetDestination(m_TargetPosition);
    }
    void WanderUpdate()
    {

    }
    void InformingUpdate()
    {

    }
    public InteractionData Interact()
    {
        StartCoroutine(TestChangeColor());
        return m_InteractionData;
    }

    IEnumerator TestChangeColor()
    {
        GetComponent<MeshRenderer>().material = testInteractMaterial;
        yield return new WaitForSeconds(3.0f);
        GetComponent<MeshRenderer>().material = testNormalMaterial;
    }

    IEnumerator StartWander(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m_CurrentState != State.Loitering)
            yield return null;

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
        m_NavAgent.SetDestination(m_TargetPosition);
        System.TimeSpan timeout = new System.TimeSpan(0, 0, 10);
        yield return new WaitUntil(IsOnTarget, timeout, ReturnFromWander);
        ReturnFromWander();
    }

    void ReturnFromWander()
    {
        Debug.Log("Animal " + gameObject.name + " is returning from wandering!");
        m_NextLoiterTime = Time.time + m_WanderTimeForReturn;
        m_NavAgent.SetDestination(m_WanderStart);
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