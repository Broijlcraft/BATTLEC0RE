using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ProjectileWeapon : FireArms {

    public GameObject projectilePrefab;
    public float projectileSpeed;
    public bool isAffectedByGravity;

    public override void ShootBehaviour(Transform attackOrigin) {

        Vector3 attackRotation = GetAttackRotation(attackOrigin, wBehaviour.range);
        Quaternion actualRotation = Quaternion.LookRotation(attackRotation);
        Vector3 pos = transform.position;
        int myNumber = PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom;
        float dmg = wBehaviour.damagePerAttack;
        float range = wBehaviour.range;
        int viewID = interactingController.photonView.ViewID;
        ObjectPool.single_PT.GlobalSpawnProjectile(myNumber, projectilePrefab.name, pos, actualRotation, dmg, range, projectileSpeed, isAffectedByGravity, viewID, SyncType.Synced);
    }
}