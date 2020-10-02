using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamListingScript : MonoBehaviourPun {

    public Transform filledListingsHolder;
    public Image[] emptyImages;
    public Button joinButton;
    Team team;
    MyPlayer lp;
    TeamManager tm;

    public void Init(int teamIndex) {
        tm = TeamManager.single_TM;
        team = TeamManager.single_TM.teams[teamIndex];
        for (int i = 0; i < emptyImages.Length; i++) {
            emptyImages[i].color = team.emptyColor;
        }
        joinButton.image.color = team.filledColor;
        joinButton.onClick.AddListener(JoinTeam);
    }

    void JoinTeam() {
        if(team.players.Count < team.playersAllowed.max) {
            lp = PlayersManager.single_PM.myLocalPlayer;
            //photonView.RPC("RPC_JoinTeam", RpcTarget.MasterClient, lp.);

            if (!lp.spl) {
                GameObject playerListingObj = Instantiate(PlayersManager.single_PM.playerListingPrefab, filledListingsHolder);
                lp.spl = playerListingObj.GetComponent<ScriptPlayerListing>();
            }
            if (lp.inTeam) {
                LeaveTeam(lp);
            }
            lp.teamIndex = team.teamId;
            lp.inTeam = true;
            lp.spl.img.color = team.filledColor;
            lp.spl.transform.SetParent(team.tls.filledListingsHolder);
            team.players.Add(lp);
        }
    }

    void RPC_JoinTeam(string photonNick, int teamID) {
        Team team = TeamManager.single_TM.teams[teamID];
        MyPlayer mp = PlayersManager.single_PM.players[photonNick];
        if (!mp.spl) {
            Transform filledListingsHolder = team.tls.filledListingsHolder;
            GameObject playerListingObj = GameObject.Instantiate(PlayersManager.single_PM.playerListingPrefab, filledListingsHolder);
            mp.spl = playerListingObj.GetComponent<ScriptPlayerListing>();
        }
        if (mp.inTeam) {
            LeaveTeam(mp);
        }
        mp.teamIndex = team.teamId;
        mp.inTeam = true;
        mp.spl.img.color = team.filledColor;
        mp.spl.transform.SetParent(team.tls.filledListingsHolder);
        team.players.Add(mp);
    }

    void LeaveTeam(MyPlayer mp) {
        TeamManager.single_TM.teams[mp.teamIndex].players.Remove(mp);
    }
}