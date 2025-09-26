using System.Collections;
using SGGames.Script.Events;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SGGames.Script.UI
{
    
    public enum TransitionType
    {
        HorizontalReflect,
        VerticalReflect,
        SpinQuadrant,
        CrashingWave,
        COUNT,
        RANDOM,
    }
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Image m_transitionFadeInImage;
        [SerializeField] private Image m_transitionFadeOutImage;
        [SerializeField] private Material m_reflectMaterial;
        [SerializeField] private Material m_spinQuadrantMaterial;
        [SerializeField] private Material m_crashingWaveMaterial;
        [SerializeField] private LoadingScreenEvent m_loadingScreenEvent;
        
        private TransitionType m_lastTransitionType;
        public static float k_DefaultLoadingDuration = 1.5f;
        private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int IsHorizontal = Shader.PropertyToID("_IsHorizontal");

        private void Awake()
        {
            m_loadingScreenEvent.AddListener(OnReceiveLoadScreenEvent);
        }

        private void OnDestroy()
        {
            m_loadingScreenEvent.RemoveListener(OnReceiveLoadScreenEvent);
        }

        [ContextMenu("Test fade in")]
        public void Test()
        {
           StartCoroutine(OnFadeIn(k_DefaultLoadingDuration, TransitionType.HorizontalReflect));
        }

        private TransitionType GetRandomTransition()
        {
            return (TransitionType)Random.Range(0, (int) TransitionType.COUNT);
        }

        private Material GetMaterialForTransition(TransitionType transitionType)
        {
            switch (transitionType)
            {
                case TransitionType.HorizontalReflect:
                case TransitionType.VerticalReflect:
                    return new Material(m_reflectMaterial);
                case TransitionType.SpinQuadrant:
                    return new Material(m_spinQuadrantMaterial);
                case TransitionType.CrashingWave:
                    return new Material(m_crashingWaveMaterial);
            }
            Debug.LogError("Unknown transition type: " + transitionType);
            return null;
        }

        private void SetupShaderProperties(TransitionType transitionType, Material material)
        {
            if (transitionType == TransitionType.HorizontalReflect)
            {
                material.SetFloat(IsHorizontal, 1.0f);
            }
            else if(transitionType == TransitionType.VerticalReflect)
            {
                material.SetFloat(IsHorizontal, 0.0f);
            }
        }

        private IEnumerator OnFadeIn(float duration, TransitionType transitionType)
        {
            m_lastTransitionType = transitionType == TransitionType.RANDOM ? GetRandomTransition() : transitionType;
            
            Material transitionMaterial = GetMaterialForTransition(m_lastTransitionType);
            if(transitionMaterial == null) yield break;
            m_transitionFadeInImage.gameObject.SetActive(true);
            m_transitionFadeInImage.material = transitionMaterial;
            m_transitionFadeInImage.material.SetFloat(Cutoff, 0);
            
            SetupShaderProperties(m_lastTransitionType, m_transitionFadeInImage.material);
            
            var startTime = Time.time;
            var timeStop = Time.time + duration;
            while (Time.time < timeStop)
            {
                var progress = Mathf.Lerp(0, 1, (Time.time - startTime) / duration);
                m_transitionFadeInImage.material.SetFloat(Cutoff, progress);
                yield return null;
            }
            m_transitionFadeInImage.material.SetFloat(Cutoff, 1);
            m_transitionFadeInImage.gameObject.SetActive(false);
            m_canvasGroup.alpha = 1;
        }

        private IEnumerator OnFadeOut(float duration)
        {
            Material transitionMaterial = GetMaterialForTransition(m_lastTransitionType);
            if(transitionMaterial == null) yield break;
            
            m_transitionFadeOutImage.gameObject.SetActive(true);
            m_transitionFadeOutImage.material = transitionMaterial;
            m_transitionFadeOutImage.material.SetFloat(Cutoff, 1);
            
            m_canvasGroup.alpha = 0;
            
            SetupShaderProperties(m_lastTransitionType, m_transitionFadeOutImage.material);
            
            var startTime = Time.time;
            var timeStop = Time.time + duration;
            while (Time.time <= timeStop)
            {
                var progress = Mathf.Lerp(1, 0, (Time.time - startTime) / duration);
                m_transitionFadeOutImage.material.SetFloat(Cutoff, progress);
                yield return null;
            }
            m_transitionFadeOutImage.material.SetFloat(Cutoff, 0);
            m_transitionFadeOutImage.gameObject.SetActive(false);
            
        }

        private void OnReceiveLoadScreenEvent(LoadingScreenEventData eventData)
        {
            if (eventData.LoadingType == LoadingScreenEventType.FadeIn)
            {
                Debug.Log("Fade In");
                StartCoroutine(OnFadeIn(eventData.Duration, eventData.TransitionType));
            }
            else if (eventData.LoadingType == LoadingScreenEventType.FadeOut)
            {
                StartCoroutine(OnFadeOut(eventData.Duration));
            }
        }
    }
}
