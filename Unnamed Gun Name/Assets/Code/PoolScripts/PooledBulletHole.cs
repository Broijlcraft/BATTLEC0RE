using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledBulletHole : MonoBehaviour, IPoolObject {

    public Vector3 offSet;
    public float duration;
    float timer = 0;

    public void OnObjectSpawn() {
        transform.localPosition += offSet;
        timer = 0;
    }

    private void Update() {
        if(timer < duration) {
            timer += Time.deltaTime;
        } else {
            gameObject.SetActive(false);
        }
    }
}