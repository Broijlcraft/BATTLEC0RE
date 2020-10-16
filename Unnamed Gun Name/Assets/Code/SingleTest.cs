using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTest : MonoBehaviour {
    public static SingleTest single_ss;
    public SingleTest ss;
    public bool single;

    private void Awake() {
        if (single) {
            single_ss = this;
        }
    }

    private void Start() {
        ss = single_ss;
    }
}