using System.Collections.Generic;
using System.Linq;
using SGGames.Script.Core;
using UnityEngine;

public class ItemManager : MonoBehaviour, IGameService, IBootStrap
{
   [SerializeField] private ItemAnnouncer m_announcer;
   [SerializeField] private ItemData[] m_items;
   
   private HashSet<ItemData> m_availableItems = new HashSet<ItemData>();
   private List<ItemData> m_ownedItems = new List<ItemData>();

   public void Install()
   {
      ServiceLocator.RegisterService<ItemManager>(this);
      foreach (var item in m_items)
      {
         m_availableItems.Add(item);
      }
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
   
   public void PurchaseItem(ItemData item)
   {
      m_ownedItems.Add(item);
      m_announcer.Show(item);
   }
}
