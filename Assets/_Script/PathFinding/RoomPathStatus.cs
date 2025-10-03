using System;
using SGGames.Script.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RoomCell
{
    public Vector2Int GridPosition;
    public Vector2 WorldPosition;
    public bool IsWalkable;
    public int DistanceValue;
}

public class RoomPathStatus : MonoBehaviour
{
    [SerializeField] private int m_roomWidth;
    [SerializeField] private int m_roomHeight;
    [SerializeField] private int m_frameToUpdate;
    private RoomCell[] m_roomValues;
    private Transform m_playerTransform;
    private readonly System.Collections.Generic.Queue<int> m_indexQueue = new System.Collections.Generic.Queue<int>(256);
    private Vector3 m_lastPlayerPosition;
    private int[] m_directions;
    private int m_frameCounter;

    private void Start()
    {
        m_roomValues = new RoomCell [m_roomWidth * m_roomHeight];
        Vector2 offset = new Vector2(0.5f - m_roomWidth/2, 0.5f - m_roomHeight/2);
        for (int i = 0; i < m_roomHeight; i++)
        {
            for (int j = 0; j < m_roomWidth; j++)
            {
                m_roomValues[i * m_roomWidth + j] = new RoomCell();
                m_roomValues[i * m_roomWidth + j].GridPosition = new Vector2Int(j, i);
                m_roomValues[i * m_roomWidth + j].WorldPosition = transform.position + new Vector3(j, i, 0) + (Vector3)offset;
                m_roomValues[i * m_roomWidth + j].IsWalkable = true;
            }
        }
        m_directions = new []{ -1, 1, -m_roomWidth, m_roomWidth }; // left, right, up, down

        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        var cellPosOfPlayer = WorldToCell(m_playerTransform.position);
        GetCell(cellPosOfPlayer).DistanceValue = 100;
        UpdateGridValue();

        m_lastPlayerPosition = m_playerTransform.position;
        m_frameCounter = 0;
    }

    private void Update()
    {
        if(m_playerTransform == null) return;
        m_frameCounter++;
        if (m_frameCounter >= m_frameToUpdate && m_playerTransform.position != m_lastPlayerPosition)
        {
            m_lastPlayerPosition = m_playerTransform.position;
            UpdateGridValue();
            Debug.Log("Update Grid Value");
        }
    }


    private Vector2Int WorldToCell(Vector3 worldPosition)
    {
        Vector2 offset = new Vector2(0.5f - m_roomWidth/2, 0.5f - m_roomHeight/2);
        var cell = (worldPosition - transform.position - (Vector3)offset);
        return new Vector2Int(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y));
    }

    private Vector3 CellToWorld(Vector2Int cellPosition)
    {
        return m_roomValues[cellPosition.x + cellPosition.y * m_roomWidth].WorldPosition;
    }

    private RoomCell GetCell(Vector2Int cellPosition)
    {
        return m_roomValues[cellPosition.x + cellPosition.y * m_roomWidth];
    }

    private void UpdateGridValue()
    {
        var playerCellPos = WorldToCell(m_playerTransform.position);
        int playerIndex = playerCellPos.x + playerCellPos.y * m_roomWidth;
    
        // Fast clear using Array.Clear
        for (int i = 0; i < m_roomValues.Length; i++)
        {
            m_roomValues[i].DistanceValue = 0;
        }
    
        // Set player position and start flood fill
        m_roomValues[playerIndex].DistanceValue = 100;
    
        m_indexQueue.Clear();
        m_indexQueue.Enqueue(playerIndex);
    
        while (m_indexQueue.Count > 0)
        {
            int currentIndex = m_indexQueue.Dequeue();
            int currentValue = m_roomValues[currentIndex].DistanceValue;
            int newValue = currentValue - 1;
        
            if (newValue <= 0) continue;
        
            // Check all 4 directions using index arithmetic
            foreach (int direction in m_directions)
            {
                int neighborIndex = currentIndex + direction;
            
                // Bounds checking
                if (IsValidNeighbor(currentIndex, neighborIndex, direction))
                {
                    var neighbor = m_roomValues[neighborIndex];
                    if (neighbor.IsWalkable && neighbor.DistanceValue == 0)
                    {
                        neighbor.DistanceValue = newValue;
                        if (newValue > 1)
                            m_indexQueue.Enqueue(neighborIndex);
                    }
                }
            }
        }
    }
    
    private bool IsValidNeighbor(int currentIndex, int neighborIndex, int direction)
    {
        if (neighborIndex < 0 || neighborIndex >= m_roomValues.Length)
            return false;
    
        // Check for horizontal wrapping
        if (direction == -1) // left
            return (currentIndex % m_roomWidth) > 0;
        if (direction == 1) // right
            return (currentIndex % m_roomWidth) < (m_roomWidth - 1);
    
        return true; // up/down directions
    }


    private void OnDrawGizmosSelected()
    {
        if(m_roomValues == null) return;
        if(m_roomValues.Length == 0) return;

        Vector2 offset = new Vector2(0.5f - m_roomWidth/2, 0.5f - m_roomHeight/2);
        for (int i = 0; i < m_roomHeight; i++)
        {
            for (int j = 0; j < m_roomWidth; j++)
            {
                var cell = m_roomValues[i * m_roomWidth + j];
            
                // Draw grid outline
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + new Vector3(j, i, 0) + (Vector3)offset, Vector3.one);
            
                // Draw unwalkable cells
                if (!cell.IsWalkable)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(cell.WorldPosition, Vector3.one);
                }
            
#if UNITY_EDITOR
                // Draw distance values as text
                if (cell.DistanceValue > 0)
                {
                    // Set text color based on value (optional visual enhancement)
                    Color textColor = Color.white;
                    if (cell.DistanceValue == 100)
                        textColor = Color.green; // Player position
                    else if (cell.DistanceValue < 50)
                        textColor = Color.yellow;
                
                    // Create GUI style for the text
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = textColor;
                    style.fontSize = 12;
                    style.alignment = TextAnchor.MiddleCenter;
                
                    // Draw the distance value
                    Handles.Label(cell.WorldPosition, cell.DistanceValue.ToString(), style);
                }
#endif
            }
        }
    
        // Draw player position
        if (m_playerTransform != null)
        {
            var cellPosOfPlayer = WorldToCell(m_playerTransform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(m_roomValues[cellPosOfPlayer.x + cellPosOfPlayer.y * m_roomWidth].WorldPosition, Vector3.one * 0.8f);
        }

    }
}
