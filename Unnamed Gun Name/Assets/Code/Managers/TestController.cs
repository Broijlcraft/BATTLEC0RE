using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {
    public static TestController tc_Single;

    public bool dev, testing;
    public Animator anim; 
    private void Awake() {
        tc_Single = this;
    }
}
