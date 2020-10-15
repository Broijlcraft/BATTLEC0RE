using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ObjectPool : MonoBehaviourPun {

    public static ObjectPool single_PT;
    
    [SerializeField] private List<Pool> unSyncedPools = new List<Pool>();
    [SerializeField] private List<Pool> syncedPools = new List<Pool>();

    [Header("HideInInspector")]
    public List<PlayerPool> playerPools = new List<PlayerPool>();
    public int maxPlayers;

    #region Init
    private void Awake() {
        single_PT = this;
        playerPools.Clear();
        if (MultiplayerSetting.single_MPS) {
            maxPlayers = MultiplayerSetting.single_MPS.maxPlayers;
            for (int i = 0; i < maxPlayers-1; i++) {
                Dictionary<string, Queue<GameObject>> tempUnSyncedPoolDict = CreatePoolDictFromPoolList(unSyncedPools);
                Dictionary<string, Queue<GameObject>> tempSyncedPoolDict = CreatePoolDictFromPoolList(syncedPools);
                PlayerPool pp = new PlayerPool { unSyncedPoolDictionary = tempUnSyncedPoolDict, syncedPoolDictionary = tempSyncedPoolDict};
                playerPools.Add(pp);
            }
        }
    }

    Dictionary<string, Queue<GameObject>> CreatePoolDictFromPoolList(List<Pool> poolList) {
        Dictionary<string, Queue<GameObject>> tempPoolDictionary = new Dictionary<string, Queue<GameObject>>();
        for (int poolCount = 0; poolCount < poolList.Count; poolCount++) {
            if (poolList[poolCount].prefab) {
                Queue<GameObject> tempPool = new Queue<GameObject>();
                for (int amount = 0; amount < poolList[poolCount].amountPerPlayer; amount++) {
                    GameObject poolObject = Instantiate(poolList[poolCount].prefab, -Vector3.one, Quaternion.identity);
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

    public void SetPoolOwners(int playerID, int photonViewID) {
        photonView.RPC("RPC_SetPoolOwners", RpcTarget.All, playerID, photonViewID);
    }

    [PunRPC]
    void RPC_SetPoolOwners(int playerID, int photonViewID) {
        playerPools[playerID].view = PhotonNetwork.GetPhotonView(photonViewID);
    }

    #endregion

    public void GlobalSpawnProjectile(int playerID, string tag, Vector3 pos, Quaternion rot, int damage, float range, float projectileSpeed, bool _isAffectedByGravity, SyncType poolSyncType) {
        int isAffectedByGravity = Tools.BoolToInt(_isAffectedByGravity);
        int syncType = (int)poolSyncType;
        photonView.RPC("RPC_GlobalSpawnProjectile", RpcTarget.All, playerID, tag, pos, rot, damage, range, projectileSpeed, isAffectedByGravity, syncType);
    }

    [PunRPC]
    void RPC_GlobalSpawnProjectile(int playerID, string tag, Vector3 pos, Quaternion rot, int damage, float range, float projectileSpeed, int _isAffectedByGravity, SyncType poolSyncType) {
        bool isAffectedByGravity = Tools.IntToBool(_isAffectedByGravity);
        GameObject projObject = SpawnFromPool(playerID, tag, pos, rot, poolSyncType);
        if (projObject) {
            Projectile proj = projObject.GetComponent<Projectile>();
            proj.Launch(playerID, damage, range, projectileSpeed, isAffectedByGravity);
        }
    }

    public void GlobalSpawnFromPool(int playerID, string tag, Vector3 pos, Quaternion rot, SyncType poolSyncType) {
        int syncType = (int)poolSyncType;
        photonView.RPC("RCP_SpawnFromPool", RpcTarget.All, playerID, tag, pos, rot, syncType);
    }

    [PunRPC]
    void RCP_SpawnFromPool(int playerID, string tag, Vector3 pos, Quaternion rot, int syncType) {
        SyncType poolSyncType = (SyncType)syncType;
        SpawnFromPool(playerID, tag, pos, rot, poolSyncType);
    }

    public GameObject SpawnFromPool(int playerID, string tag, Vector3 pos, Quaternion rot, SyncType poolSyncType) {
        GameObject returnObject = GetPooledObject(playerID, tag, poolSyncType);

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

    public PhotonView GetPhotonView(int playerID) {
        return playerPools[playerID].view;
    }

    GameObject GetPooledObject(int playerID, string tag, SyncType poolSyncType) {
        GameObject pooledObject = null;

        PlayerPool poolToTakeFrom = playerPools[playerID];
        if(poolSyncType == SyncType.Synced) {
            if (poolToTakeFrom.syncedPoolDictionary.ContainsKey(tag)) {
                pooledObject = poolToTakeFrom.syncedPoolDictionary[tag].Dequeue();
                poolToTakeFrom.syncedPoolDictionary[tag].Enqueue(pooledObject);
            } else {
                Debug.LogWarning($"SyncedPoolDictionary with tag {tag} doesn't exist");
            }
        } else {
            if (poolToTakeFrom.unSyncedPoolDictionary.ContainsKey(tag)) {
                pooledObject = poolToTakeFrom.unSyncedPoolDictionary[tag].Dequeue();
                poolToTakeFrom.unSyncedPoolDictionary[tag].Enqueue(pooledObject);
            } else {
                Debug.LogWarning($"UnSyncedPoolDictionary with tag {tag} doesn't exist");
            }
        }

        return pooledObject;
    }
}

[System.Serializable]
public class PlayerPool {
    public PhotonView view;
    public Dictionary<string, Queue<GameObject>> unSyncedPoolDictionary = new Dictionary<string, Queue<GameObject>>();
    public Dictionary<string, Queue<GameObject>> syncedPoolDictionary = new Dictionary<string, Queue<GameObject>>();
}

public enum SyncType {
    Synced,
    UnSynced
}

[System.Serializable]
public class Pool {
    public GameObject prefab;
    public int amountPerPlayer;
}