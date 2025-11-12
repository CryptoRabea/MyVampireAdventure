using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Pooling
{
    /// <summary>
    /// Manages object pools for efficient reuse of GameObjects
    /// Essential for mobile performance (projectiles, enemies, VFX, etc.)
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string poolName;
            public GameObject prefab;
            public int initialSize = 10;
            public int maxSize = 100;
            public bool expandable = true;
        }

        [Header("Pool Definitions")]
        [SerializeField] private List<Pool> pools = new List<Pool>();

        [Header("Settings")]
        [SerializeField] private Transform poolContainer;

        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, Pool> poolDefinitions = new Dictionary<string, Pool>();
        private Dictionary<GameObject, string> activeObjects = new Dictionary<GameObject, string>();

        private void Awake()
        {
            if (poolContainer == null)
            {
                poolContainer = new GameObject("PoolContainer").transform;
                poolContainer.SetParent(transform);
            }

            InitializePools();
        }

        /// <summary>
        /// Initialize all defined pools
        /// </summary>
        private void InitializePools()
        {
            foreach (Pool pool in pools)
            {
                if (pool.prefab == null)
                {
                    Debug.LogWarning($"[ObjectPoolManager] Pool '{pool.poolName}' has no prefab assigned");
                    continue;
                }

                CreatePool(pool);
            }

            Debug.Log($"[ObjectPoolManager] Initialized {poolDictionary.Count} pools");
        }

        /// <summary>
        /// Create a new pool
        /// </summary>
        private void CreatePool(Pool pool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Create initial objects
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreatePooledObject(pool.prefab, pool.poolName);
                objectPool.Enqueue(obj);
            }

            poolDictionary[pool.poolName] = objectPool;
            poolDefinitions[pool.poolName] = pool;
        }

        /// <summary>
        /// Create a single pooled object
        /// </summary>
        private GameObject CreatePooledObject(GameObject prefab, string poolName)
        {
            GameObject obj = Instantiate(prefab, poolContainer);
            obj.name = $"{poolName}_{obj.GetInstanceID()}";
            obj.SetActive(false);
            return obj;
        }

        /// <summary>
        /// Register a pool at runtime
        /// </summary>
        public void RegisterPool(string poolName, GameObject prefab, int initialSize = 10, int maxSize = 100, bool expandable = true)
        {
            if (poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"[ObjectPoolManager] Pool '{poolName}' already exists");
                return;
            }

            Pool pool = new Pool
            {
                poolName = poolName,
                prefab = prefab,
                initialSize = initialSize,
                maxSize = maxSize,
                expandable = expandable
            };

            CreatePool(pool);
        }

        /// <summary>
        /// Spawn an object from the pool
        /// </summary>
        public GameObject SpawnFromPool(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolName))
            {
                Debug.LogError($"[ObjectPoolManager] Pool '{poolName}' doesn't exist");
                return null;
            }

            GameObject obj = null;
            Queue<GameObject> pool = poolDictionary[poolName];
            Pool poolDef = poolDefinitions[poolName];

            // Try to get an inactive object from the pool
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            // Expand the pool if allowed and not at max size
            else if (poolDef.expandable && activeObjects.Count < poolDef.maxSize)
            {
                obj = CreatePooledObject(poolDef.prefab, poolName);
                Debug.Log($"[ObjectPoolManager] Expanded pool '{poolName}'");
            }
            else
            {
                Debug.LogWarning($"[ObjectPoolManager] Pool '{poolName}' exhausted (max: {poolDef.maxSize})");
                return null;
            }

            // Setup the object
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // Track active object
            activeObjects[obj] = poolName;

            // Call IPoolable interface if implemented
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnSpawnFromPool();

            return obj;
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;

            if (!activeObjects.ContainsKey(obj))
            {
                Debug.LogWarning($"[ObjectPoolManager] Object '{obj.name}' is not from any pool");
                return;
            }

            string poolName = activeObjects[obj];
            activeObjects.Remove(obj);

            // Call IPoolable interface if implemented
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnReturnToPool();

            // Deactivate and return to pool
            obj.SetActive(false);
            obj.transform.SetParent(poolContainer);
            poolDictionary[poolName].Enqueue(obj);
        }

        /// <summary>
        /// Return an object to pool after a delay
        /// </summary>
        public void ReturnToPoolDelayed(GameObject obj, float delay)
        {
            StartCoroutine(ReturnToPoolDelayedCoroutine(obj, delay));
        }

        private System.Collections.IEnumerator ReturnToPoolDelayedCoroutine(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(obj);
        }

        /// <summary>
        /// Clear all pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in poolDictionary.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null) Destroy(obj);
                }
            }

            foreach (var obj in activeObjects.Keys)
            {
                if (obj != null) Destroy(obj);
            }

            poolDictionary.Clear();
            activeObjects.Clear();
            Debug.Log("[ObjectPoolManager] All pools cleared");
        }

        /// <summary>
        /// Get pool statistics
        /// </summary>
        public string GetPoolStats(string poolName)
        {
            if (!poolDictionary.ContainsKey(poolName))
                return $"Pool '{poolName}' not found";

            int inactiveCount = poolDictionary[poolName].Count;
            int activeCount = 0;

            foreach (var kvp in activeObjects)
            {
                if (kvp.Value == poolName) activeCount++;
            }

            return $"Pool '{poolName}': Active={activeCount}, Inactive={inactiveCount}, Total={activeCount + inactiveCount}";
        }
    }
}
