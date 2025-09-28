using SGGames.Script.Core;
using TMPro;
using UnityEngine;

public class CoinHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_coinText;

    private void Start()
    {
        ServiceLocator.GetService<CurrencyManager>().OnCoinChange += OnCoinChange;
        OnCoinChange(ServiceLocator.GetService<CurrencyManager>().Coins);
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<CurrencyManager>().OnCoinChange -= OnCoinChange;
    }

    private void OnCoinChange(int totalCoins)
    {
        m_coinText.text = totalCoins.ToString();
    }
}
