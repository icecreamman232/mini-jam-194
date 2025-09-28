using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
   [SerializeField] private EnemyHealth m_health;
   [SerializeField] private EnemyMovement m_movement;
   [SerializeField] private EnemyAI m_ai;
   [SerializeField] private SpriteRenderer m_model;
   [SerializeField] private GameEvent m_gameEvent;
   
   private float m_frozeTimer;
   
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

   public void ApplyFroze(float duration)
   {
      if (m_frozeTimer <= 0)
      {
         m_frozeTimer = duration;
         StartCoroutine(OnFrozen());
        
      }
      else //Reset froze timer if already frozen
      {
         m_frozeTimer = duration;
      }
   }

   private IEnumerator OnFrozen()
   {
      m_model.color = new Color(0.2389196f, 0.5321058f, 0.8584906f);
      var originalSpeed = m_movement.Speed;
      var frozeSpeed = m_movement.Speed * 0.55f; //Reduce 45% of speed
      m_movement.SetSpeed(frozeSpeed);
      while (m_frozeTimer > 0)
      {
         m_frozeTimer -= Time.deltaTime;
         yield return null;
      }

      m_model.color = Color.white;
      m_movement.SetSpeed(originalSpeed);
   }
}
