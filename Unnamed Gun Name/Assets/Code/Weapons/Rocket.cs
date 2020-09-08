using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile {

    public float explosionForce, explosionRange;

    public Explosion explosionInfo;

    public override void Launch(Vector3 targetPos, WeaponBehaviour behaviour, float projectileSpeed) {
        explosionInfo.Init(transform, behaviour.damagePerAttack, explosionRange, explosionForce);
        base.Launch(targetPos, behaviour, projectileSpeed);
    }

    public override void OutOfRange() {
        base.OutOfRange();
        explosionInfo.Explode();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}