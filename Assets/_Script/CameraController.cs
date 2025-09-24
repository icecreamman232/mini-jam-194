using SGGames.Script.Core;
using UnityEngine;

namespace SGGames.Scripts.System
{
    public class CameraController : MonoBehaviour, IGameService
    {
        [SerializeField] private Transform m_followingTarget;
        [SerializeField] private bool m_canFollow;
        
        [Header("Screen Shake Settings")]
        [SerializeField] private float m_shakeIntensity = 1f;
        [SerializeField] private float m_shakeDuration = 0.2f;
        [SerializeField] private AnimationCurve m_shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        
        [Header("Movement Settings")]
        [SerializeField] private float m_followSpeed = 5f; // Speed of camera follow

        
        [Header("Camera Bounds")]
        [SerializeField] private bool m_useBounds = true;
        [SerializeField] private Vector2 m_boundsMin = new Vector2(-10f, -10f);
        [SerializeField] private Vector2 m_boundsMax = new Vector2(10f, 10f);
        
        [Header("Gizmo Settings")]
        [SerializeField] private bool m_showBoundsInScene = true;
        [SerializeField] private Color m_boundsColor = Color.green;

        private Vector3 m_currentTargetPos;
        private float m_cameraDistance;
        private Camera m_camera;
        
        // Screen shake variables
        private Vector3 m_shakeOffset;
        private bool m_isShaking;
        private float m_shakeTimer;
        private float m_currentShakeDuration;
        private float m_currentShakeIntensity;

        
        // Camera dimensions in world units for pixel perfect camera
        private float m_cameraHalfWidth;
        private float m_cameraHalfHeight;
        
        private readonly Vector3 m_defaultPosition = new Vector3(0f, 0f, -10f);
        
        public static bool IsActivated;

        private void Awake()
        {
            ServiceLocator.RegisterService<CameraController>(this);
            m_cameraDistance = transform.position.z;
            m_camera = GetComponent<Camera>();
            CalculateCameraBounds();
            IsActivated = true;
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<CameraController>();
        }
        
        /// <summary>
        /// Triggers screen shake with default parameters
        /// </summary>
        public void TriggerShake()
        {
            TriggerShake(m_shakeIntensity, m_shakeDuration);
        }

        /// <summary>
        /// Triggers screen shake with custom intensity and duration
        /// </summary>
        /// <param name="intensity">Shake intensity (how far the camera moves)</param>
        /// <param name="duration">Shake duration in seconds</param>
        public void TriggerShake(float intensity, float duration)
        {
            m_currentShakeIntensity = intensity;
            m_currentShakeDuration = duration;
            m_shakeTimer = 0f;
            m_isShaking = true;
        }

        /// <summary>
        /// Stops any ongoing screen shake
        /// </summary>
        public void StopShake()
        {
            m_isShaking = false;
            m_shakeOffset = Vector3.zero;
        }

        /// <summary>
        /// Instantly snaps the camera to the target position
        /// </summary>
        public void SnapToTargetPosition()
        {
            transform.position = m_followingTarget.position;
        }


        private void CalculateCameraBounds()
        {
            // For pixel perfect camera with ref 960x540 and PPU 64
            // Camera height in world units = reference height / pixels per unit / 2
            m_cameraHalfHeight = (540f / 64f) / 2f;
            m_cameraHalfWidth = (960f / 64f) / 2f;
        }
        
        private void FixedUpdate()
        {
            if (!IsActivated) return;
            if (!m_canFollow) return;
            if (m_followingTarget == null) return;
            
            UpdateShake();
            
            
            m_currentTargetPos = m_followingTarget.position;
            m_currentTargetPos.z = m_cameraDistance;
            
            if (m_useBounds)
            {
                // Apply bounds constraint taking camera dimensions into account
                float constrainedX = Mathf.Clamp(m_currentTargetPos.x, 
                    m_boundsMin.x + m_cameraHalfWidth, 
                    m_boundsMax.x - m_cameraHalfWidth);
                    
                float constrainedY = Mathf.Clamp(m_currentTargetPos.y, 
                    m_boundsMin.y + m_cameraHalfHeight, 
                    m_boundsMax.y - m_cameraHalfHeight);
                
                m_currentTargetPos.x = constrainedX;
                m_currentTargetPos.y = constrainedY;
            }
            
            // Smooth camera movement using lerp
            Vector3 targetPosition = Vector3.Lerp(transform.position, m_currentTargetPos, m_followSpeed * Time.deltaTime);
            
            // Apply shake offset
            transform.position = targetPosition + m_shakeOffset;

        }
        
        private void UpdateShake()
        {
            if (!m_isShaking) return;
            
            m_shakeTimer += Time.deltaTime;
            float progress = m_shakeTimer / m_currentShakeDuration;
            
            if (progress >= 1f)
            {
                // Shake finished
                m_isShaking = false;
                m_shakeOffset = Vector3.zero;
                return;
            }
            
            // Calculate shake intensity based on curve
            float curveValue = m_shakeCurve.Evaluate(progress);
            float currentIntensity = m_currentShakeIntensity * curveValue;
            
            // Generate random shake offset
            m_shakeOffset = new Vector3(
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                0f
            );
        }

        

        private void OnDrawGizmosSelected()
        {
            if (m_showBoundsInScene)
            {
                DrawBoundsGizmos();
            }
        }

        private void OnDrawGizmos()
        {
            if (m_showBoundsInScene)
            {
                DrawBoundsGizmos();
            }
        }

        private void DrawBoundsGizmos()
        {
            Gizmos.color = m_boundsColor;
            
            // Draw bounds rectangle
            Vector3 center = new Vector3((m_boundsMin.x + m_boundsMax.x) / 2f, 
                                        (m_boundsMin.y + m_boundsMax.y) / 2f, 
                                        transform.position.z);
            Vector3 size = new Vector3(m_boundsMax.x - m_boundsMin.x, 
                                      m_boundsMax.y - m_boundsMin.y, 
                                      0f);
            
            Gizmos.DrawWireCube(center, size);
            
            // Draw camera bounds (the actual constrained area)
            if (Application.isPlaying || m_camera != null)
            {
                // Recalculate camera bounds if not in play mode
                if (!Application.isPlaying)
                {
                    CalculateCameraBounds();
                }
                
                Gizmos.color = new Color(m_boundsColor.r, m_boundsColor.g, m_boundsColor.b, 0.3f);
                Vector3 constrainedCenter = new Vector3(
                    (m_boundsMin.x + m_cameraHalfWidth + m_boundsMax.x - m_cameraHalfWidth) / 2f,
                    (m_boundsMin.y + m_cameraHalfHeight + m_boundsMax.y - m_cameraHalfHeight) / 2f,
                    transform.position.z);
                Vector3 constrainedSize = new Vector3(
                    (m_boundsMax.x - m_cameraHalfWidth) - (m_boundsMin.x + m_cameraHalfWidth),
                    (m_boundsMax.y - m_cameraHalfHeight) - (m_boundsMin.y + m_cameraHalfHeight),
                    0f);
                
                Gizmos.DrawCube(constrainedCenter, constrainedSize);
            }
        }

        // Method to update bounds at runtime if needed
        public void SetBounds(Vector2 min, Vector2 max)
        {
            m_boundsMin = min;
            m_boundsMax = max;
        }
        
        // Method to enable/disable bounds at runtime
        public void SetUseBounds(bool useBounds)
        {
            m_useBounds = useBounds;
        }

        public void SetTarget(Transform playerTransform)
        {
            m_followingTarget = playerTransform;
        }
    }
}
