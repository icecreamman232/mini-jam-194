using System.Collections;
using System.Collections.Generic;
using SGGames.Script.Core;
using SGGames.Scripts.System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private LevelGrade m_levelGrade;
    [Header("Events")] 
    [SerializeField] private RegisterEnemyEvent m_registerEnemyEvent;
    [SerializeField] private GameEvent m_gameEvent;
    [Header("Player Settings")]
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private Transform m_playerSpawnPoint;

    [Header("Level Settings")] 
    [SerializeField] private LevelContainer m_lvlEasyContainer;
    [SerializeField] private Transform m_levelSpawnPointParent;

    private HashSet<EnemyHealth> m_enemiesGroups = new HashSet<EnemyHealth>();
    private Transform m_player;
    private GameObject m_currentLevel;
    
    public Transform Player => m_player;
    
    public void Install()
    {
        ServiceLocator.RegisterService<LevelManager>(this);
        m_gameEvent.AddListener(OnReceiveGameEvent);
        m_registerEnemyEvent.AddListener(OnReceiveEnemyRegister);


        StartCoroutine(OnLoadFirstLevel());
    }
    
    public void Uninstall()
    {
        ServiceLocator.UnregisterService<LevelManager>();
        m_registerEnemyEvent.RemoveListener(OnReceiveEnemyRegister);
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }

    private IEnumerator OnLoadFirstLevel()
    {
        yield return StartCoroutine(InstantiatePlayer());
        yield return StartCoroutine(CreateLevel());
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

    private IEnumerator CreateLevel()
    {
        LevelContainer levelContainer = null;
        if (m_levelGrade == LevelGrade.Easy)
        {
            levelContainer = m_lvlEasyContainer;
        }

        if (levelContainer == null)
        {
            Debug.LogError("LevelContainer is null");
            yield break;       
        }
        
        m_currentLevel = Instantiate(levelContainer.GetRandomLevelPrefab(), m_levelSpawnPointParent);
        yield return new WaitForEndOfFrame();
        m_gameEvent.Raise(GameEventType.CreatedLevel);
    }
    
    private void OnReceiveEnemyRegister(RegisteredEnemyData registerData)
    {
        if (registerData.State == EnemyState.Alive)
        {
            m_enemiesGroups.Add(registerData.EnemyHealth);
        }
        else if (registerData.State == EnemyState.Dead)
        {
            m_enemiesGroups.Remove(registerData.EnemyHealth);
            if (m_enemiesGroups.Count == 0)
            {
                //Room is cleared
                Debug.Log("Room is cleared");
            }
        }
    }
    
    
    private void OnReceiveGameEvent(GameEventType eventType)
    {
        
    }
}
