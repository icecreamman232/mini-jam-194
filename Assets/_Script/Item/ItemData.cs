using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "SGGames/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemID ItemID;
    public int Price;
    public Sprite Icon;
    public string Name;
    [TextArea(3,10)]
    public string Description;
    public float ToxicPoint;
    public ModifierData[] Modifiers;
}

public enum ItemID
{
    PenetratedBullet,
    RicochetBullet,
    StabilizerBarrel,
    StabilizerStock,
    DestroyerBarrel,
}

[Serializable]
public class ModifierData
{
    public ModifierType Type;
    public float Value;
}

public enum ModifierType
{
    WeaponRecoil,
    WeaponAccuracy,
    ChangeTargetMask,
}
