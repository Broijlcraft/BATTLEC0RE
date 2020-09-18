using System.Collections;
using UnityEngine;

public class FireArms : Weapon {

    [Space]
    public float timeToSwitchBehaviour = 1f;
        
    [HideInInspector] public ActiveWeapon currentActiveWeapon = ActiveWeapon.primary;
    [HideInInspector] public WeaponBehaviour wBehaviour;

    public override void Use() {
        int behaviourIndex = 0;
        if(weaponType == WeaponType.Primary) {
            behaviourIndex = (int)currentActiveWeapon;
        }
        if (!weaponBehaviours[behaviourIndex].canNotAttack) {
            StartCoroutine(Shoot(behaviourIndex));
        }
    }

    public virtual IEnumerator Shoot(int behaviourIndex) {
        WeaponBehaviour behaviour = weaponBehaviours[behaviourIndex];
        behaviour.canNotAttack = true;
        float attackSpeed = 1 / behaviour.attacksPerSecond;

        if (behaviour.currentAo == behaviour.attackOrigins.Length - 1) {
            behaviour.currentAo = 0;
        } else {
            behaviour.currentAo++;
        }

        AttackOrigin origin = behaviour.attackOrigins[behaviour.currentAo];
        wBehaviour = behaviour;
        ShootBehaviour(origin.origin);

        origin.animator.speed = behaviour.attacksPerSecond;
        origin.animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(attackSpeed);
        behaviour.canNotAttack = false;
    }

    public virtual void ShootBehaviour(Transform attackOrigin) {
        RaycastHit hit;
        Vector3 attackRot = GetAttackRotation(attackOrigin, wBehaviour.range);

        if (Physics.Raycast(attackOrigin.position, attackRot, out hit, wBehaviour.range, ~TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask)) {
            Health health = hit.transform.GetComponent<Health>();
            if (health) {
                health.DoDamage(wBehaviour.damagePerAttack, PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(interactingController.photonView.Owner.NickName));
            }
        }
    }

    public Vector3 GetAttackRotation(Transform origin, float range) {
        Vector3 attackPos = interactingController.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, range));

        //this is to check if something is in front of the camera before the ray reaches the max range and use that as target instead
        RaycastHit hit;
        if (Physics.Raycast(interactingController.cam.transform.position, interactingController.cam.transform.forward, out hit, range)) {
            attackPos = hit.point;
        }

        Vector3 dir = attackPos - origin.position;
        return dir;
    }
}

public enum ActiveWeapon {
    primary = 0,
    secondary = 1
}