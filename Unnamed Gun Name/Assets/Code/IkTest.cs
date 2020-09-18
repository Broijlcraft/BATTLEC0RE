using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkTest : MonoBehaviour {
    public bool ik;

    public float sense, swayIntensity, swaySmooth;

    public Transform[] bones, horizontalSwayHolders;
    public Transform target, verticalSwayHolder;
    public Vector3 defaultHorizontalSwayRotation, defaultVerticalSwayRotation;

    public float topAngle, bottomAngle;
    //private void Awake() {
    //    anim = GetComponentInChildren<Animator>();
    //}

    //private void Update() {
    //    float fw = Input.GetAxis("Vertical");
    //    if (fw != 0) { 
    //        anim.SetBool("Walk", true);
    //    } else {
    //        anim.SetBool("Walk", false);
    //    }
    //    transform.Translate(transform.forward*fw*Time.deltaTime*speed);
    //}

    //private void OnAnimatorIK(int layerIndex) {
    //    print("anim");
    //    if (anim) {
    //        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
    //        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);


    //        RaycastHit hit;
    //        print(anim.GetIKPosition(AvatarIKGoal.LeftFoot));
    //        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

    //        if (Physics.Raycast(ray, out hit, distanceToGround + 1f, ignoreLayers)) {
    //            print(hit.transform.name);
    //            if (hit.transform.tag == "Walkable") {
    //                Vector3 footPosition = hit.point;
    //                footPosition.y += distanceToGround;
    //                anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
    //                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
    //            }
    //        }
    //        Debug.DrawRay(ray.origin, ray.direction);
    //        print("ues");
    //    }
    //}

    private void LateUpdate() {
        //if (ik) {
        //    for (int i = 0; i < bones.Length; i++) {
        //        Vector3 lastRot = bones[i].localRotation.eulerAngles;
        //        lastRot.x = 0;
        //        lastRot.z = 0;
        //        bones[i].LookAt(target);
        //        lastRot = bones[i].localRotation.eulerAngles;
        //        //lastRot.y = bones[i].localRotation.eulerAngles.y;
        //        bones[i].localRotation = Quaternion.Euler(lastRot);
        //    }
        //}
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate() {
        Rotate();
    }

    float xRotationAxisAngle;
    void Rotate() {
        float mouseX = Input.GetAxis("Mouse X") * sense;
        float mouseY = Input.GetAxis("Mouse Y") * sense;

        xRotationAxisAngle += mouseY;

        ApplySway(mouseX, mouseY);

        //if (xRotationAxisAngle > topAngle) {
        //    xRotationAxisAngle = topAngle;
        //    mouseY = 0f;
        //    ClampXRotationAxisToValue(verticalChestRotationHolder.transform, -cameraSettings.maxVerticalTopViewAngle);
        //} else if (xRotationAxisAngle < -cameraSettings.maxVerticalBottomViewAngle) {
        //    xRotationAxisAngle = -cameraSettings.maxVerticalBottomViewAngle;
        //    mouseY = 0f;
        //    ClampXRotationAxisToValue(verticalChestRotationHolder.transform, cameraSettings.maxVerticalBottomViewAngle);
        //}
        //verticalChestRotationHolder.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ApplySway(float mouseX, float mouseY) {
        Quaternion rotX = Quaternion.AngleAxis(-swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(defaultHorizontalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;

        Quaternion verticalTemp = Quaternion.Euler(defaultVerticalSwayRotation);
        Quaternion verticalTargetRotation = verticalTemp * rotY;

        for (int i = 0; i < horizontalSwayHolders.Length; i++) {
            horizontalSwayHolders[i].localRotation = Quaternion.Lerp(horizontalSwayHolders[i].transform.localRotation, horizontalTargetRotation, Time.deltaTime * swaySmooth);
        }
        //verticalSwayHolder.localRotation = Quaternion.Lerp(verticalSwayHolder.transform.localRotation, verticalTargetRotation, Time.deltaTime * swaySmooth);
    }

    void ClampXRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }
}
