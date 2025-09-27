using UnityEngine;

public class EnemyController : MonoBehaviour
{
   [SerializeField] private EnemyHealth m_health;
   [SerializeField] private EnemyMovement m_movement;
   [SerializeField] private EnemyAI m_ai;
   [SerializeField] private SpriteRenderer m_model;
   [SerializeField] private GameEvent m_gameEvent;

   private void Awake()
   {
      m_gameEvent.AddListener(OnReceiveGameEvent);
      var currentColor = m_model.color;
      currentColor.a = 0.5f;
      m_model.color = currentColor;
   }

   private void OnDestroy()
   {
      m_gameEvent.RemoveListener(OnReceiveGameEvent);
   }
   
   private void OnReceiveGameEvent(GameEventType gameEventType)
   {
      if (gameEventType == GameEventType.LevelStarted)
      {
         m_movement.SetCanMove(true);
         m_ai.SetCanThink(true);
         
         var currentColor = m_model.color;
         currentColor.a = 1f;
         m_model.color = currentColor;
      }
      else if(gameEventType == GameEventType.GameOver)
      {
         m_movement.SetCanMove(false);
         m_ai.SetCanThink(false);
         
         this.gameObject.SetActive(false);
      }
   }
}
