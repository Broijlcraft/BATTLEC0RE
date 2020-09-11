using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile {

    public float explosionForce, explosionRange;
    float damage;
    public Explosion explosionInfo;

    public override void Launch(float _damage, float _range, float projectileSpeed, bool _isAffectedByGravity) {
        damage = _damage;
        base.Launch(_damage, _range, projectileSpeed, _isAffectedByGravity);
    }

    public override void OutOfRange() {
        base.OutOfRange();
        explosionInfo.Explode(transform.position, transform.rotation, damage, explosionRange, explosionForce);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}