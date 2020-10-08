using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlacement : MonoBehaviour {
    public Foot[] feet;
    public float distance;

    private void LateUpdate() {
        for (int i = 0; i < feet.Length; i++) {
            RaycastHit hit;
            Transform target = feet[i].target;
            if (Physics.Raycast(target.position, Vector3.down, out hit, distance)) {
                target.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                print(Quaternion.FromToRotation(Vector3.forward, hit.normal).eulerAngles);
            } else {
                target.rotation = Quaternion.Euler(feet[i].defaultRotation);
            }
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < feet.Length; i++) {
            Transform f = feet[i].target;
            Debug.DrawRay(f.position, Vector3.down * distance);
        }
    }
}

[System.Serializable]
public class Foot {
    public Transform target;
    public Vector3 defaultRotation;
}