using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryAndSecondaryWeapon : Weapon {

    [Space]
    public ActiveWeapon currentActiveWeapon;
    public float timeToSwitch = 1f;

    public override void Use() {
        print("Bang! " + currentActiveWeapon);
    }
}

public enum ActiveWeapon {
    primary = 0,
    secondary = 1
}