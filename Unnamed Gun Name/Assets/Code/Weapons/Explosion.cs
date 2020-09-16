using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Explosion {
    [HideInInspector] public Vector3 originPosition;
    [HideInInspector] public Quaternion originRotation;
    [HideInInspector] public float damage, explosionRange, explosionForce;

    public float particleDestroyTime;
    public GameObject explosionPrefab;
    
    public void Explode(int playerID, Vector3 _originPosition, Quaternion _originRotation, float _damage, float _explosionRange, float _explosionForce) {
        SetValues(_originPosition, _originRotation, _damage, _explosionRange, _explosionForce);
        PlayParticles(playerID);
        Collider[] colls = Physics.OverlapSphere(originPosition, explosionRange);
        for (int i = 0; i < colls.Length; i++) {
            Controller controller;
            if (colls[i].transform.parent) {
                controller = colls[i].gameObject.GetComponentInChildren<Controller>();
            } else {
                controller = colls[i].gameObject.GetComponent<Controller>();
            }
            if (controller) {
                controller.health.DoDamage(damage);
                controller.rigid.AddExplosionForce(explosionForce, originPosition, explosionRange);
            }
        }
    }

    void SetValues(Vector3 _originPosition, Quaternion _originRotation, float _damage, float _explosionRange, float _explosionForce) {
        originPosition = _originPosition;
        originRotation = _originRotation;
        damage = _damage;
        explosionRange = _explosionRange;
        explosionForce = _explosionForce;
    }

    void PlayParticles(int playerID) {
        ObjectPool.single_PT.SpawnFromPool(playerID, explosionPrefab.name, originPosition, originRotation, SyncType.UnSynced);
    }
}