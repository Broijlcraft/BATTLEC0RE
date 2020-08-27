using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public Interactable_SOBJ scriptableObject;

    public float affectivenessPerUse;
    public int usesPerSecond;

    [Header("HideInInspector")]
    public Collider[] colliders;
    public float currentUseCoolDown;
    public bool beingHeld, usingPrimary, usingSecondary;

    public virtual void PrimaryUse(bool beingHeld) {

    }

    public virtual void SecondaryUse(bool beingHeld) {

    }
}

[CreateAssetMenu]
public class Interactable_SOBJ : ScriptableObject{
    public int index, usesLeft;
}