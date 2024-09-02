using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ObjectType 
{
    Bullet,
    MonsterBullet,
    BatCatcher,
    SwordsMan,
    Gunslinger
}

// A manager for collecting and managing pooled objects
public class PoolingManager : Singleton<PoolingManager>
{
    // Dictionary to store queues of GameObjects categorized by their type
    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();

    // Dictionary to store prefabs corresponding to their type
    public Dictionary<ObjectType, GameObject> prefabDictionary = new Dictionary<ObjectType, GameObject>();

    public GameObject bulletPrefab;
    public GameObject monsterBulletPrefab;
    public GameObject BatCatcherPrefab;
    public GameObject SwordsManPrefab;
    public GameObject GunslingerPrefab;

    public Transform[] PrefabParent = new Transform[5];

    void Start()
    {
        // Assign prefabs to the PoolingManager
        Instance.prefabDictionary[ObjectType.Bullet] = bulletPrefab;
        Instance.prefabDictionary[ObjectType.MonsterBullet] = monsterBulletPrefab;
        Instance.prefabDictionary[ObjectType.BatCatcher] = BatCatcherPrefab;
        Instance.prefabDictionary[ObjectType.SwordsMan] = SwordsManPrefab;
        Instance.prefabDictionary[ObjectType.Gunslinger] = GunslingerPrefab;

        // Initialize pools after assigning prefabs
        Instance.InitializePools();
    }
    // Initialize the pools
    public void InitializePools()
    {
        // Initialize each type's queue
        foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
        {
            poolDictionary[type] = new Queue<GameObject>();

            if (prefabDictionary.TryGetValue(type, out GameObject prefab))
            {
                int index = ((int)type > 1 ? 20 : 50);
                for (int i = 0; i < index; i++)
                {
                    GameObject obj = Instantiate(prefab, PrefabParent[(int)type]);
                    obj.SetActive(false);
                    poolDictionary[type].Enqueue(obj);
                }
            }
            else
            {
                Debug.LogWarning($"Prefab for type {type} is not assigned in the prefab dictionary.");
            }
        }
    }

    // Method to get an object of a specific type
    public GameObject GetObject(ObjectType type)
    {
        if (poolDictionary.TryGetValue(type, out Queue<GameObject> queue))
        {
            if (queue.Count == 0)
            {
                // If the queue is empty, instantiate a new object and return it
                if (prefabDictionary.TryGetValue(type, out GameObject prefab))
                {
                    GameObject newObject = Instantiate(prefab);
                    newObject.SetActive(true);
                    return newObject;
                }
                else
                {
                    Debug.LogWarning($"Prefab for type {type} is not assigned in the prefab dictionary.");
                    return null;
                }
            }
            else
            {
                // If there are objects in the queue, dequeue and return one
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        else
        {
            Debug.LogWarning($"Queue for type {type} does not exist in the pool dictionary.");
            return null;
        }
    }

    // Method to return an object to the pool
    public void ReturnObject(ObjectType type, GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.TryGetValue(type, out Queue<GameObject> queue))
        {
            queue.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Queue for type {type} does not exist in the pool dictionary.");
        }
    }
}