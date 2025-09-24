using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float m_minDamage;
    [SerializeField] private float m_maxDamage;

    private float GetDamage()
    {
        return Random.Range(m_minDamage, m_maxDamage);
    }
}
