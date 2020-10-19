using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use only for bodyparts not weapons.
public class CustomRobotPart : MonoBehaviour {
    public BodypartType bodypartType;
    public MeshFilter meshFilter { get { return GetComponent<MeshFilter>(); } }
    public MeshRenderer meshRenderer { get { return GetComponent<MeshRenderer>(); } }
}