using SGGames.Script.Core;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IGameService, IBootStrap
{
    [SerializeField] private int m_coins;
    
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
    }

    public bool SpendCoin(int amount)
    {
        if(amount > m_coins) return false;
        m_coins -= amount;
        return true;
    }
}
