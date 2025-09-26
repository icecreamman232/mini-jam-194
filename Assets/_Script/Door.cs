using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform m_model;
    [SerializeField] private SpriteMask m_spriteMask;
    [SerializeField] private Collider2D m_collider;
    [SerializeField] private GameEvent m_gameEvent;
    [SerializeField] private Transform m_centerPoint;
    private PlayerController m_playerRef;
    
    private void Awake()
    {
        m_gameEvent.AddListener(OnReceiveGameEvent);
        HideDoor();
    }

    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (other.gameObject.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Freeze();
            PlayGoToDoorAnimation(playerController);
        }
        else
        {
            Debug.LogError("PlayerController is null");
        }
    }

    private void PlayGoToDoorAnimation(PlayerController player)
    {
        m_playerRef = player;
        player.transform.DOMove(m_centerPoint.position, 0.3f)
            .OnComplete(() =>
            {
                SinkDownAnim();
            });
    }

    private void SinkDownAnim()
    {
        m_spriteMask.enabled = true;
        var currentLocalY = m_playerRef.transform.localPosition.y;
        m_playerRef.transform.DOMoveY(currentLocalY - 1f, 1f)
            .SetEase(Ease.InCirc)
            .OnComplete(() =>
            {
                m_playerRef.HideVisual();
                m_gameEvent.Raise(GameEventType.LoadNextLevel);
            });
    }
    
    private void OnReceiveGameEvent(GameEventType gameEventType)
    {
        if (gameEventType == GameEventType.OpenDoor)
        {
            ShowDoor();
        }
        else if(gameEventType == GameEventType.CreatedLevel)
        {
            HideDoor();      
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
        m_spriteMask.enabled = false;
        m_collider.enabled = false;
        m_model.localScale = Vector3.one * 0.5f;
        m_model.gameObject.SetActive(false);
    }
}
