using System.Collections;
using UnityEngine;

public class FireArms : Weapon {

    [Space]
    public float timeToSwitchBehaviour = 1f;

    [Header("For projectile based arms")]
    public Projectile projectile;
    
    [HideInInspector] public ActiveWeapon currentActiveWeapon = ActiveWeapon.primary;

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

        //AttackOrigin attackOrigin = behaviour.attackOrigins[behaviour.currentAo];
        if (behaviour.currentAo == behaviour.attackOrigins.Length - 1) {
            behaviour.currentAo = 0;
        } else {
            behaviour.currentAo++;
        }

        RaycastHit hit;
        if (Physics.Raycast(interactingController.cam.transform.position, interactingController.cam.transform.forward, out hit, behaviour.range, ~TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask)) {
            print(hit.transform.name);
            Health health = hit.transform.GetComponent<Health>();
            if (health) {
                health.DoDamage(behaviour.damagePerAttack);
            }
        }
        InteractableActions.ia_Single.PlayFireArmsEffect(index, behaviourIndex, behaviour.currentAo, "Shoot");
        yield return new WaitForSeconds(attackSpeed);
        behaviour.canNotAttack = false;
    }
}

public enum ActiveWeapon {
    primary = 0,
    secondary = 1
}