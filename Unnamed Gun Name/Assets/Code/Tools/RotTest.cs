using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotTest : MonoBehaviour {

    void Update() {
        print(transform.rotation.eulerAngles);
    }
}
