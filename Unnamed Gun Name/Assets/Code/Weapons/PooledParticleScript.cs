using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledParticleScript : MonoBehaviour, IPoolObject {

    public float duration;
    [HideInInspector] public ParticleSystem[] particleSystems;

    private void Awake() {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void OnObjectSpawn() {
        if (particleSystems.Length > 0) {
            for (int i = 0; i < particleSystems.Length; i++) {
                particleSystems[i].Play();
            }
        }
        StartCoroutine(Disable());
    }


    IEnumerator Disable() {
        yield return new WaitForSeconds(duration);
        if (particleSystems.Length > 0) {
            for (int i = 0; i < particleSystems.Length; i++) {
                particleSystems[i].Stop();
            }
        }
        gameObject.SetActive(false);
    }
}