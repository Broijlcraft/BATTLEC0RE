using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour{
    [Header("HideInInspector")]
    public int index;
    public Controller interactingController;

    public virtual void Interact(Controller controller) {
        if (!interactingController) {
            InteractableActions.ia_Single.CheckAndSetInteracting(index, controller.photonView.ViewID);
        }
    }
}
