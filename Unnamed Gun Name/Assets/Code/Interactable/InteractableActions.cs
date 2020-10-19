using Photon.Pun;

public class InteractableActions : MonoBehaviourPun {

    public static InteractableActions single_IA;

    private void Awake() {
        if (!InteractableActions.single_IA) {
            InteractableActions.single_IA = this;
        }
    }

    //rpc functions

    public void DestroyIa(int index, float time, RpcTarget selectedTarget) {
        photonView.RPC("RPC_DestroyIa", selectedTarget, index, time);
    }

    [PunRPC]
    void RPC_DestroyIa(int index, float time) {
        try {
            Destroy(InteractablesList.single_IaList.interactables[index].gameObject, time);
        } catch { }
    }

    public void CheckAndSetInteracting(int index, int id) {
        photonView.RPC("RPC_CheckAndSetInteracting", RpcTarget.MasterClient, index, id);
    }

    [PunRPC]
    void RPC_CheckAndSetInteracting(int index, int id) {
        if (!InteractablesList.single_IaList.interactables[index].interactingController) {
            photonView.RPC("RPC_SetInteracting", RpcTarget.All, index, id);
        }
    }

    [PunRPC]
    void RPC_SetInteracting(int index, int id) {
        Controller controller = PhotonNetwork.GetPhotonView(id).GetComponent<Controller>();
        Interactable ia = InteractablesList.single_IaList.interactables[index];
        if(!ia.interactingController || ia.interactingController == controller) {
            ia.interactingController = controller;
            ia.Interact(controller);
        }
    }
    
    public void SwitchWeaponBehaviour(int id, int behaviour) {
        photonView.RPC("RPC_SwitchWeaponBehaviour", RpcTarget.All, id, behaviour);
    }

    [PunRPC]
    void RPC_SwitchWeaponBehaviour(int id, int behaviourIndex) {
        WeaponController weaponController = PhotonNetwork.GetPhotonView(id).GetComponent<WeaponController>();
        StartCoroutine(weaponController.SwitchWeaponBehaviour(behaviourIndex));
    }

    public void PlayFireArmsEffect(int index, int behaviourIndex, int aoIndex, string triggerName) {
        //photonView.RPC("RPC_PlayFireArmEffects", RpcTarget.All, index, behaviourIndex, aoIndex, triggerName);
    }

    [PunRPC]
    void RPC_PlayFireArmEffects(int index, int behaviourIndex, int aoIndex, string triggerName) {
        FireArms ia = InteractablesList.single_IaList.interactables[index] as FireArms;
        WeaponBehaviour behaviour = ia.weaponBehaviours[behaviourIndex];
        AttackOrigin origin = behaviour.attackOrigins[aoIndex];
        origin.animator.speed = behaviour.attacksPerSecond;
        origin.animator.SetTrigger(triggerName);
    }
}