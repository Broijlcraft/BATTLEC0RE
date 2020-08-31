using UnityEngine;

public class Weapon : Interactable {

    public FireMode fireMode;
    public WeaponType weaponType;

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

public enum FireMode {
    Automatic,
    SemiAutomatic
}

public enum WeaponType {
    Primary,
    Power
}