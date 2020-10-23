using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AssemblyArm : MonoBehaviour {
    public Transform target, defaultTarget;
    public AssemblyArmPart[] assemblyArmParts;

    //public List<Transform> carts

    private void Start() {
        for (int i = 0; i < assemblyArmParts.Length; i++) {
            assemblyArmParts[i].arm = this;
        }
    }

    private void FixedUpdate() {
        for (int i = 0; i < assemblyArmParts.Length; i++) {
            assemblyArmParts[i].RotatePart();
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < assemblyArmParts.Length; i++) {
            Transform part = assemblyArmParts[i].part;
            if (part.gameObject.activeSelf) {
                Debug.DrawRay(part.position, part.forward * 2f, new Color(1f,0f,1f));
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cart")) {

        }
    }
}

[System.Serializable]
public class AssemblyArmPart {
    public enum Axis {
        x,
        y,
        z
    }
    public Axis axis;
    public float turnSpeed = 3f;
    public bool useLocal;
    public Transform part, defaultTarget;
    [HideInInspector] public AssemblyArm arm;

    public void RotatePart() {
        Transform tp;
        if (arm.target != null) {
            tp = arm.target;
        } else {
            tp = defaultTarget;
        }
        Vector3 dir = tp.position - part.position;
        Quaternion lookRotation = Quaternion.Euler(Vector3.zero);
        if (dir != Vector3.zero) {
            lookRotation = Quaternion.LookRotation(dir);
        }

        Quaternion partRot = part.rotation;

        if (useLocal) {
            partRot = part.localRotation;
        }

        Vector3 newFullRot = Quaternion.Lerp(partRot, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        Quaternion newPartRotation = Quaternion.identity;
        switch (axis) {
            case Axis.x:
            newPartRotation = Quaternion.Euler(newFullRot.x, 0f, 0f);
            break;
            case Axis.y:
            newPartRotation = Quaternion.Euler(0f, newFullRot.y, 0f);
            break;
            case Axis.z:
            newPartRotation = Quaternion.Euler(0f, 0f, newFullRot.z);
            break;
        }
        if (useLocal) {
            part.localRotation = newPartRotation;
        } else {
            part.rotation = newPartRotation;
        }
    }
}