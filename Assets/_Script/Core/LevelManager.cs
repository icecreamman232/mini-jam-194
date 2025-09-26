using System.Collections;
using System.Collections.Generic;
using SGGames.Script.Core;
using SGGames.Script.Events;
using SGGames.Script.UI;
using SGGames.Scripts.System;
using UnityEngine;

public class LevelManager : MonoBehaviour, IBootStrap, IGameService
{
    [SerializeField] private LevelGrade m_levelGrade;
    [Header("Events")] 
    [SerializeField] private RegisterEnemyEvent m_registerEnemyEvent;
    [SerializeField] private GameEvent m_gameEvent;
    [SerializeField] private LoadingScreenEvent m_loadingScreenEvent;
    [Header("Player Settings")]
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private Transform m_playerSpawnPoint;

    [Header("Level Settings")] 
    [SerializeField] private LevelContainer m_lvlEasyContainer;
    [SerializeField] private Transform m_levelSpawnPointParent;

    private HashSet<EnemyHealth> m_enemiesGroups = new HashSet<EnemyHealth>();
    private Transform m_player;
    private GameObject m_currentLevel;
    private LoadingScreenEventData m_loadingScreenEventData = new LoadingScreenEventData();
    
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
        InputManager.SetActive(false);
        
        yield return StartCoroutine(InstantiatePlayer());
        
        InputManager.SetActive(true);
        
        yield return StartCoroutine(CreateLevel());


        yield return new WaitForSeconds(1f);
        m_gameEvent.Raise(GameEventType.LevelStarted);
    }

    private IEnumerator OnLoadNextLevel()
    {
        InputManager.SetActive(false);

        m_loadingScreenEventData.TransitionType = TransitionType.RANDOM;
        m_loadingScreenEventData.LoadingType = LoadingScreenEventType.FadeIn;
        m_loadingScreenEvent.Raise(m_loadingScreenEventData);
        yield return new WaitForSeconds(LoadingScreenController.k_DefaultLoadingDuration);
        
        Destroy(m_currentLevel);
        yield return new WaitForEndOfFrame();
        m_player.position = m_playerSpawnPoint.position;
        yield return new WaitForEndOfFrame();
        
        yield return StartCoroutine(CreateLevel());
        
        m_loadingScreenEventData.TransitionType = TransitionType.RANDOM;       
        m_loadingScreenEventData.LoadingType = LoadingScreenEventType.FadeOut;
        m_loadingScreenEvent.Raise(m_loadingScreenEventData);
        var playerController = m_player.GetComponent<PlayerController>();
        playerController.UnFreeze();
        
        yield return new WaitForSeconds(LoadingScreenController.k_DefaultLoadingDuration);
        
        InputManager.SetActive(true);
        yield return new WaitForSeconds(1f);
        m_gameEvent.Raise(GameEventType.LevelStarted);
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
                m_gameEvent.Raise(GameEventType.OpenDoor);
            }
        }
    }
    
    
    private void OnReceiveGameEvent(GameEventType eventType)
    {
        if (eventType == GameEventType.LoadNextLevel)
        {
            Debug.Log("Load Next Level");
            StartCoroutine(OnLoadNextLevel());
        }
    }
}
