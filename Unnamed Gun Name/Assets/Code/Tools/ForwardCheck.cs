using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardCheck : MonoBehaviour {

    private void OnDrawGizmosSelected() {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}