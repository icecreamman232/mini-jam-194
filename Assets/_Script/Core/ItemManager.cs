using System.Collections.Generic;
using System.Linq;
using SGGames.Script.Core;
using UnityEngine;

public class ItemManager : MonoBehaviour, IGameService, IBootStrap
{
   [SerializeField] private PlayerCollectToxicEvent m_playerCollectToxicEvent;
   [SerializeField] private ItemAnnouncer m_announcer;
   [SerializeField] private ItemData[] m_items;

   private PlayerWeaponHandler m_playerWeaponHandler;
   private PlayerToxicController m_playerToxicController;
   private HashSet<ItemData> m_availableItems = new HashSet<ItemData>();
   private List<ItemData> m_ownedItems = new List<ItemData>();
   
   private Dictionary<ModifierType, ItemModifier> m_modifiers = new Dictionary<ModifierType, ItemModifier>();

   public void Install()
   {
      ServiceLocator.RegisterService<ItemManager>(this);
      foreach (var item in m_items)
      {
         m_availableItems.Add(item);
      }
      
      var playerRef = ServiceLocator.GetService<LevelManager>().Player;
      m_playerToxicController = playerRef.GetComponent<PlayerToxicController>();
      m_playerWeaponHandler = playerRef.GetComponent<PlayerWeaponHandler>();
      
      SetupModifiers();
   }

   public void Uninstall()
   {
      ServiceLocator.UnregisterService<ItemManager>();
   }

   public ItemData GetRandomItem()
   {
      if(m_availableItems.Count == 0) return null;
      
      int randomIndex = UnityEngine.Random.Range(0, m_availableItems.Count);
      var randomItem = m_availableItems.ElementAt(randomIndex);
      m_availableItems.Remove(randomItem);
      return randomItem;
   }

   [ContextMenu("Unlock Item")]
   private void Test()
   {
      PurchaseItem(m_items[0]);
   }
   
   public void PurchaseItem(ItemData item)
   {
      Debug.Log($"Purchase item {item.Name}");
      m_ownedItems.Add(item);
      m_announcer.Show(item);
      
      m_playerToxicController.AddToxic(item.ToxicPoint);
      m_playerWeaponHandler.UpdateGunModel(item);
      
      foreach (var modifier in item.Modifiers)
      {
         m_modifiers[modifier.Type].Apply(modifier.Value);
      }
   }
   
   private void SetupModifiers()
   {
      m_modifiers.Add(ModifierType.WeaponAccuracy, new AccuracyModifier());
      m_modifiers.Add(ModifierType.WeaponRecoil, new RecoilModifier());
      m_modifiers.Add(ModifierType.ChangeTargetMask, new DestroyEnemyBulletModifier(m_playerWeaponHandler));
      m_modifiers.Add(ModifierType.UpdateDamage, new DamageModifier(m_playerWeaponHandler));
      m_modifiers.Add(ModifierType.PenetratedBullet, new PenetrationBulletModifier(m_playerWeaponHandler));
   }
}
