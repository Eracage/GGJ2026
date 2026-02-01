using UnityEngine;

[CreateAssetMenu(fileName = "InteractionData", menuName = "Scriptable Objects/InteractionData")]
public class InteractionData : ScriptableObject
{
    public string interactionName = "Interacting";
    public float duration = 1;
    public MaskData mask = null;
}
