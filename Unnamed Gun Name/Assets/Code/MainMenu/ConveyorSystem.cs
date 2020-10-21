using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConveyorSystem : MonoBehaviour {
    public Transform pointHolder;
    public Transform[] points;

    public int amountOfModels;
    public GameObject prefab;
    public float firstSpawnDelay = 1f, timeBetweenObjects = 1f;
    int currentPoint;
    public Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

    private void Awake() {
        Queue<GameObject> tempPool = new Queue<GameObject>();
        for (int amount = 0; amount < amountOfModels; amount++) {
            GameObject poolObject = Instantiate(prefab, -Vector3.one, Quaternion.identity);
            poolObject.SetActive(false);
            poolObject.transform.SetParent(transform);
            poolObject.name = poolObject.name += amount;
            tempPool.Enqueue(poolObject);
        }
        pool.Add(prefab.name, tempPool);
    }

    private void Start() {
        StartCoroutine(SpawnInit());
    }

    IEnumerator SpawnInit() {
        yield return new WaitForSeconds(firstSpawnDelay);
        InvokeRepeating(nameof(SpawnCart), 0, timeBetweenObjects);
    }

    void SpawnCart() {
        GameObject coObject = SpawnFromPool(prefab.name, points[0].position, Quaternion.identity);
        ConveyorObjects co = coObject.GetComponent<ConveyorObjects>();
        co.Init(this);
    }

    public void SetTarget(ConveyorObjects cart) {
        int index = cart.pointIndex;
        index++;
        if (index < points.Length) {
            cart.targetPoint = points[index];
            cart.pointIndex = index;
        } else { 
            cart.gameObject.SetActive(false);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot) {
        GameObject returnObject = GetPooledObject(tag);

        if (returnObject) {
            returnObject.transform.rotation = rot;
            returnObject.transform.position = pos;
            returnObject.SetActive(true);

            IPoolObject poolObject = returnObject.GetComponent(typeof(IPoolObject)) as IPoolObject;

            if (poolObject != null) {
                poolObject.OnObjectSpawn();
            }
        }
        return returnObject;
    }

    GameObject GetPooledObject(string tag) {
        GameObject pooledObject = null;

        if (pool.ContainsKey(tag)) {
            pooledObject = pool[tag].Dequeue();
            pool[tag].Enqueue(pooledObject);
        } else {
            Debug.LogWarning($"SyncedPoolDictionary with tag {tag} doesn't exist");
        }


        return pooledObject;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ConveyorSystem))]
public class ConveyorSystemEditor : Editor {

    ConveyorSystem target_CS;

    public void OnEnable() {
        target_CS = (ConveyorSystem)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Set points")) {
            List<Transform> points = new List<Transform>();
            int i;
            for (i = 0; i < target_CS.pointHolder.childCount; i++) {
                Transform point = target_CS.pointHolder.GetChild(i);
                point.name = $"Point ({i})";
                points.Add(point);
                EditorUtility.SetDirty(point);
            }
            target_CS.points = points.ToArray();
            Debug.LogWarning($"Successfully set {"Points"}, don't forget to save!");
        }
    }
}
#endif