using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Script.Core
{
    public class InputManager : MonoBehaviour, IGameService, IBootStrap
    {
        [SerializeField] private Texture2D m_cursorTexture;
        private static Camera m_camera;
        private InputAction m_moveAction;
        private InputAction m_attackAction;
        private InputAction m_reloadAction;
        private InputAction m_pauseAction;

        private Vector2 m_hotSpot = Vector2.zero; // The click point in your texture (usually 0,0 for top-left)
        private CursorMode m_cursorMode = CursorMode.Auto;
        
        public Action<Vector2> OnMoveInputCallback;
        public Action<Vector2> WorldMousePosition;
        public Action OnAttackInputCallback;
        public Action OnAttackInputHeldCallback;
        public Action OnReloadInputCallback;
        public Action OnPauseInputCallback;

        public static bool IsActivated;

        #region Unity Cycle
        private void Awake()
        {
            if (m_cursorTexture != null)
            {
                Cursor.SetCursor(m_cursorTexture, m_hotSpot, m_cursorMode);
            }
        }
        
        private void Update()
        {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (Time.timeScale >= 1)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }
            
            #endif
            
            if (!IsActivated) return;
            OnMoveInputCallback?.Invoke(m_moveAction.ReadValue<Vector2>());
            WorldMousePosition?.Invoke(ComputeWorldMousePosition());
            if (m_attackAction.IsPressed())
            {
                OnAttackInputHeldCallback?.Invoke();
            }
        }
        
        #endregion

        public static void SetActive(bool isActive)
        {
            IsActivated = isActive;
            if (isActive)
            {
                InputSystem.actions.Enable();
            }
            else
            {
                InputSystem.actions.Disable();
            }
        }
        
        public static Vector3 GetWorldMousePosition()
        {
            if(m_camera == null) return Vector3.zero;
            var mousePos = Input.mousePosition;
            mousePos = m_camera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }
        
        
        public void Install()
        {
            m_camera = Camera.main;
            ServiceLocator.RegisterService<InputManager>(this);
            FindActions();
            RegisterAction();
            IsActivated = true;
        }

        public void Uninstall()
        {
            IsActivated = false;
            UnregisterAction();
            ServiceLocator.UnregisterService<InputManager>();
        }

        private void FindActions()
        {
            m_moveAction = InputSystem.actions.FindAction("Move");
            m_attackAction = InputSystem.actions.FindAction("Attack");
            m_reloadAction = InputSystem.actions.FindAction("Reload");
            m_pauseAction = InputSystem.actions.FindAction("Pause");
        }

        private void RegisterAction()
        {
            m_attackAction.performed += AttackActionOnPerformed;
            m_reloadAction.performed += OnReloadInputPerformed;
            m_pauseAction.performed += OnPauseInputPerformed;
        }

        private void UnregisterAction()
        {
            m_attackAction.performed -= AttackActionOnPerformed;
            m_reloadAction.performed -= OnReloadInputPerformed;
            m_pauseAction.performed -= OnPauseInputPerformed;
        }
        
        private void AttackActionOnPerformed(InputAction.CallbackContext callbackContext)
        {
            OnAttackInputCallback?.Invoke();
        }
        
        private void OnReloadInputPerformed(InputAction.CallbackContext callbackContext)
        {
            OnReloadInputCallback?.Invoke();
        }
        
        private void OnPauseInputPerformed(InputAction.CallbackContext callbackContext)
        {
            OnPauseInputCallback?.Invoke();
        }
        
        private Vector3 ComputeWorldMousePosition()
        {
            var mousePos = Input.mousePosition;
            mousePos = m_camera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            return mousePos;
        }
    }
}
