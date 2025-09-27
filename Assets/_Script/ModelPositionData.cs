using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Model Position Data", menuName = "SGGames/Model Position Data")]
public class ModelPositionData : ScriptableObject
{
    public ModelData[] ModelDataList;
}

[Serializable]
public class ModelData
{
    public ModelType Type;
    public ItemID ItemID;
    public Sprite Model;
    public Vector3 Position;
}

public enum ModelType
{
    Barrel,
    Stock,
    Gun
}
