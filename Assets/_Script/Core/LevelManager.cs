using System.Collections;
using SGGames.Script.Core;
using SGGames.Scripts.System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private GameEvent m_gameEvent;
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private Transform m_playerSpawnPoint;
    private Transform m_player;
    
    public Transform Player => m_player;
    
    public void Install()
    {
        ServiceLocator.RegisterService<LevelManager>(this);
        m_gameEvent.AddListener(OnReceiveGameEvent);


        StartCoroutine(OnLoadFirstLevel());
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<LevelManager>();
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }

    private IEnumerator OnLoadFirstLevel()
    {
        yield return StartCoroutine(InstantiatePlayer());
    }

    private IEnumerator InstantiatePlayer()
    {
        var playerObject = Instantiate(m_playerPrefab, m_playerSpawnPoint.position, Quaternion.identity);
        m_player = playerObject.transform;
        yield return new WaitForEndOfFrame();
        ServiceLocator.GetService<CameraController>().SetTarget(m_player);
        m_gameEvent.Raise(GameEventType.CreatedPlayer);
        yield return new WaitForSeconds(2f); //Wait for player appear animation
    }
    
    private void OnReceiveGameEvent(GameEventType eventType)
    {
        
    }
}
