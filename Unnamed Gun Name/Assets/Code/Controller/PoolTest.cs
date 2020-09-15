using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PoolTest : MonoBehaviourPun {

    public static PoolTest single_PT;
    
    [SerializeField] private List<Pool> unSyncedPools = new List<Pool>();
    [SerializeField] private List<Pool> syncedPools = new List<Pool>();

    public List<PlayerPool> playerPool = new List<PlayerPool>();
    public int maxPlayers;

    private void Awake() {
        single_PT = this;
        if (MultiplayerSetting.single_MPS) {
            maxPlayers = MultiplayerSetting.single_MPS.maxPlayers;
            for (int i = 0; i < maxPlayers-1; i++) {
                Dictionary<string, Queue<GameObject>> tempUnSyncedPoolDict = CreatePoolDictFromPoolList(unSyncedPools);
                Dictionary<string, Queue<GameObject>> tempSyncedPoolDict = CreatePoolDictFromPoolList(syncedPools);
                PlayerPool pp = new PlayerPool { unSyncedPoolDictionary = tempUnSyncedPoolDict, syncedPoolDictionary = tempSyncedPoolDict};
                playerPool.Add(pp);
            }
        }
    }

    private void Start() {
        for (int i = 0; i < maxPlayers; i++) {

        }
    }

    Dictionary<string, Queue<GameObject>> CreatePoolDictFromPoolList(List<Pool> poolList) {
        Dictionary<string, Queue<GameObject>> tempPoolDictionary = new Dictionary<string, Queue<GameObject>>();
        for (int poolCount = 0; poolCount < poolList.Count; poolCount++) {
            if (poolList[poolCount].prefab) {
                Queue<GameObject> tempPool = new Queue<GameObject>();
                for (int amount = 0; amount < poolList[poolCount].amountPerPlayer; amount++) {
                    GameObject poolObject = Instantiate(poolList[poolCount].prefab, Vector3.zero, Quaternion.identity);
                    poolObject.SetActive(false);
                    poolObject.transform.SetParent(transform);
                    poolObject.name = poolObject.name += amount;
                    tempPool.Enqueue(poolObject);
                }
                tempPoolDictionary.Add(poolList[poolCount].prefab.name, tempPool);
            }
        }
        return tempPoolDictionary;
    }

    public GameObject SpawnFromPool(int playerId, string tag, Vector3 pos, Quaternion rot) {
        GameObject returnObject = null;
        if (playerPool[1].syncedPoolDictionary.ContainsKey(tag)) {
            GameObject objectToSpawn = playerPool[1].syncedPoolDictionary[tag].Dequeue();

            objectToSpawn.transform.rotation = rot;
            objectToSpawn.transform.position = pos;
            objectToSpawn.SetActive(true);
            returnObject = objectToSpawn;

            IPoolObject poolObject = objectToSpawn.GetComponent(typeof(IPoolObject)) as IPoolObject;

            if (poolObject != null) {
                poolObject.OnObjectSpawn();
            }
            playerPool[1].syncedPoolDictionary[tag].Enqueue(objectToSpawn);

        } else {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist");
        }
        return returnObject;
    }
}

[System.Serializable]
public class PlayerPool {

    [Header("HideInInspector")]
    public int photonviewId;
    public Dictionary<string, Queue<GameObject>> unSyncedPoolDictionary = new Dictionary<string, Queue<GameObject>>();
    public Dictionary<string, Queue<GameObject>> syncedPoolDictionary = new Dictionary<string, Queue<GameObject>>();
}

public enum SyncType {
    Synced,
    UnSynced
}