using System;
using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Toxic Debuff Event", menuName = "SGGames/Toxic Debuff Event")]
public class ToxicDebuffEvent : ScriptableEvent<ToxicDebuffData>
{
    
}

[Serializable]
public class ToxicDebuffData
{
    public ToxicModifierID ID;
    public string Name;
    public string Description;
}
