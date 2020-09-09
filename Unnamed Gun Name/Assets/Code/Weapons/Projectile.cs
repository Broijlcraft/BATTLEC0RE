using UnityEngine;
using Photon.Pun;

public class Projectile : Interactable {

    public bool isAffectedByGravity;

    [Header("HideInInspector")]
    public bool inAir;
    Rigidbody rigid;
    Vector3 startPoint;
    float range;

    private void Awake() {
        if (!rigid) {
            rigid = GetComponent<Rigidbody>();
        }
    }

    private void Update() {
        if (range < Vector3.Distance(startPoint, transform.position) && inAir) {
            OutOfRange();
        }
    }

    public virtual void Launch(Vector3 targetPos, WeaponBehaviour behaviour, float projectileSpeed) {
        startPoint = transform.position;
        range = behaviour.range;

        transform.rotation = Quaternion.LookRotation(targetPos);
        rigid.AddForce(transform.forward * projectileSpeed);

        inAir = true;

        if (isAffectedByGravity) {
            rigid.useGravity = true;
        }
    }

    public virtual void Move() {

    }

    public virtual void OutOfRange() {
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        inAir = false;
    }

    private void OnTriggerEnter(Collider other) {
        OutOfRange();
    }
}