using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ToxicDebuffAnnouncer : MonoBehaviour
{
    [SerializeField] private ToxicDebuffEvent m_debuffEvent;
    [SerializeField] private RectTransform m_announcerGO;
    [SerializeField] private TextMeshProUGUI m_debuffName;
    [SerializeField] private TextMeshProUGUI m_debuffDesc;

    private void Awake()
    {
        m_announcerGO.gameObject.SetActive(false);
        m_debuffEvent.AddListener(Show);
    }

    private void OnDestroy()
    {
        m_debuffEvent.RemoveListener(Show);
    }

    public void Show(ToxicDebuffData debuffData)
    {
        var anchorPos = m_announcerGO.anchoredPosition;
        anchorPos.x = -150;
        m_announcerGO.anchoredPosition = anchorPos;
        
        m_debuffName.text = debuffData.Name;
        m_debuffDesc.text = debuffData.Description;
        m_announcerGO.gameObject.SetActive(true);

        m_announcerGO.DOAnchorPosX(0, 0.3f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                StartCoroutine(AfterShow());
            });
    }

    private IEnumerator AfterShow()
    {
        yield return new WaitForSeconds(2f);
        m_announcerGO.gameObject.SetActive(false);
    }
}
