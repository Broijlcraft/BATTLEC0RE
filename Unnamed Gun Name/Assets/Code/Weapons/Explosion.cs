using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Explosion {
    [HideInInspector] public Transform origin;
    [HideInInspector] public float damage, explosionRange, explosionForce;

    public GameObject[] explosionParticles;
    public float particleDestroyTime;
    
    public void Init(Transform _origin, float _damage, float _explosionRange, float _explosionForce) {
        origin = _origin;
        damage = _damage;
        explosionRange = _explosionRange;
        explosionForce = _explosionForce;
    }

    public void Explode() {
        Debug.Log("Boom");
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
        for (int i = 0; i < explosionParticles.Length; i++) {
            GameObject par = GameObject.Instantiate(explosionParticles[i], origin.position, Quaternion.identity);
            GameObject.Destroy(par.gameObject, particleDestroyTime);
            //explosionParticles[i].Play();
        }
    }
}