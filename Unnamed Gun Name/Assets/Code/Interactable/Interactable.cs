using UnityEngine;

public class Interactable : MonoBehaviour, IPoolObject {

    public GameObject prefab;

    [HideInInspector] public int index;
    [HideInInspector] public Controller interactingController;

    public virtual void Interact(Controller controller) {
        if (!interactingController) {
            InteractableActions.ia_Single.CheckAndSetInteracting(index, controller.photonView.ViewID);
        }
    }

    public virtual void OnObjectSpawn() { }

    public virtual void Use() { }
}