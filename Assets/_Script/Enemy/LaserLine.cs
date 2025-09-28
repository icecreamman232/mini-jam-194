using System;
using System.Collections;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] private EdgeCollider2D m_edgeCollider2D;
    [SerializeField] private float m_drawSpeed = 5f; // Units per second
    public void SetLaserLineTo(Vector2 target, Action onFinish)
    {
        StartCoroutine(OnSetLaserLineTo(target, onFinish));
    }

    private IEnumerator OnSetLaserLineTo(Vector2 target, Action onFinish)
    {
        // Get the current end position of the line (last point)
        Vector2 startPosition;
        if (m_lineRenderer.positionCount == 0)
        {
            // First time drawing - start from object position
            startPosition = transform.position;
            m_lineRenderer.positionCount = 1;
            m_lineRenderer.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 0f));

        }
        else
        {
            // Continue from the last point
            Vector3 lastPos = m_lineRenderer.GetPosition(m_lineRenderer.positionCount - 1);
            startPosition = new Vector2(lastPos.x, lastPos.y);

        }
        
        // Add a new point at the current end position
        m_lineRenderer.positionCount++;
        int newPointIndex = m_lineRenderer.positionCount - 1;
        m_lineRenderer.SetPosition(newPointIndex, new Vector3(startPosition.x, startPosition.y, 0f));

        
        float distance = Vector2.Distance(startPosition, target);
        float duration = distance / m_drawSpeed;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Interpolate the new point towards the target
            Vector2 currentEndPosition = Vector2.Lerp(startPosition, target, progress);
            m_lineRenderer.SetPosition(newPointIndex, new Vector3(currentEndPosition.x, currentEndPosition.y, 0f));

            
            // Update the collider to match the current line
            UpdateCollider();
            
            yield return null;
        }
        
        // Ensure the new point reaches exactly the target position
        m_lineRenderer.SetPosition(newPointIndex, new Vector3(target.x, target.y, 0f));

        UpdateCollider();
        onFinish?.Invoke();
    }

    
    private void UpdateCollider()
    {
        Vector2[] points = new Vector2[m_lineRenderer.positionCount];

        for (int i = 0; i < m_lineRenderer.positionCount; i++)
        {
            Vector3 worldPos = m_lineRenderer.GetPosition(i);
            Vector2 localPos = transform.InverseTransformPoint(worldPos);
            points[i] = localPos;
        }
        m_edgeCollider2D.points = points;
    }

}
