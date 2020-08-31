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
        if(weaponType != WeaponType.Primary || (Manager.single_M && Manager.single_M.dev)) {
            base.Interact(controller);
            if (interactingController == controller) {
                controller.weaponsController.AttachDetachWeapon(this, true);
            }
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
    public Transform[] attackOrigins;
    public AttackType attackType;

    public float damagePerAttack;
    public int attacksPerSecond;

    [HideInInspector] public int ao_Index;
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