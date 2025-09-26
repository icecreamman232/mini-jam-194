using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private int m_minCoin;
    [SerializeField] private int m_maxCoin;
    [SerializeField] private float m_spawnCoinChance;
    [SerializeField] private GameObject m_coinPrefab;
    
    private EnemyHealth m_health;

    private void Start()
    {
        m_health = GetComponentInParent<EnemyHealth>();
        m_health.OnDeath = SpawnLoot;
    }

    private void SpawnLoot()
    {
        var chanceToSpawnCoin = UnityEngine.Random.Range(0f, 100f);
        if (chanceToSpawnCoin <= m_spawnCoinChance)
        {
            var coin = Instantiate(m_coinPrefab, transform.position, Quaternion.identity);
            var rb = coin.GetComponent<Rigidbody2D>();
            rb.AddForce((UnityEngine.Random.insideUnitCircle).normalized * UnityEngine.Random.Range(300, 500f));
        }
    }
}
