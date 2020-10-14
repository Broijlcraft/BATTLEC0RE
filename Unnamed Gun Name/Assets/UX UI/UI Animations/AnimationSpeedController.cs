using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour {
    Animator animator {
        get {
            return GetComponent<Animator>();
        }
    }
    public float speed;
    float lastValue;

    private void Start() {
        lastValue = 0;
    }

    private void LateUpdate() {
        if (animator) {
            if (MinSpeedCheck() && lastValue != speed) {
                animator.speed = speed;
                lastValue = speed;
            }
        }
    }

    bool MinSpeedCheck() {
        if (speed < 0.0001f || float.IsNaN(speed)) {
            speed = 0.0001f;
        }
        return true;
    }
}