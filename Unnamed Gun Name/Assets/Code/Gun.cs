using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : InteractableObject {

    public enum FireMode {
        SemiAutomatic,
        Automatic,
        Burst
    }
    
    public FireMode fireMode;

    public bool testing;
    [Header("HideInInscpector")]
    float coolDown;

    private void Update() {
        if (TestController.tc_Single && TestController.tc_Single.testing) {
            coolDown = 1 / usesPerSecond;
        }

        if ((usingPrimary || currentUseCoolDown > 0) && currentUseCoolDown <= coolDown) {
            currentUseCoolDown += Time.deltaTime;
        }
    }

    public override void PrimaryUse(bool beingHeld) {
        switch (fireMode) {
            case FireMode.SemiAutomatic: case FireMode.Burst:
                if (usingPrimary == false) {
                    FireMechanism();
                }
            break;
            case FireMode.Automatic:
                if (beingHeld) {
                    FireMechanism();
                }
            break;
        }
        usingPrimary = beingHeld;
    }

    void FireMechanism() {
        if(currentUseCoolDown == 0 || currentUseCoolDown > coolDown) {
            Shoot();    
            currentUseCoolDown = 0;
        }
    }

    void Shoot() {
        Debug.LogWarning("Bang!");
    }

    public override void SecondaryUse(bool beingHeld) {

    }
}