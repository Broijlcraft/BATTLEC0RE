using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        Health health = other.gameObject.GetComponent<Health>();
        if (health) {
            health.DoDamage(health.maxHealth, "the world");
        }
    }
}