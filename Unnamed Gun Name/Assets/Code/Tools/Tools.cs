using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class Tools {

    public static void EnableDisableGameObjectsFromArray(GameObject[] obs, bool newState) {
        for (int i = 0; i < obs.Length; i++) {
            obs[i].SetActive(newState);
        }
    }

    public static void SetLocalOrGlobalLayers(GameObject[] gameObjects, bool global) {
        int index = TagsAndLayersManager.single_TLM.localPlayerLayerInfo.index;
        SetLayers(gameObjects, index, global);
    }

    static void SetLayers(GameObject[]obs, int index, bool global) {
        if (global) {
            index = 0;
        }

        for (int i = 0; i < obs.Length; i++) {
            GameObject go = obs[i];
            go.layer = index;
        }
    }

    public static int BoolToInt(bool b) {
        int i = 0;
        if (b) {
            i = 1;
        }
        return i;
    }

    public static bool IntToBool(int i) {
        bool b = false;
        if(i == 1) {
            b = true;
        }
        return b;
    }

    public static bool OwnerCheck(PhotonView view) {
        bool hasOwnerAndIsLocal = false;

        if (view && view.IsMine) {
            hasOwnerAndIsLocal = true;
        }

        return hasOwnerAndIsLocal;
    }

    public static string RemoveIdFromNickname(string nickname) {
        char[] characterList = nickname.ToCharArray();
        string finalNickname = "";
        for (int iB = 0; iB < characterList.Length; iB++) {
            if (characterList[iB].ToString() == "#") {
                break;
            }
            finalNickname += characterList[iB];
        }
        return finalNickname;
    }

    public static MyPlayer CreatePlayer(Player play, int _teamIndex) {
        bool isInTeam = false;
        if (_teamIndex >= 0) {
            isInTeam = true;
        }
        MyPlayer mp = new MyPlayer() { player = play, nickname = play.NickName, teamIndex = _teamIndex, inTeam = isInTeam };
        return mp;
    }
}