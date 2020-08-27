using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldGun : InteractableObject {

//    public enum FireMode {
//        SemiAutomatic,
//        Automatic,
//        Burst
//    }
//    public FireMode fireMode;

//    [Range(0, 1)]
//    public float bulletSpread, bulletSpreadWhenAiming;
//    [Tooltip("Leave empty if player cam is origin")]
//    public Transform origin;
//    public AmmoType bullets;

//    [Range(0.1f, 179f)]
//    public float fovZoom = 50f;

//    [Header("SFX")]
//    public AudioClip shot;
//    public AudioClip empty;

//    [Header("HideInInspector")]
//    public Transform currentFireOrigin;
//    public Controller controller;
//    public bool aiming;

//    int burst = 1;
//    RaycastHit hit;

//    public GameObject bullet;

//    private void Update() {
//        if (beingHeld) {
//            switch (fireMode) {
//                case FireMode.SemiAutomatic:
//                case FireMode.Burst:
//                if (Input.GetMouseButtonDown(0)) {
//                    PrimaryUse();
//                }
//                break;
//                case FireMode.Automatic:
//                if (Input.GetMouseButton(0)) {
//                    PrimaryUse();
//                }
//                break;
//            }

//            if (Input.GetButtonDown("ReloadGun")) {
//                ReloadGun();
//            }

//            //if (primaryUsed) {
//            //    currentCoolDown += Time.deltaTime;
//            //    if (currentCoolDown > 1 / shotPerSecond) {
//            //        currentCoolDown = 0;
//            //        primaryUsed = false;
//            //    }
//            //}
//        }
//    }

//    public override void PrimaryUse() {
//        if (origin) {
//            if (CheckForBullets()) {
//                burst = 1;
//                if (fireMode != FireMode.Burst) {
//                    AudioManager.PlaySound(shot, audioGroup);
//                    Manager.fps_controller.SendOutTrigger(soundRange);
//                    Manager.fps_controller.triggerRange = soundRange;
//                }
//                if (smokeParticle) {
//                    GameObject smoke = Instantiate(smokeParticle, origin.position, origin.rotation);
//                    ParticleSystem part = smoke.GetComponent<ParticleSystem>();
//                    Destroy(smoke, part.main.duration);
//                }
//                if (bullets.burstType.bulletsPerShot == 1) {
//                    Fire();
//                } else {
//                    InvokeRepeating("Fire", 1 / shotPerSecond / bullets.burstType.bulletsPerShot * bullets.burstType.burstDelay, 1 / shotPerSecond / bullets.burstType.bulletsPerShot * bullets.burstType.burstDelay);
//                }
//                scriptableObject.usesLeft--;
//                Manager.UpdateUsesLeftText(true, scriptableObject.usesLeft.ToString());
//                CheckForBullets();
//            } else {
//                if (empty) {
//                    AudioManager.PlaySound(empty, audioGroup);
//                }
//            }
//            primaryUsed = true;
//        }
//    }

//    public override void SecondaryUse() {
//        if (aiming) {
//            controller.cam.fieldOfView = fovZoom;
//            controller.itemCam.fieldOfView = fovZoom;
//        } else {
//            Manager.mainCamera.fieldOfView = Manager.startFov;
//            Manager.fps_controller.itemCamera.fieldOfView = Manager.startFov;
//        }
//    }

//    bool CheckForBullets() {
//        if (scriptableObject.usesLeft > 0) {
//            return true;
//        } else {
//            //ReloadGun();
//            return false;
//        }
//    }

//    //Vector3 BulletSpread() {
//    //    if (secondaryUsed) {
//    //        return new Vector3(Random.Range(-bulletSpreadWhenAiming, bulletSpreadWhenAiming), Random.Range(-bulletSpreadWhenAiming, bulletSpreadWhenAiming), Random.Range(-bulletSpreadWhenAiming, bulletSpreadWhenAiming)) + origin.forward;
//    //    } else {
//    //        return new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread)) + origin.forward;
//    //    }
//    //}

//    //void Fire() {
//    //    if (burst >= bullets.burstType.bulletsPerShot) {
//    //        CancelInvoke();
//    //    }
//    //    if (shot && fireMode == FireMode.Burst) {
//    //        AudioManager.PlaySound(shot, audioGroup);
//    //        Manager.fps_controller.SendOutTrigger(soundRange);
//    //        Manager.fps_controller.triggerRange = soundRange;
//    //    }
//    //    Vector3 spread = BulletSpread();
//    //    if (Physics.Raycast(origin.position, spread, out hit, range)) {
//    //        print(hit.transform.gameObject.name);
//    //        if (hit.transform.GetComponentInChildren<DoSomethingWhenHit>()) {
//    //            print("yes");
//    //            hit.transform.GetComponentInChildren<DoSomethingWhenHit>().GetShot(hit.point, hit.normal, origin.transform.forward, effectivenessPerUse, hitForce);
//    //        } else if (bullets.defaultBulletHole) {
//    //            GameObject hole = Instantiate(bullets.defaultBulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
//    //            hole.transform.SetParent(hit.transform);
//    //        }
//    //        //print(hit.transform.gameObject.name);
//    //    }
//    //    if (bulletParticle) {
//    //        GameObject bullet = Instantiate(bulletParticle, origin.position, origin.rotation);
//    //        ParticleSystem part = bullet.GetComponent<ParticleSystem>();
//    //        Destroy(bullet, part.main.duration);
//    //    }
//    //    burst++;
//    //}

//    public void ReloadGun() {
//        Inventory inventory = GetComponentInParent<Inventory>();
//        for (int i = 0; i < inventory.storedItems.Count; i++) {
//            int bulletsNeeded = maxUses - scriptableObject.usesLeft;
//            if (bulletsNeeded <= 0) {
//                break;
//            }
//            if (inventory.storedItems[i] && inventory.storedItems[i].name == bullets.scriptableBullet.name) {
//                if (bulletsNeeded <= inventory.storedItems[i].currentInStack) {
//                    inventory.storedItems[i].currentInStack -= bulletsNeeded;
//                    inventory.SetPictureAndValueInSpecificUIStorageSlot(inventory.storedItems[i], i);
//                    scriptableObject.usesLeft += bulletsNeeded;
//                } else {
//                    scriptableObject.usesLeft += inventory.storedItems[i].currentInStack;
//                    inventory.storedItems[i].currentInStack -= inventory.storedItems[i].currentInStack;
//                    inventory.SetPictureAndValueInSpecificUIStorageSlot(inventory.storedItems[i], i);
//                }
//            }
//        }
//        Manager.UpdateUsesLeftText(true, scriptableObject.usesLeft.ToString());
//    }

//    //private void OnDrawGizmosSelected() {
//    //    if (origin) {
//    //        Debug.DrawRay(origin.position, origin.transform.forward, Color.red * 1000);
//    //    }
//    //}
//}

//[System.Serializable]
//public class AmmoType : ScriptableObject {
//    public GameObject prefab_Ammo;
//    public Burst burstType;
//}

//[System.Serializable]
//public class Burst {
//    [Range(0.01f, 1)]
//    public float burstDelay = 1;
//    public int bulletsPerShot = 1;
}