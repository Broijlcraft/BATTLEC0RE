using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TeamManager : MonoBehaviourPun {
    public static TeamManager single_TM;

    [Tooltip("this will later be set by what gamemode is picked")]
    public int maxTeams;

    public Transform teamListingsHolder;
    public GameObject teamListingPrefab;

    [Header("HideInInspector")]
    public List<Team> teams = new List<Team>();
    
    /* to be added:
     * teams being created from code not editor
     */

    private void Awake() {
        single_TM = this;
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < maxTeams; i++) {
            //create new team from script
            GameObject tlsObj = Instantiate(teamListingPrefab, teamListingsHolder);
            TeamListingScript tls = tlsObj.GetComponent<TeamListingScript>();
            tls.Init(i);
            Team team = teams[i];
            team.teamId = i;
            team.tls = tls;
        }
    }
}

[System.Serializable]
public class Team {
    public RangeI playersAllowed;
    public Color emptyColor, filledColor;
    public int teamId;
    [Space]
    public List<MyPlayer> players = new List<MyPlayer>();

    [HideInInspector] public TeamListingScript tls;
}