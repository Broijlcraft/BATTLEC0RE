using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class MyPlayer {
    [Header("HideInSpector")]
    public Player player;
    public string nickname;
    public int teamIndex;
    public bool inTeam;
    public ScriptPlayerListing spl;
}