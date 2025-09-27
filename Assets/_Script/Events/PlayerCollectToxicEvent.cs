using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Collect Toxic Event", menuName = "SGGames/Player Collect Toxic Event")]
public class PlayerCollectToxicEvent : ScriptableEvent<ToxicEventData>
{
    
}

public class ToxicEventData
{
    public int ToxicLevel;
    public float CurrentToxic;
    public float MaxToxic;
}
