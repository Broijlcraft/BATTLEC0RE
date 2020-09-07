using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class WeaponController : MonoBehaviourPun {

    public WeaponsHolder primaryWeaponsHolder, powerWeaponsHolder;

    public WeaponsHolder[] weaponsHolders;

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
        }
    }

    private void Update() {
        if (!controller.health.isDead) {
            WeaponSwitchCheck();
            PrimaryAndPowerInputCheckAndUse(1, powerWeaponsHolder);
            PrimaryAndPowerInputCheckAndUse(0, primaryWeaponsHolder);
        }
    }

    void WeaponSwitchCheck() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(!isAttaching && !isDetaching && scroll != 0 && primaryWeaponsHolder.weaponAttached && photonView.IsMine) {
            if (!isChangingBehaviour) {
                int behaviourIndex = InteractableActions.ia_Single.GetBehaviourIndex(primaryWeaponsHolder.weaponAttached);
                InteractableActions.ia_Single.SwitchWeaponBehaviour(photonView.ViewID, behaviourIndex);
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
        if (!isAttaching && !isDetaching && photonView.IsMine) {
            Weapon weapon = holder.weaponAttached;
            if (weapon && (weapon.weaponType != WeaponType.Primary || (weapon.weaponType == WeaponType.Primary && !isChangingBehaviour))) {
                bool buttonPressed = false;
                int behaviourIndex = InteractableActions.ia_Single.GetBehaviourIndex(weapon);
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

    public void AttachDetachWeapon(Weapon weapon, bool useAnim) {
        if (!isAttaching && !isDetaching) {
            WeaponsHolder holder = GetHolder(weapon.weaponType);
            StartCoroutine(CheckForAndSetAttached(holder, weapon, useAnim));
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

    IEnumerator CheckForAndSetAttached(WeaponsHolder holder, Weapon weapon, bool useAnim) {
        if (!isAttaching && !isDetaching) {
            float extraAttachWaitTime = 0f;
            if (holder.weaponAttached) {
                extraAttachWaitTime = holder.timeToDetach;
                animationSpeed = 1 / holder.timeToDetach;
                StartCoroutine(Detach(holder, useAnim));
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
            if (photonView.IsMine) {
                Tools.SetLocalOrGlobalLayers(weapon.meshObjects.ToArray(), false);
            }
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
        isDetaching = true;
        if (useAnim) {
            holder.animator.speed = animationSpeed;
            holder.animator.SetTrigger("ScrewOff");
            yield return new WaitForSeconds(holder.timeToDetach);
        }
        if (photonView.IsMine) {
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
    [HideInInspector] public Animator animator;

    public void Init(Controller controller) {
        BodypartType partType = GetHolderBodypart();

        if (controller.robotParts) {
            int partIndex = controller.robotParts.GetPartIndex(partType);
            if(partIndex >= 0) {
                weaponsHolder = controller.robotParts.parts[partIndex].transform;
            }
        }

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