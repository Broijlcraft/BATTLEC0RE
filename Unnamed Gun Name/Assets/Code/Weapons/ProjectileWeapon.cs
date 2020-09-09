using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : FireArms {

    public string projectileName;
    public float projectileSpeed;


    public override void ShootBehaviour(WeaponBehaviour behaviour, Transform attackOrigin) {
        Vector3 attackRotation = GetAttackRotation(attackOrigin, behaviour.range);
        GameObject projObject = ObjectPooler.single_OP.SpawnFromPool(projectileName, attackOrigin.position, Quaternion.Euler(attackRotation));
        Projectile proj = projObject.GetComponent<Projectile>();
        proj.Launch(attackRotation, behaviour, projectileSpeed);
    }
}