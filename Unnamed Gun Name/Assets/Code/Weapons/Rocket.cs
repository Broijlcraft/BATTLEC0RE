using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rocket : Projectile {

    public float explosionForce, explosionRange;
    float damage;
    public Explosion explosionInfo;
    int playerID;

    public override void Launch(int playerID_, float _damage, float _range, float projectileSpeed, bool _isAffectedByGravity, int photonViewID) {
        playerID = playerID_;
        base.Launch(playerID_, _damage, _range, projectileSpeed, _isAffectedByGravity, photonViewID);

        if (Tools.OwnerCheck(PhotonNetwork.GetPhotonView(photonViewID))) {
            damage = _damage;
        } else {
            damage = 0;
        }
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