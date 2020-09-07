using UnityEngine;
using Photon.Pun;

public class Projectile : Interactable {

    public bool isAffectedByGravity;

    [Header("HideInInspector")]
    public bool inAir;
    Rigidbody rigid;
    Vector3 startPoint;
    float range;

    void Init() {
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
        Init();
        inAir = true;
        range = behaviour.range;
        startPoint = transform.position;

        transform.SetParent(null);

        transform.rotation = Quaternion.LookRotation(targetPos);
        rigid.AddForce(transform.forward * projectileSpeed);

        if (isAffectedByGravity) {
            rigid.useGravity = true;
        }
    }

    public virtual void Move() {

    }

    public virtual void OutOfRange() {
        rigid.isKinematic = true;
        inAir = false;
    }

    private void OnTriggerEnter(Collider other) {
        OutOfRange();
    }
}