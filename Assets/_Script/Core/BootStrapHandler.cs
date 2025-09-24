using UnityEngine;

namespace SGGames.Script.Core
{
    public class BootStrapHandler : MonoBehaviour, IGameService
    {
        [SerializeField] private MonoBehaviour[] m_bootStrap;

        private void Awake()
        {
            ServiceLocator.RegisterService<BootStrapHandler>(this);
            foreach (var component in m_bootStrap)
            {
                if (component is IBootStrap bootStrap)
                {
                    bootStrap.Install();
                }
            }
        }

        public void UninstallBootStrap()
        {
            foreach (var component in m_bootStrap)
            {
                if (component is IBootStrap bootStrap)
                {
                    bootStrap.Uninstall();
                }
            }
        }
    }
}
