using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler single_OP;

    public List<Pool> pools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake() {
        single_OP = this;
    }

    private void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        for (int i = 0; i < pools.Count; i++) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            if (pools[i].prefab) {
                for (int iB = 0; iB < pools[i].poolSize; iB++) {
                    GameObject poolObject = Instantiate(pools[i].prefab);
                    poolObject.SetActive(false);
                    poolObject.transform.SetParent(transform);
                    objectPool.Enqueue(poolObject);
                }
                poolDictionary.Add(pools[i].tag, objectPool);
            }
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot) {
        GameObject returnObject = null;
        if (poolDictionary.ContainsKey(tag)) {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.transform.rotation = rot;
            objectToSpawn.transform.position = pos;
            objectToSpawn.SetActive(true);
            returnObject = objectToSpawn;

            IPoolObject poolObject = objectToSpawn.GetComponent(typeof(IPoolObject)) as IPoolObject;

            if (poolObject != null) {
                poolObject.OnObjectSpawn();
            }

            poolDictionary[tag].Enqueue(objectToSpawn);
        } else {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist");
        }

        return returnObject;
    }

}

[System.Serializable]
public class Pool {
    public string tag;
    public GameObject prefab;
    public int poolSize;
}