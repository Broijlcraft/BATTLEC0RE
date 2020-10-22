using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour {

    public float speed;
    [Range(-1, 1)]
    public int x, y, z;

    public Vector3 firstEndRot, finalEndRot;
    [Header("Anim Settings")]
    public bool isPlaying;

    private void FixedUpdate() {
        Vector3 newRot = new Vector3(x, y, z) * speed * Time.deltaTime;
        transform.Rotate(newRot);
    }

    private void OnDisable() {
        if (isPlaying) {
            transform.localRotation = Quaternion.Euler(firstEndRot);
            StartCoroutine(SetRot());
            isPlaying = false;
        }
    }

    IEnumerator SetRot() {
        yield return new WaitForEndOfFrame();
        transform.localRotation = Quaternion.Euler(finalEndRot);
    }
}