using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour {

    public float speed;
    [Range(-1, 1)]
    public int x, y, z;

    private void FixedUpdate() {
        Vector3 newRot = new Vector3(x, y, z) * speed * Time.deltaTime;
        transform.Rotate(newRot);
    }
}