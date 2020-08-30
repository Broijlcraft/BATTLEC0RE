using UnityEngine;

public class Interactable : MonoBehaviour{

    [HideInInspector] public int index;
    [HideInInspector] public Controller interactingController;

    public virtual void Interact(Controller controller) {
        if (!interactingController) {
            InteractableActions.ia_Single.CheckAndSetInteracting(index, controller.photonView.ViewID);
        }
    }

    public virtual void Use() {

    }
}
