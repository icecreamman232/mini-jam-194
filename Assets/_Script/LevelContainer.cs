using UnityEngine;

public enum LevelGrade
{
    Easy,
    Medium,
    Hard,
}

[CreateAssetMenu(fileName = "LevelContainer", menuName = "Scriptable Objects/LevelContainer")]
public class LevelContainer : ScriptableObject
{
    [SerializeField] private LevelGrade m_levelGrade;
    [SerializeField] private GameObject[] m_levelPrefabs;
    
    public LevelGrade LevelGrade => m_levelGrade;
    public GameObject[] LevelPrefabs => m_levelPrefabs;
    
    public GameObject GetLevelPrefab(int index)
    {
        return m_levelPrefabs[index];
    }

    public GameObject GetRandomLevelPrefab()
    {
        return m_levelPrefabs[Random.Range(0, m_levelPrefabs.Length)];
    }
}
