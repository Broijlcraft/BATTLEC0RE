using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile {

    public float explosionForce, explosionRange;
    float damage;
    public Explosion explosionInfo;
    int playerID;

    public override void Launch(int playerID, float _damage, float _range, float projectileSpeed, bool _isAffectedByGravity, int photonViewID) {
        damage = _damage;
        this.playerID = playerID;
        base.Launch(playerID, _damage, _range, projectileSpeed, _isAffectedByGravity, photonViewID);
    }

    public override void OutOfRange() {
        base.OutOfRange();
        explosionInfo.Explode(playerID, transform.position, transform.rotation, damage, explosionRange, explosionForce);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}