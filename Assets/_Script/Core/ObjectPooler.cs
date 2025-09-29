using System.Collections.Generic;
using UnityEngine;

namespace SGGames.Script.Core
{
    public class ObjectPooler<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool m_autoCreateParentForPool;
        [SerializeField] private GameObject m_parent;
        [SerializeField] private bool m_isSharePool;
        [SerializeField] private bool m_pooledObjectActiveByDefault;
        [SerializeField] private T m_objectToPool;
        [SerializeField] private int m_poolSize;

        private List<T> m_pool;
        private GameObject m_autoCreatedPoolParent;
        public List<T> Pool => m_pool;
        
        protected virtual void Awake()
        {
            CreatePool();
        }
        
        private void CreatePool()
        {
            if (m_objectToPool == null) return;
            
            if (m_pool == null)
            {
                m_pool = new List<T>();
            }

            if (m_autoCreateParentForPool)
            {
                m_autoCreatedPoolParent = new GameObject(m_objectToPool.name + " Parent");
            }
            

            //Find existing pool with same name
            if (m_isSharePool)
            {
                var pools = FindObjectsByType<ObjectPooler<T>>(FindObjectsSortMode.None);
                for (int i = 0; i < pools.Length; i++)
                {
                    if(pools[i].GetInstanceID() == this.GetInstanceID()) continue;
                    if (pools[i].m_objectToPool.name == m_objectToPool.name && pools[i].m_objectToPool!=null)
                    {
                        m_pool = pools[i].m_pool;
                        return;
                    }
                }
            }
            
            for (int i = 0; i < m_poolSize; i++)
            {
                var pooledObject = Instantiate(m_objectToPool, m_autoCreateParentForPool ? m_autoCreatedPoolParent.transform : m_parent.transform);
                var currentName = pooledObject.name;
                currentName += $"({i})";
                pooledObject.name = currentName;
                pooledObject.gameObject.SetActive(m_pooledObjectActiveByDefault);
                m_pool.Add(pooledObject);
            }
        }

        public T GetPooledObject()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                if (!m_pool[i].gameObject.activeInHierarchy)
                {
                    m_pool[i].gameObject.SetActive(true);
                    return m_pool[i];
                }
            }
            
            return null;
        }

        public List<T> GetPooledObjects(int amount)
        {
            var pooledObjects = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                var pooledObject = GetPooledObject();
                if (pooledObject == null)
                {
                    break;
                }
                pooledObjects.Add(pooledObject);
            }
            return pooledObjects;
        }

        public void CleanUp()
        {
            if (m_autoCreatedPoolParent != null)
            {
                Destroy(m_autoCreatedPoolParent);
                return;
            }
            
            
            for (int i = 0; i < m_pool.Count; i++)
            {
                if(m_pool[i].gameObject == null) continue;
                Destroy(m_pool[i].gameObject);
            }
        }
    }
}

