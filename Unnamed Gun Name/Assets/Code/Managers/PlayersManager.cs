using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviourPun {
    public static PlayersManager single_PM;

    public GameObject playerListingPrefab;

    [Header("HideInInspector")]
    public MyPlayer myLocalPlayer;
    public Dictionary<string, MyPlayer> players = new Dictionary<string, MyPlayer>();

    public void Awake() {
        single_PM = this;
    }

    public void Init() {
        Player lp = PhotonNetwork.LocalPlayer;

        if (!players.ContainsKey(lp.NickName)) {
            myLocalPlayer = Tools.CreatePlayer(lp, -1);
            photonView.RPC("RPC_AddPlayer", RpcTarget.All, lp.NickName);
        }
    }

    [PunRPC]
    void RPC_AddPlayer(string photonNick) {
        Player[] pPlayers = PhotonNetwork.PlayerList;
        MyPlayer newPlayer = new MyPlayer();
        for (int i = 0; i < pPlayers.Length; i++) {
            if(pPlayers[i].NickName == photonNick) {
            }
        }
        if (newPlayer.player != null) {
            players.Add(photonNick, newPlayer);
        }
        Test();
    }

    void Test() {
        foreach (KeyValuePair<string, MyPlayer> entry in players) {
            Debug.Log(entry.Key);
        }
    }
}