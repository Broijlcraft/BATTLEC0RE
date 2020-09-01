﻿using UnityEngine;

public class Weapon : Interactable {

    public WeaponType weaponType;
    public WeaponBehaviour[] weaponBehaviours = new WeaponBehaviour[1];

    //Dev
    Vector3 startPos;
    Quaternion startRot;
    Transform startParent;

    private void Start() {
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }

    public override void Interact(Controller controller) {
        if(weaponType != WeaponType.Primary) {
            CheckAndAttach(controller);
        } else if (weaponType == WeaponType.Primary && !controller.weaponsController.primaryWeaponsHolder.weaponAttached) {
            CheckAndAttach(controller);
        }
    }

    void CheckAndAttach(Controller controller) {
        base.Interact(controller);
        if (interactingController == controller) {
            controller.weaponsController.AttachDetachWeapon(this, true);
        }
    }

    public override void Use() {

    }

    //Dev
    public void ResetPosAndRot() {
        transform.position = startPos;
        transform.rotation = startRot;
        if (startParent) {
            transform.SetParent(startParent);
        }
    }
}

[System.Serializable]
public class WeaponBehaviour {
    [Space]
    public AttackOrigin[] attackOrigins;
    [Space]
    public AttackType attackType;

    public float damagePerAttack = 1f, range = 1f, attacksPerSecond = 1f;

    [HideInInspector] public int currentAo;
    [HideInInspector] public bool canNotAttack;
}

[System.Serializable]
public class AttackOrigin {

    public Transform origin;
    public Animator animator;
}

public enum AttackType {
    Automatic,
    SemiAutomatic,
    Melee
}

public enum WeaponType {
    Primary,
    Power
}