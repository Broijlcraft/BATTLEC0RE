using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public float damage = 5f, range = 2f, force = 10f;

    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            Explode();
        }
    }

    public void Explode() {
        Collider[] colls = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colls.Length; i++) {
            Controller controller;
            if (colls[i].transform.parent) {
                controller = colls[i].gameObject.GetComponentInChildren<Controller>();
            } else {
                controller = colls[i].gameObject.GetComponent<Controller>();
            }
            if (controller) {
                print("Boom");
                controller.health.DoDamage(damage);
                controller.rigid.AddExplosionForce(force, transform.position, range);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
