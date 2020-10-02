using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public MeshCollider[] colls;
    
    private void Reset() {
        colls = FindObjectsOfType<MeshCollider>();
    }
}
