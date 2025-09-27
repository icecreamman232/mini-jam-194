using System;
using SGGames.Script.Core;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private int m_coins = 3;
    
    public Action<int> OnCoinChange;
    
    public void Install()
    {
        ServiceLocator.RegisterService<CurrencyManager>(this);
    }

    public void Uninstall()
    {
        ServiceLocator.UnregisterService<CurrencyManager>();
    }

    public void AddCoin(int amount)
    {
        m_coins += amount;
        OnCoinChange?.Invoke(m_coins);
    }

    public bool SpendCoin(int amount)
    {
        if(amount > m_coins) return false;
        m_coins -= amount;
        OnCoinChange?.Invoke(m_coins);
        return true;
    }
    
    public bool CanPurchase(int amount)
    {
        return m_coins >= amount;
    }
}
