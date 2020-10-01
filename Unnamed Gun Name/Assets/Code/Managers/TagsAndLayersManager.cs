using UnityEngine;

public class TagsAndLayersManager : MonoBehaviour {

    public static TagsAndLayersManager single_TLM;

    public string interactableTag = "Interactable";
    public LayerInfo playerLayerInfo, localPlayerLayerInfo, interactableLayer, noPlayerCollision, cantBeSeenByPlayer;

    public void Awake() {
        if (!TagsAndLayersManager.single_TLM) {
            TagsAndLayersManager.single_TLM = this;
        }
    }
}

[System.Serializable]
public class LayerInfo {
    public LayerMask layerMask;
    public int index;
}