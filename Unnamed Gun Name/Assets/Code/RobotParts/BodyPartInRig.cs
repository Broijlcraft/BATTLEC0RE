using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use only for bodyparts not weapons.
public class BodyPartInRig : MonoBehaviour {
    public BodypartType bodypartType;
    public SkinnedMeshRenderer skinnedMeshRenderer { get { return GetComponent<SkinnedMeshRenderer>(); } }
    public UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
}