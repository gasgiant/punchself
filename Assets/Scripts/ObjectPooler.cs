using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    class PooledObject
    {
        public GameObject go;
        public Transform tr;

        public PooledObject(GameObject _go, Transform _tr)
        {
            go = _go;
            tr = _tr;
        }
    }

    public static ObjectPooler Instance;

    public List<Pool> pools;


    Dictionary<string, Queue<PooledObject>> poolsDictionary;

    void Awake()
    {
        Instance = this;

        poolsDictionary = new Dictionary<string, Queue<PooledObject>>();

        foreach (Pool pool in pools)
        {
            Queue<PooledObject> objectQueue = new Queue<PooledObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject go = Instantiate(pool.prefab);
                objectQueue.Enqueue(new PooledObject(go, go.transform));
                go.SetActive(false);
            }
            poolsDictionary.Add(pool.tag, objectQueue);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolsDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        PooledObject po = poolsDictionary[tag].Dequeue();
        int cap = 0;
        while (po.go.activeInHierarchy)
        {
            if (cap >= poolsDictionary[tag].Count)
            {
                Debug.LogWarning("All objects of pool " + tag + " are buisy!");
                break;
            }
            poolsDictionary[tag].Enqueue(po);
            po = poolsDictionary[tag].Dequeue();
            cap++;
        }

        po.go.SetActive(false);
        po.tr.position = position;
        po.tr.rotation = rotation;
        po.go.SetActive(true);

        poolsDictionary[tag].Enqueue(po);
        return po.go;
    }

}
