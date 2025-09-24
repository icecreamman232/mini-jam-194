using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Reload Event", menuName = "SGGames/Player Reload Event")]
public class PlayerReloadEvent : ScriptableEvent<ReloadEventData>
{
    
}

public class ReloadEventData
{
    public float CurrentReloadTime;
    public float MaxReloadTime;
}
