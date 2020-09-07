using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : FireArms {

    public GameObject projectile;
    public float projectileSpeed;

    public override void ShootBehaviour(WeaponBehaviour behaviour, Transform attackOrigin) {
        GameObject projObject = Instantiate(projectile, attackOrigin);
        Projectile proj = projObject.GetComponent<Projectile>();
        Vector3 attackRotation = GetAttackRotation(attackOrigin, behaviour.range);
        proj.Launch(attackRotation, behaviour, projectileSpeed);
    }
}