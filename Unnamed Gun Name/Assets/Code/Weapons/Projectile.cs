using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    
    [Header("HideInInspector")]
    public float damage, speed, range;
    public Vector3 startPoint;

    private void Update() {
        if(range < Vector3.Distance(startPoint, transform.position)) {
            Explode();
        }
    }

    void Explode() {

    }
}