using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableInScene : MonoBehaviour {
    public Interactable_SOBJ scriptableObject;

    [Header("HideInInspector")]
    public int index;
    public Collider[] colliders;
}

[CreateAssetMenu]
public class Interactable_SOBJ : ScriptableObject{
    public int index;
}