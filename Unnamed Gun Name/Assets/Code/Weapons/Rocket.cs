using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile {

    public float explosionForce, explosionRange;

    public Explosion explosionInfo;
    
    public override void OutOfRange() {
        base.OutOfRange();
        explosionInfo.Explode(transform.position, transform.rotation, 0f, explosionRange, explosionForce);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}