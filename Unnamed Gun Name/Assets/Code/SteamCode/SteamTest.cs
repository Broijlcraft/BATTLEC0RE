////////////////////////////////////////////////////////
//Steam has to be running in background for this to work
////////////////////////////////////////////////////////
using UnityEngine;
using Steamworks;

public class SteamTest : MonoBehaviour {

    public static SteamTest single_SS;

    [HideInInspector] public bool nameIsSet;
    [HideInInspector] public string nickname;

    public void Awake() {
        single_SS = this;
        DontDestroyOnLoad(gameObject);
        if (!SteamManager.Initialized) { return; }
        nameIsSet = true;
        nickname = SteamFriends.GetPersonaName();
    }

    //private void Start() {
    //    if (!SteamManager.Initialized) { return; }
    //    string myName = SteamFriends.GetPersonaName();
    //    int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
    //    for (int i = 0; i < friendCount; i++) {
    //        CSteamID friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
    //        string friendName = SteamFriends.GetFriendPersonaName(friend);
    //        print(myName + " is friends with " + friendName);
    //    }
    //}

    public Texture2D GetSmallAvatar(CSteamID user) {
        int FriendAvatar = SteamFriends.GetSmallFriendAvatar(user);
        uint ImageWidth, ImageHeight;
        bool success = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

        if (success && ImageWidth > 0 && ImageHeight > 0) {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];
            Texture2D returnTexture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            success = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
            if (success) {
                returnTexture.LoadRawTextureData(Image);
                returnTexture.Apply();
            }
            return returnTexture;
        } else {
            Debug.LogError("Couldn't get avatar.");
            return new Texture2D(0, 0);
        }
    }
}