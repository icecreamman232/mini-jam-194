using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Register Enemy Event", menuName = "SGGames/Register Enemy Event")]
public class RegisterEnemyEvent : ScriptableEvent<RegisteredEnemyData>
{
    
}

public enum EnemyState
{
    Alive,
    Dead,
}

public class RegisteredEnemyData
{
    public EnemyState State;
    public EnemyHealth EnemyHealth;
}
