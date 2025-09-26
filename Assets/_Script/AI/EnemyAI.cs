using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] protected bool m_canThink;

    public void SetCanThink(bool canThink)
    {
        m_canThink = canThink;
    }
}
