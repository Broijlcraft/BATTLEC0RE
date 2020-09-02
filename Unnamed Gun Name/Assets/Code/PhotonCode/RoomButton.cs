using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomButton : MonoBehaviour {

    public Text nameText, sizeText;
    public Button button;
    [Header("HideInInspector")]
    public string roomName;
    public int roomSize;

    public void SetRoom(RoomInfo room) {
        roomName = room.Name;
        nameText.text = roomName;
        sizeText.text = room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString();
        button.onClick.AddListener(JoinRoomOnClick);
    }

    public void JoinRoomOnClick() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRoom(roomName);
        } else {
            Debug.LogWarning("Please wait, you are still being connected to the PhotonNetwork");
        }
    }
}
