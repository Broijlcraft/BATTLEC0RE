using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Explosion {
    [HideInInspector] public Transform origin;
    [HideInInspector] public float damage, explosionRange, explosionForce;

    public float particleDestroyTime;
    public string explosionParticleName;
    
    public void Init(Transform _origin, float _damage, float _explosionRange, float _explosionForce) {
        origin = _origin;
        damage = _damage;
        explosionRange = _explosionRange;
        explosionForce = _explosionForce;
    }

    public void Explode() {
        PlayParticles();
        Collider[] colls = Physics.OverlapSphere(origin.position, explosionRange);
        for (int i = 0; i < colls.Length; i++) {
            Controller controller;
            if (colls[i].transform.parent) {
                controller = colls[i].gameObject.GetComponentInChildren<Controller>();
            } else {
                controller = colls[i].gameObject.GetComponent<Controller>();
            }
            if (controller) {
                controller.health.DoDamage(damage);
                controller.rigid.AddExplosionForce(explosionForce, origin.position, explosionRange);
            }
        }
    }

    void PlayParticles() {
        GameObject parObject = ObjectPooler.single_OP.SpawnFromPool(explosionParticleName, origin.position, origin.rotation);
    }
}