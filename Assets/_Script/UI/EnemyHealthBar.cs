using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_bar;

    private void Awake()
    {
        m_canvasGroup.alpha = 0;
    }

    public void UpdateBar(float current, float max)
    {
        if (current > 0 && current < max)
        {
            m_canvasGroup.alpha = 1;
        }
        m_bar.fillAmount = MathHelpers.Remap(current, 0, max, 0, 1);
        if (m_bar.fillAmount <= 0)
        {
            m_canvasGroup.alpha = 0;
        }
    }
}
