using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "SGGames/Item Data")]
public class ItemData : ScriptableObject
{
    public int Price;
    public Sprite Icon;
    public string Name;
    [TextArea(3,10)]
    public string Description;
}
