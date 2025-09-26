using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform m_model;
    [SerializeField] private Collider2D m_collider;
    [SerializeField] private GameEvent m_gameEvent;

    private void Awake()
    {
        m_gameEvent.AddListener(OnReceiveGameEvent);
    }

    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }

    private void OnReceiveGameEvent(GameEventType gameEventType)
    {
        if (gameEventType == GameEventType.OpenDoor)
        {
            ShowDoor();
        }
    }

    [ContextMenu("Show Door")]
    private void ShowDoor()
    {
        m_model.gameObject.SetActive(true);
        m_model.localScale = Vector3.one * 0.5f;
        m_model.DOScale(Vector3.one,0.5f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                m_collider.enabled = true;
            });
    }

    private void HideDoor()
    {
        m_collider.enabled = false;
        m_model.localScale = Vector3.one * 0.5f;
        m_model.gameObject.SetActive(false);
    }
}
