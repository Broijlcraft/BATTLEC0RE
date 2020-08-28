using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviourPun {
    public PlayerView playerView;

    public Camera cam, itemCam;
    public float defaultFov;

    public Text text_Nickname;

    public MeshRenderer[] meshRenderersToDisableLocally;
    [Space]
    public GameObject inGameHud;
    public bool hideCursorOnStart, keepLocalMeshesEnabled;
    [Space]
    public float defaultWalkSpeed;
    public float sprintMultiplier = 1.2f, mouseSensitivity = 1f;
    [Range(0, 90)]
    public float maxVerticalTopViewAngle = 30, maxVerticalBottomViewAngle = 30;
    [Space]
    public int localPlayerLayer = 10;

    [Header("Dev")]
    public InteractableObject ia_InHand;

    [Header("Particles")]
    //public VisualFX[] particles;

    [HideInInspector] public bool canMove;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform localPlayerTarget;
    [HideInInspector] public Collider[] colliders;
    Camera[] cams;
    AudioListener audioListeners;
    float currentSprintValue, xRotationAxisAngle;

    private void Awake() {
        TurnCollidersOnOff(false);
        cams = GetComponentsInChildren<Camera>();
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;
        if (photonView.IsMine || playerView.devView) {
            text_Nickname.gameObject.SetActive(false);
            if (meshRenderersToDisableLocally.Length > 0 && !keepLocalMeshesEnabled) {
                for (int i = 0; i < meshRenderersToDisableLocally.Length; i++) {
                    meshRenderersToDisableLocally[i].enabled = false;
                }
            }
        } else {
            if (inGameHud) {
                inGameHud.SetActive(false);
            }
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        }
    }

    private void Start() {
        //if (photonView.IsMine && Spawnpoints.sp_Single && PhotonRoomCustomMatchMaking.roomSingle) {
        //    Vector3[] posAndRot = Spawnpoints.sp_Single.GetSpPositionAndRotation(PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom - 1);
        //    transform.position = posAndRot[0];
        //    transform.rotation = Quaternion.Euler(posAndRot[1]);
        //}
        TurnCollidersOnOff(true);
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Init();
    }

    private void Update() {
        if (ia_InHand && photonView.IsMine) {
            if (Input.GetMouseButton(0)) {
                ia_InHand.PrimaryUse(true);
            }
            if (Input.GetMouseButtonUp(0)) {
                ia_InHand.PrimaryUse(false);
            }
            if (Input.GetMouseButton(1)) {
                ia_InHand.SecondaryUse(true);
            }
            if (Input.GetMouseButtonUp(1)) {
                ia_InHand.SecondaryUse(true);
            }
        }
        if(TestController.tc_Single && TestController.tc_Single.testing) {
            if (!ia_InHand) {
                if (Input.GetButtonDown("Jump")) {
                    ia_InHand = FindObjectOfType<Gun>();
                }
            }
            if (TestController.tc_Single.anim) {
                if (Input.GetMouseButtonDown(1)) {
                    TestController.tc_Single.anim.SetBool("Moving", true); 
                }
            }
        }
    }

    private void FixedUpdate() {
        if ((canMove && photonView.IsMine) || playerView.devView) { 
            Rotate();

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * currentSprintValue * defaultWalkSpeed;
            float horizontal = Input.GetAxis("Horizontal") * currentSprintValue * defaultWalkSpeed;            

            Vector3 newPos = new Vector3(horizontal, 0, vertical);
            transform.Translate(newPos);
        }

        if (localPlayerTarget) {
            text_Nickname.transform.LookAt(localPlayerTarget);
            text_Nickname.transform.Rotate(0, 180, 0);
        }
    }

    public void Init() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (photonView.IsMine || playerView.devView) {
            for (int i = 0; i < cams.Length; i++) {
                cams[i].enabled = true;
            }
            audioListeners.enabled = true;
            gameObject.layer = localPlayerLayer;
        }
        canMove = true;
        Debug.LogWarning("(bool)canMove WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
    }

    void Rotate() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotationAxisAngle += mouseY;

        if (xRotationAxisAngle > maxVerticalTopViewAngle) {
            xRotationAxisAngle = maxVerticalTopViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(cam.transform, -maxVerticalTopViewAngle);
        } else if (xRotationAxisAngle < -maxVerticalBottomViewAngle) {
            xRotationAxisAngle = -maxVerticalBottomViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(cam.transform, maxVerticalBottomViewAngle);
        }

        cam.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    void TurnCollidersOnOff(bool state) {
        colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].enabled = state;
        }
    }

    void SprintCheck() {
        if (Input.GetButton("Shift") /*&& for only sprinting when going forward Input.GetAxis("Vertical") > 0*/) {
            currentSprintValue = sprintMultiplier;
        } else {
            currentSprintValue = 1;
        }
    }

    private void ClampXRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }

    public void ResetAtStartPosition() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.rotation = Quaternion.identity;
        cam.transform.localRotation = Quaternion.identity;
        xRotationAxisAngle = 0;
    }

    [PunRPC]
    void RPC_SetNicknameTargets() {
        if (photonView.IsMine) {
            Controller[] controllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].localPlayerTarget = cam.transform;
                controllers[i].text_Nickname.text = PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(controllers[i].photonView.Owner.NickName);
            }
        }
    }
}