using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisDoorScript : MonoBehaviour {
    public Animator anim;

    private void OnTriggerEnter(Collider other) {
        anim.SetTrigger("Open");
        anim.ResetTrigger("Close");
    }

    private void OnTriggerExit(Collider other) {
        anim.SetTrigger("Close");
        anim.ResetTrigger("Open");        
    }
}