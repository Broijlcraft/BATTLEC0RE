using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkTest : MonoBehaviour {
    Animator anim;

    public float speed;
    [Range(0, 1)]
    public float distanceToGround;

    public LayerMask ignoreLayers;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update() {
        float fw = Input.GetAxis("Vertical");
        if (fw != 0) { 
            anim.SetBool("Walk", true);
        } else {
            anim.SetBool("Walk", false);
        }
        transform.Translate(transform.forward*fw*Time.deltaTime*speed);
    }

    private void OnAnimatorIK(int layerIndex) {
        print("anim");
        if (anim) {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);


            RaycastHit hit;
            print(anim.GetIKPosition(AvatarIKGoal.LeftFoot));
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, ignoreLayers)) {
                print(hit.transform.name);
                if (hit.transform.tag == "Walkable") {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                }
            }
            Debug.DrawRay(ray.origin, ray.direction);
            print("ues");
        }
    }
}
