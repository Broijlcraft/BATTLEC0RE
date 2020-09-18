using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GamemodeManager : MonoBehaviour {
    public static GamemodeManager single_GM;

    public GamemodeType type;

    public int forTVT, forFFA;

    private void Awake() {
        single_GM = this;
    }
}

[System.Serializable]
public class Gamemode {
    public GamemodeType type;
}

public enum GamemodeType {
    TeamVsTeam,
    FreeForAll
}