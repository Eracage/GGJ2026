using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Scriptable Objects/MaskData")]
public class MaskData : ScriptableObject
{
    public uint UID = 1;
    public Sprite defaultMask;
    public bool onSheep;
    public bool onPig;
    public bool onRabbit;
    public bool onWolf;
    public Sprite playerMask;
    public Color color = Color.white;
}
