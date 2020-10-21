using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorObjects : MonoBehaviour {

    public Transform model;
    public float speed, rotationSpeed, maxDistance;

    [HideInInspector] public Transform targetPoint;
    [HideInInspector] public int pointIndex;
    [HideInInspector] public ConveyorSystem conveyor;
    
    public void Init(ConveyorSystem cs) {
        conveyor = cs;
        pointIndex = -1;
        targetPoint = null;
        conveyor.SetTarget(this);
    }

    private void Update() {
        if (targetPoint != null) {
            Vector3 direction = abs(targetPoint.position, transform.position);
            Vector3 directionToGo = targetPoint.position - transform.position;
            transform.Translate(directionToGo.normalized * speed * Time.deltaTime);
            if (direction.z < maxDistance && direction.x < maxDistance) {
                conveyor.SetTarget(this);
            }
            Vector3 lookDir = targetPoint.position - model.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            Vector3 rotationToLook = Quaternion.Lerp(model.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
            model.rotation = Quaternion.Euler(0f, rotationToLook.y, 0f);
        }
    }

    Vector3 abs(Vector3 v, Vector3 vA) {
        Vector3 vB = v - vA;
        float x = Mathf.Abs(vB.x);
        float y = Mathf.Abs(vB.y);
        float z = Mathf.Abs(vB.z);
        return new Vector3(x, y, z);
    }
}