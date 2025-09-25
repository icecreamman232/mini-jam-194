using SGGames.Script.Core;
using UnityEngine;

public class LevelManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private Transform m_player;
    
    public Transform Player => m_player;
    
    public void Install()
    {
        ServiceLocator.RegisterService<LevelManager>(this);
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<LevelManager>();
    }
}
