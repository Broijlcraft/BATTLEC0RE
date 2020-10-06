using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCounter : MonoBehaviour {

    public int amountOfChildren;

    private void Reset() {
        amountOfChildren = transform.childCount;
    }
}