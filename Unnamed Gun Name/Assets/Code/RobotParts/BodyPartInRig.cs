using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartInRig : MonoBehaviour {
    public BodypartType bodypartType;

    [HideInInspector] public int index;
    [HideInInspector] public CustomRobotPart robotPartAttached;
}