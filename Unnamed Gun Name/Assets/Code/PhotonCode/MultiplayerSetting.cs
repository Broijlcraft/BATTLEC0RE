using UnityEngine;

public class MultiplayerSetting : MonoBehaviour {

    public static MultiplayerSetting single_MPS;

    public int maxPlayers, menuScene, multiplayerScene;

    private void Awake() {
        if (MultiplayerSetting.single_MPS) {
            if(MultiplayerSetting.single_MPS != this) {
                Destroy(this.gameObject);
            }
        } else {
            MultiplayerSetting.single_MPS = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
