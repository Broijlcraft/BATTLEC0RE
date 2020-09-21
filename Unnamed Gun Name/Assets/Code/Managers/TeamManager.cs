using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {
    public static TeamManager single_TM;
    public List<Team> teams = new List<Team>();

    private void Awake() {
        single_TM = this;
    }

    public int GetTeamIndex(string nickname) {
        int index = -1;
        for (int i = 0; i < teams.Count; i++) {
            Team team = teams[i];
            for (int iB = 0; iB < team.playerNames.Count; iB++) {
                if (team.playerNames[iB].Contains(nickname)) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }
}

[System.Serializable]
public class Team {
    public string teamName;
    public Color teamColor;
    public int minPlayers, maxPlayers;

    [Space]
    public List<string> playerNames = new List<string>();
}