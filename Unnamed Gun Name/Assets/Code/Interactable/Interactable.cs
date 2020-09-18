using Photon.Pun;
using UnityEngine;

public class Interactable : MonoBehaviour, IPoolObject {

    public GameObject prefab;

    [HideInInspector] public int index;
    [HideInInspector] public Controller interactingController;
    [HideInInspector] public PhotonView ownerPV, myView;

    public virtual void PhotonInit() {
        myView = GetComponent<PhotonView>();
    }

    public virtual void Interact(Controller controller) {
        if (!interactingController) {
            InteractableActions.ia_Single.CheckAndSetInteracting(index, controller.photonView.ViewID);
        }
    }

    public virtual void OnObjectSpawn() { }

    public virtual void Use() { }
}