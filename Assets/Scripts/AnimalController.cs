using System.Collections;
using UnityEngine;

public class AnimalController : MonoBehaviour, IInteractable
{   
    [SerializeField]
    InteractionData m_InteractionData;

    public Material testNormalMaterial;
    public Material testInteractMaterial;

    void Awake()
    {

    }

    void Update()
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

}
