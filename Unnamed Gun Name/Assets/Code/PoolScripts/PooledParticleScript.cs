using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledParticleScript : MonoBehaviour, IPoolObject {

    public float duration;
    [HideInInspector] public ParticleSystem[] particleSystems;
    float timer;

    private void Awake() {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void OnObjectSpawn() {
        if (particleSystems.Length > 0) {
            for (int i = 0; i < particleSystems.Length; i++) {
                particleSystems[i].Play();
            }
        }
        timer = 0;
    }
    
    private void Update() {
        if (timer < duration) {
            timer += Time.deltaTime;
        } else {
            if (particleSystems.Length > 0) {
                for (int i = 0; i < particleSystems.Length; i++) {
                    particleSystems[i].Stop();
                }
            }
            gameObject.SetActive(false);
        }
    }
}