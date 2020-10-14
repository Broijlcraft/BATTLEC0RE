using UnityEngine;
using System.Collections.Generic;

public class Weapon : Interactable {

    public WeaponType weaponType;
    public WeaponBehaviour[] weaponBehaviours = new WeaponBehaviour[1];

    [HideInInspector] public Collider[] colliders;
    /*[HideInInspector] */
    public List<GameObject> meshObjects = new List<GameObject>();

    //Dev
    Vector3 startPos;
    Quaternion startRot;
    Transform startParent;

    public virtual void Awake() {
        colliders = GetComponentsInChildren<Collider>();
    }

    private void Start() {
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }

    public override void Interact(Controller controller) {
        if (weaponType != WeaponType.Primary) {
            CheckAndAttach(controller);
        } else if (weaponType == WeaponType.Primary && !controller.weaponsController.primaryWeaponsHolder.weaponAttached) {
            CheckAndAttach(controller);
        }
    }

    void CheckAndAttach(Controller controller) {
        base.Interact(controller);
        if (interactingController == controller) {
            controller.weaponsController.AttachDetachWeapon(this, true, true);
        }
    }

    public override void Use() {

    }

    public virtual void Attach(Transform newParent) {
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    //Dev
    public void ResetPosAndRot() {
        transform.position = startPos;
        transform.rotation = startRot;
        if (startParent) {
            transform.SetParent(startParent);
        }
    }

    public virtual int GetBehaviourIndex(Weapon weapon) {
        int index = 0;

        return index;
    }
}

[System.Serializable]
public class WeaponBehaviour {
    [Space]
    public AttackOrigin[] attackOrigins;
    [Space]
    public AttackType attackType;
    public GameObject impactEffect;
    [Space]
    public int damagePerAttack = 1;
    public float range = 1f;
    public float attacksPerSecond = 1f;
    
    public Vector2 bulletSpread;

    [HideInInspector] public int currentAo;
    [HideInInspector] public bool canNotAttack;
}

[System.Serializable]
public class AttackOrigin {

    public Transform origin;
    public Animator animator;
    public AttackEffect attackEffect;
}

[System.Serializable]
public class AttackEffect {
    public ParticleSystem attackParticle;
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