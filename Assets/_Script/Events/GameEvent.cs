using SGGames.Script.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Event", menuName = "SGGames/Game Event")]
public class GameEvent : ScriptableEvent<GameEventType>
{
    
}

public enum GameEventType
{
    CreatedPlayer,
}
