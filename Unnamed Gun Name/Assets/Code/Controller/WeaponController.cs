using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class WeaponController : MonoBehaviourPun {

    public WeaponsHolder primaryWeaponsHolder, powerWeaponsHolder;

    float animationSpeed;
    [HideInInspector] public Controller controller;
    [HideInInspector] public bool isAttaching, isDetaching, isChangingBehaviour;

    public void Init(Controller control) {
        controller = control;
        primaryWeaponsHolder.Init(controller);
        powerWeaponsHolder.Init(controller);
    }

    private void Start() {
        if (primaryWeaponsHolder.weaponAttached) {
            primaryWeaponsHolder.weaponAttached.interactingController = controller;
            AttachDetachWeapon(primaryWeaponsHolder.weaponAttached, false, false);
        }
        if (powerWeaponsHolder.weaponAttached) {
            powerWeaponsHolder.weaponAttached.interactingController = controller;
            AttachDetachWeapon(powerWeaponsHolder.weaponAttached, false, false);
        }
    }

    private void Update() {
        if (!controller.health.isDead && PhotonNetwork.IsConnected) {
            WeaponSwitchCheck();
            PrimaryAndPowerInputCheckAndUse(1, powerWeaponsHolder);
            PrimaryAndPowerInputCheckAndUse(0, primaryWeaponsHolder);
        }
    }

    void WeaponSwitchCheck() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(!isAttaching && !isDetaching && scroll != 0 && primaryWeaponsHolder.weaponAttached && controller.IsMineAndAliveCheck()) {
            if (!isChangingBehaviour) {
                Weapon weapon = primaryWeaponsHolder.weaponAttached;
                int behaviourIndex = weapon.GetBehaviourIndex(weapon);
                InteractableActions.single_IA.SwitchWeaponBehaviour(photonView.ViewID, behaviourIndex);
            }
        }
    }

    public IEnumerator SwitchWeaponBehaviour(int behaviourIndex) {
        isChangingBehaviour = true;
        FireArms weapon = primaryWeaponsHolder.weaponAttached as FireArms;
        if (behaviourIndex == 0) {
            SwitchBehaviour(weapon, "PrimToSec", ActiveWeapon.secondary);
        } else {
            SwitchBehaviour(weapon, "SecToPrim", ActiveWeapon.primary);
        }
        yield return new WaitForSeconds(weapon.timeToSwitchBehaviour);
        isChangingBehaviour = false;
    }

    void SwitchBehaviour(FireArms weapon, string triggerString, ActiveWeapon newActiveWeapon) {
        primaryWeaponsHolder.animator.SetTrigger(triggerString);
        weapon.currentActiveWeapon = newActiveWeapon;
    }

    void PrimaryAndPowerInputCheckAndUse(int mouseInput, WeaponsHolder holder) {
        if (!isAttaching && !isDetaching && controller.IsMineAndAliveCheck()) {
            Weapon weapon = holder.weaponAttached;
            if (weapon && (weapon.weaponType != WeaponType.Primary || (weapon.weaponType == WeaponType.Primary && !isChangingBehaviour))) {
                bool buttonPressed = false;
                int behaviourIndex = weapon.GetBehaviourIndex(weapon);
                switch (weapon.weaponBehaviours[behaviourIndex].attackType) {
                    case AttackType.Automatic:
                    if (Input.GetMouseButton(mouseInput)) {
                        buttonPressed = true;
                    }
                    break;
                    case AttackType.SemiAutomatic:
                    if (Input.GetMouseButtonDown(mouseInput)) {
                        buttonPressed = true;
                    }
                    break;
                }
                if (buttonPressed) {
                    weapon.Use();
                }
            }
        }
    }

    public void AttachDetachWeapon(Weapon weapon, bool useAnim, bool allowDetach) {
        if (!isAttaching && !isDetaching) {
            WeaponsHolder holder = GetHolder(weapon.weaponType);
            StartCoroutine(CheckForAndSetAttached(holder, weapon, useAnim, allowDetach));
        }
    }

    public WeaponsHolder GetHolder(WeaponType type) {
        WeaponsHolder holder = new WeaponsHolder();
        switch (type) {
            case WeaponType.Primary:
            holder = primaryWeaponsHolder;
            break;
            case WeaponType.Power:
            holder = powerWeaponsHolder;
            break;
        }
        return holder;
    }

    IEnumerator CheckForAndSetAttached(WeaponsHolder holder, Weapon weapon, bool useAnim, bool allowDetach) { 
        if (!isAttaching && !isDetaching) {
            float extraAttachWaitTime = 0f;
            if (holder.weaponAttached && allowDetach) {
                extraAttachWaitTime = holder.timeToDetach;
                animationSpeed = 1 / holder.timeToDetach;
                StartCoroutine(Detach(holder, useAnim));
            }
            if (controller.IsMineCheck()) {
                Tools.SetLocalOrGlobalLayers(weapon.meshObjects.ToArray(), false);
            }
            if (useAnim) {
                yield return new WaitForSeconds(extraAttachWaitTime);
            }
            animationSpeed = 1 / holder.timeToAttach;
            StartCoroutine(Attach(holder, weapon, useAnim));
        }
    }

    IEnumerator Attach(WeaponsHolder holder, Weapon weapon, bool useAnim) {
        if (weapon) {
            isAttaching = true;
            weapon.transform.SetParent(holder.weaponsHolder);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            if (useAnim) {
                holder.animator.speed = animationSpeed;
                holder.animator.SetTrigger("ScrewOn");
                yield return new WaitForSeconds(holder.timeToAttach);
            }
        }
        holder.weaponAttached = weapon;
        isAttaching = false;
    }

    IEnumerator Detach(WeaponsHolder holder, bool useAnim) {
        print("Detach");
        isDetaching = true;
        if (useAnim) {
            holder.animator.speed = animationSpeed;
            holder.animator.SetTrigger("ScrewOff");
            yield return new WaitForSeconds(holder.timeToDetach);
        }
        if (controller.IsMineCheck()) {
            Tools.SetLocalOrGlobalLayers(holder.weaponAttached.meshObjects.ToArray(), true);
        }
        holder.weaponAttached.transform.SetParent(null);
        holder.weaponAttached.ResetPosAndRot();
        holder.weaponAttached.interactingController = null;
        holder.weaponAttached = null;
        isDetaching = false;
    }
}

[System.Serializable]
public class WeaponsHolder {

    public WeaponType weaponType;
    public float timeToAttach = 2f, timeToDetach = 1f;

    [Header("HideInInspector")]
    public Weapon weaponAttached;
    public Transform weaponsHolder;
    /*[HideInInspector] */public Animator animator;

    public void Init(Controller controller) {
        BodypartType partType = GetHolderBodypart();

        if (weaponsHolder) {
            animator = weaponsHolder.GetComponent<Animator>();
        }
    }

    BodypartType GetHolderBodypart() {
        BodypartType partType = BodypartType.PrimaryWeaponHolder;
        if(weaponType == WeaponType.Power) {
            partType = BodypartType.PowerWeaponHolder;
        }
        return partType;
    }
}