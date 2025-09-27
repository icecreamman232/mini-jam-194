using System.Collections;
using SGGames.Script.Core;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
   [SerializeField] private bool m_isUnlocked;
   [SerializeField] private PlayerWeaponHandler m_weaponHandler;
   [SerializeField] private BoxCollider2D m_collider;
   [SerializeField] private float m_teleportRange;
   [SerializeField] private int m_shootCount;

   private PlayerController m_controller;
   private bool m_isTeleporting;
   private int m_shootCounter;

   private void Start()
   {
      m_controller = GetComponent<PlayerController>();
      m_weaponHandler.OnShoot = OnShoot;
   }

   public void SetUnlock()
   {
      m_isUnlocked = true;
   }

   private void OnShoot()
   {
      if (!m_isUnlocked) return;
      if (m_isTeleporting) return;
      m_shootCounter++;
      if (m_shootCounter >= m_shootCount)
      {
         m_shootCounter = 0;
         Teleport();
      }
   }
   private Vector2 GetRandomTeleportPosition()
   {
      var randomPosition = UnityEngine.Random.insideUnitCircle.normalized * m_teleportRange;
      var checkCollide = Physics2D.OverlapBox(randomPosition, m_collider.size, 0, LayerMask.GetMask("Obstacle"));
      if (checkCollide != null)
      {
         return -randomPosition;
      }
      return randomPosition;
   }

   private void Teleport()
   {
      StartCoroutine(TeleportingCoroutine());
   }
   
   private IEnumerator TeleportingCoroutine()
   {
      m_isTeleporting = true;
      InputManager.SetActive(false);

      m_controller.Freeze();
      m_controller.HideVisual();
      yield return new WaitForSeconds(0.2f);
      
      transform.position = GetRandomTeleportPosition();
      yield return new WaitForEndOfFrame();
      
      m_controller.UnFreeze();
      
      InputManager.SetActive(true);
      m_isTeleporting = false;
   }
}
