using SGGames.Script.UI;
using UnityEngine;

namespace SGGames.Script.Events
{
    [CreateAssetMenu(fileName = "Loading Screen Event", menuName = "SGGames/Event/Loading Screen Event")]
    public class LoadingScreenEvent : ScriptableEvent<LoadingScreenEventData>
    {
    
    }

    public enum LoadingScreenEventType
    {
        /// <summary>
        /// From 0 -> 1
        /// </summary>
        FadeIn,
        
        /// <summary>
        /// From 1 -> 0
        /// </summary>
        FadeOut,
    }

    public class LoadingScreenEventData
    {
        public LoadingScreenEventType LoadingType;
        public TransitionType TransitionType = TransitionType.RANDOM;
        public float Duration = 1.5f;
    }
}
