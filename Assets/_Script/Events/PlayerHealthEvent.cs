using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Health Event", menuName = "SGGames/Player Health Event")]
public class PlayerHealthEvent : ScriptableEvent<HealthEventData>
{
    
}

public class HealthEventData
{
    public float CurrentHealth;
    public float MaxHealth;
}
