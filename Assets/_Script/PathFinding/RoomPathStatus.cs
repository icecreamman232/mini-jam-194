using System;
using System.Collections.Generic;
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
    public bool IsOccupiedByEnemy;
}

public class RoomPathStatus : MonoBehaviour, IGameService
{
    [SerializeField] public int m_roomWidth;
    [SerializeField] public int m_roomHeight;
    [SerializeField] private int m_frameToUpdate;
    private RoomCell[] m_roomValues;
    private Transform m_playerTransform;
    private readonly System.Collections.Generic.Queue<int> m_indexQueue = new System.Collections.Generic.Queue<int>(256);
    private Vector3 m_lastPlayerPosition;
    private int[] m_directions;
    private int m_frameCounter;
    
    // Track all registered enemies
    private HashSet<Transform> m_registeredEnemies = new HashSet<Transform>();


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
        m_directions = new []{ -1, 1, -m_roomWidth - 1, -m_roomWidth + 1 , m_roomWidth - 1, m_roomWidth + 1}; // left, right, up, down

        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        var cellPosOfPlayer = WorldToCell(m_playerTransform.position);
        GetCell(cellPosOfPlayer).DistanceValue = 100;
        UpdateGridValue();

        m_lastPlayerPosition = m_playerTransform.position;
        m_frameCounter = 0;
        
        ServiceLocator.RegisterService<RoomPathStatus>(this);
    }

    private void Update()
    {
        if(m_playerTransform == null) return;
        m_frameCounter++;
        if (m_frameCounter >= m_frameToUpdate && m_playerTransform.position != m_lastPlayerPosition)
        {
            m_lastPlayerPosition = m_playerTransform.position;
            UpdateGridValue();
            m_frameCounter = 0;
        }
        
        // Update enemy occupied cells every frame
        UpdateEnemyOccupiedCells();

    }
    
    public void RegisterEnemy(Transform enemy)
    {
        m_registeredEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Transform enemy)
    {
        m_registeredEnemies.Remove(enemy);
    }


    public List<Vector2> FindPathToPlayer(Vector3 targetPosition)
    {
        var startCell = WorldToCell(targetPosition);
        int currentIndex = startCell.x + startCell.y * m_roomWidth;

        // Validate starting position
        if (currentIndex < 0 || currentIndex >= m_roomValues.Length || !m_roomValues[currentIndex].IsWalkable)
            return new List<Vector2>(); // Return empty array if invalid starting position

        // If we're already at the player position, return empty path
        if (m_roomValues[currentIndex].DistanceValue == 100)
            return new List<Vector2>();

        // Use a list to collect path points
        var pathList = new System.Collections.Generic.List<Vector2>();

        // Follow the path by choosing highest cost neighbors (toward player)
        while (m_roomValues[currentIndex].DistanceValue != 100)
        {
            var nextCellPos = GetBestNeighborCell(currentIndex, startCell);
            int nextIndex = nextCellPos.x + nextCellPos.y * m_roomWidth;
    
            // Safety check: if we can't find a better neighbor or we're stuck in a loop
            if (nextIndex == currentIndex || m_roomValues[nextIndex].DistanceValue <= m_roomValues[currentIndex].DistanceValue)
                break;
    
            currentIndex = nextIndex;
    
            // Add the world position to the path (convert Vector3 to Vector2)
            Vector3 worldPos = CellToWorld(nextCellPos);
            pathList.Add(new Vector2(worldPos.x, worldPos.y));
    
            // Safety check: prevent infinite loops
            if (pathList.Count > m_roomWidth * m_roomHeight)
                break;
        }

        return pathList;
    }
    
    private void UpdateEnemyOccupiedCells()
    {
        // Clear all enemy occupation flags
        for (int i = 0; i < m_roomValues.Length; i++)
        {
            m_roomValues[i].IsOccupiedByEnemy = false;
        }
    
        // Mark cells occupied by enemies
        foreach (var enemy in m_registeredEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;
            
            var cellPos = WorldToCell(enemy.position);
        
            // Validate cell position
            if (cellPos.x >= 0 && cellPos.x < m_roomWidth && cellPos.y >= 0 && cellPos.y < m_roomHeight)
            {
                var cell = GetCell(cellPos);
                cell.IsOccupiedByEnemy = true;
            }
        }
    }




    private Vector2Int GetBestNeighborCell(int currentIndex, Vector2Int requestingEnemyCell)

    {
        int highestCost = -1;
        int bestNeighborIndex = currentIndex; // Default to current position if no valid neighbor found

        // Check all directions using index arithmetic
        foreach (int direction in m_directions)
        {
            int neighborIndex = currentIndex + direction;
    
            // Bounds checking
            if (IsValidNeighbor(currentIndex, neighborIndex, direction))
            {
                var neighbor = m_roomValues[neighborIndex];
            
                // Check if cell is walkable and not occupied by another enemy
                bool isTraversable = neighbor.IsWalkable && neighbor.DistanceValue > 0;
            
                // Allow movement if cell is not occupied OR if it's the requesting enemy's current cell
                Vector2Int neighborPos = new Vector2Int(neighborIndex % m_roomWidth, neighborIndex / m_roomWidth);
                bool canMove = !neighbor.IsOccupiedByEnemy || neighborPos == requestingEnemyCell;
            
                if (isTraversable && canMove && neighbor.DistanceValue > highestCost)
                {
                    highestCost = neighbor.DistanceValue;
                    bestNeighborIndex = neighborIndex;
                }
            }
        }

        return new Vector2Int(bestNeighborIndex % m_roomWidth, bestNeighborIndex / m_roomWidth);

    }


    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        Vector2 offset = new Vector2(0.5f - m_roomWidth/2, 0.5f - m_roomHeight/2);
        var cell = (worldPosition - transform.position - (Vector3)offset);
        return new Vector2Int(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y));
    }

    public Vector3 CellToWorld(Vector2Int cellPosition)
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

        int currentRow = currentIndex / m_roomWidth;
        int currentCol = currentIndex % m_roomWidth;
        int neighborRow = neighborIndex / m_roomWidth;
        int neighborCol = neighborIndex % m_roomWidth;

        // Check for horizontal wrapping (left/right movement)
        if (direction == -1) // left
            return currentCol > 0;
        if (direction == 1) // right
            return currentCol < (m_roomWidth - 1);
    
        // Check for vertical movement (up/down)
        if (direction == -m_roomWidth) // down
            return currentRow > 0;
        if (direction == m_roomWidth) // up
            return currentRow < (m_roomHeight - 1);
    
        // Check diagonal movements
        if (direction == -m_roomWidth - 1) // down-left
            return currentRow > 0 && currentCol > 0;
        if (direction == -m_roomWidth + 1) // down-right
            return currentRow > 0 && currentCol < (m_roomWidth - 1);
        if (direction == m_roomWidth - 1) // up-left
            return currentRow < (m_roomHeight - 1) && currentCol > 0;
        if (direction == m_roomWidth + 1) // up-right
            return currentRow < (m_roomHeight - 1) && currentCol < (m_roomWidth - 1);

        return false;

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
                
                // Draw occupied cells
                if (cell.IsOccupiedByEnemy)
                {
                    Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange semi-transparent
                    Gizmos.DrawCube(cell.WorldPosition, Vector3.one * 0.7f);
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
        
        
        // // Test pathfinding from mouse position to player
        // Vector3 mouseWorldPos = InputManager.GetWorldMousePosition();
        // if (mouseWorldPos != Vector3.zero)
        // {
        //     // Draw mouse position
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawSphere(mouseWorldPos, 0.3f);
        //
        //     // Find and draw path
        //     List<Vector2> path = FindPathToPlayer(mouseWorldPos);
        //     if (path.Count > 0)
        //     {
        //         // Draw path lines
        //         Gizmos.color = Color.magenta;
        //         Vector3 previousPos = mouseWorldPos;
        //     
        //         for (int i = 0; i < path.Count; i++)
        //         {
        //             Vector3 currentPos = new Vector3(path[i].x, path[i].y, mouseWorldPos.z);
        //         
        //             // Draw line from previous to current
        //             Gizmos.DrawLine(previousPos, currentPos);
        //         
        //             // Draw path point
        //             Gizmos.color = Color.yellow;
        //             Gizmos.DrawSphere(currentPos, 0.2f);
        //         
        //             previousPos = currentPos;
        //             Gizmos.color = Color.magenta;
        //         }
        //     }
        //
        //     // Draw mouse grid cell outline
        //     var mouseCell = WorldToCell(mouseWorldPos);
        //     if (mouseCell.x >= 0 && mouseCell.x < m_roomWidth && mouseCell.y >= 0 && mouseCell.y < m_roomHeight)
        //     {
        //         Gizmos.color = Color.cyan;
        //         var mouseCellWorldPos = CellToWorld(mouseCell);
        //         Gizmos.DrawWireCube(mouseCellWorldPos, Vector3.one * 1.2f);
        //     }
        // }

    }
}
