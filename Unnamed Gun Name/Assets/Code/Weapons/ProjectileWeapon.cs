using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ProjectileWeapon : FireArms {

    public GameObject projectilePrefab;
    public float projectileSpeed;
    public bool isAffectedByGravity;
    PhotonView myView;

    private void Awake() {
        myView = GetComponent<PhotonView>();
    }

    public override void ShootBehaviour(Transform attackOrigin) {
        Vector3 attackRotation = GetAttackRotation(attackOrigin, wBehaviour.range);
        Quaternion actualRotation = Quaternion.LookRotation(attackRotation);
        Vector3 pos = transform.position;
        ObjectPooler.single_OP.GlobalSpawnProjectile(projectilePrefab.name, pos, actualRotation, wBehaviour.damagePerAttack, wBehaviour.range, projectileSpeed, isAffectedByGravity, myView.ViewID);
    }
}