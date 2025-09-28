using System;
using SGGames.Script.Core;
using TMPro;
using UnityEngine;

public class ItemDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private TextMeshProUGUI m_priceText;
    [SerializeField] private TextMeshProUGUI m_toxicCostText;
    
    private ItemData m_itemData;
    private CurrencyManager m_currencyManager;

    private void Start()
    {
        m_currencyManager = ServiceLocator.GetService<CurrencyManager>();
        ServiceLocator.GetService<CurrencyManager>().OnCoinChange += OnCoinChange;
        Setup(ServiceLocator.GetService<ItemManager>().GetRandomItem());
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<CurrencyManager>().OnCoinChange -= OnCoinChange;
    }

    private void OnCoinChange(int totalCoins)
    {
        if (m_currencyManager.CanPurchase(m_itemData.Price))
        {
            m_priceText.color = Color.white;
        }
        else
        {
            m_priceText.color = new Color(1f, 0.1411765f, 0f);
        }
        
    }

    private void Setup(ItemData itemData)
    {
        m_itemData = itemData;
        m_model.sprite = itemData.Icon;
        m_priceText.text = itemData.Price.ToString();
        m_toxicCostText.text = itemData.ToxicPoint.ToString();
        if (!m_currencyManager.CanPurchase(m_itemData.Price))
        {
            m_priceText.color = new Color(1f, 0.1411765f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        if (m_currencyManager.SpendCoin(m_itemData.Price))
        {
            ServiceLocator.GetService<ItemManager>().PurchaseItem(m_itemData);
            this.gameObject.SetActive(false);
        }
    }
}
