using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoomCustomMatchMaking : MonoBehaviourPunCallbacks, IInRoomCallbacks {

    public static PhotonRoomCustomMatchMaking roomSingle;

    public GameObject playerPrefab, lobbyGameObject, roomGameObject, playerListingPrefab, startButton, loadingTextObject;
    public Transform playersPanel;
    [HideInInspector] public PhotonView PV;

    public int currentScene;
    [Space]
    public Player[] photonPlayers;

    [Header("HideInInspector")]
    public bool isLoaded;
    public int playersInGame, playersInRoom, myNumberInRoom;

    private void Awake() {
        if (PhotonRoomCustomMatchMaking.roomSingle == null) {
            PhotonRoomCustomMatchMaking.roomSingle = this;
        } else {
            if(PhotonRoomCustomMatchMaking.roomSingle != this) {
                Destroy(PhotonRoomCustomMatchMaking.roomSingle.gameObject);
                PhotonRoomCustomMatchMaking.roomSingle = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start() {
        PV = GetComponent<PhotonView>();
    }

    public override void OnJoinedRoom() {
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        ClearPlayerListings();
        myNumberInRoom = playersInRoom;
        TeamsTest();
        ListPlayers();
        if (lobbyGameObject) {
            lobbyGameObject.SetActive(false);
        }
        if (roomGameObject) {
            roomGameObject.SetActive(true);
        }
        if (startButton) {
            if (PhotonNetwork.IsMasterClient) {
                startButton.SetActive(true);
            } else {
                startButton.SetActive(false);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if (lobbyGameObject) {
            lobbyGameObject.SetActive(false);
        }
        if (roomGameObject) {
            roomGameObject.SetActive(true);
        }
        if (startButton) {
            if (PhotonNetwork.IsMasterClient) {
                startButton.SetActive(true);
            } else {
                startButton.SetActive(false);
            }
        }

        ClearPlayerListings();
        TeamsTest();
        ListPlayers();

        playersInRoom++;
    }

    void TeamsTest() {
        if (PhotonNetwork.InRoom) {
            photonPlayers = PhotonNetwork.PlayerList;
            TeamManager.single_TM.teams[0].playerNames.Clear();
            TeamManager.single_TM.teams[1].playerNames.Clear();
            for (int i = 0; i < photonPlayers.Length; i++) {
                int index;
                if (i % 2 == 0) {
                    index = 0;
                } else {
                    index = 1;
                }
                TeamManager.single_TM.teams[index].playerNames.Add(photonPlayers[i].NickName);
            }
        }
    }

    void ClearPlayerListings() {
        if (playersPanel) {
            for (int i = playersPanel.childCount - 1; i >= 0; i--) {
                Destroy(playersPanel.GetChild(i).gameObject);
            }
        }
    }

    void ListPlayers() {
        if (PhotonNetwork.InRoom) {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
                GameObject tempNickNameObject = Instantiate(playerListingPrefab, playersPanel);
                ScriptPlayerListing spl = tempNickNameObject.GetComponent<ScriptPlayerListing>();
                string nickname = PhotonNetwork.PlayerList[i].NickName;
                string cleanedNickname = Tools.RemoveIdFromNickname(nickname);
                spl.text_Nickname.text = cleanedNickname;
                int index = TeamManager.single_TM.GetTeamIndex(nickname);
                if(index >= 0) {
                    spl.img.color = TeamManager.single_TM.teams[index].teamColor;
                }
            }
        }
    }

    public void StartGame() {
        isLoaded = true;
        PhotonNetwork.LoadLevel(currentScene + 1);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.single_MPS.multiplayerScene) {
            isLoaded = true;
            Manager.single_M.SceneFinishedLoading();
            PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        }
    }

    public void EnableRoomLoadingUI() {
        PV.RPC("RPC_EnableRoomLoadingUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " left the game");
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
        TeamsTest();
    }

    [PunRPC]
    void RPC_EnableRoomLoadingUI() {
        loadingTextObject.SetActive(true);
    }

    [PunRPC]
    void RPC_LoadedGameScene() {
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length) {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_CreatePlayer() {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
