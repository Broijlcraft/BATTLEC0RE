using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody), typeof(Health))]
[RequireComponent(typeof(PlayerView), typeof(PhotonView), typeof(WeaponController))] 
public class Controller : MonoBehaviourPun {

    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform verticalCamHolder;
    public Text nicknameText;
    public Transform uiLookAtHolder;
    public float interactRange;

    [Space]
    public SpeedSettings forwardsSpeedSettings;
    public SpeedSettings sidewaysSpeedSettings;

    [Space]
    public CameraSettings cameraSettings;
    
    [Header("Local Settings")]
    public MeshRenderer[] meshRenderersToDisableLocally;
    public bool hideCursorOnStart, keepLocalMeshesEnabled;
    
    [HideInInspector] public bool canMove;
    [HideInInspector] public Health health;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform nicknameTarget;
    [HideInInspector] public WeaponController weaponsController;

    Camera[] cams;
    AudioListener audioListeners;
    float currentForwardSprintValue, currentSidewaysSprintValue, xRotationAxisAngle;

    private void Awake() {
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        cams = GetComponentsInChildren<Camera>();
        weaponsController = GetComponent<WeaponController>();
        weaponsController.controller = this;
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;
        if (photonView.IsMine || playerView.devView) {
            uiLookAtHolder.gameObject.SetActive(false);
            if (meshRenderersToDisableLocally.Length > 0 && !keepLocalMeshesEnabled) {
                for (int i = 0; i < meshRenderersToDisableLocally.Length; i++) {
                    meshRenderersToDisableLocally[i].enabled = false;
                }
            }
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        }
    }

    private void Start() {
        if (photonView.IsMine && Spawnpoints.sp_Single && PhotonRoomCustomMatchMaking.roomSingle) {
            if(Spawnpoints.sp_Single.spawnpoints.Length > 0) {
                Spawnpoints.sp_Single.SetSpPositionAndRotation(transform, PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom - 1);
            }
        }
        TurnCollidersOnOff(true);
        Init();
    }

    void Init() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (photonView.IsMine || playerView.devView) {
            for (int i = 0; i < cams.Length; i++) {
                cams[i].enabled = true;
            }
            audioListeners.enabled = true;
            gameObject.layer = TagsAndLayersManager.single_TLM.localPlayerLayerInfo.index;
        }
        canMove = true;
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Debug.LogWarning("(bool)Can Move WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
    }

    private void Update() {
        if ((photonView.IsMine || playerView.devView) && Input.GetButtonDown("Interact") /*&& !health.isDead*/) {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactRange, ~TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask)) {
                if (hit.transform.CompareTag(TagsAndLayersManager.single_TLM.interactableTag)) {
                    hit.transform.GetComponent<Interactable>().Interact(this);
                }
            }
        }
    }

    private void FixedUpdate() {
        if (((canMove && photonView.IsMine) || playerView.devView) && !health.isDead) { 
            Rotate();

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * currentForwardSprintValue * forwardsSpeedSettings.defaultSpeed;
            float horizontal = Input.GetAxis("Horizontal") * currentSidewaysSprintValue * sidewaysSpeedSettings.defaultSpeed;            

            Vector3 newPos = new Vector3(horizontal, 0, vertical) * Time.deltaTime;
            transform.Translate(newPos);
        }

        if (nicknameTarget) {
            uiLookAtHolder.LookAt(nicknameTarget);
        }
    }
    
    void SprintCheck() {
        if (Input.GetButton("Sprint")) {
            currentForwardSprintValue = forwardsSpeedSettings.sprintMultiplier;
            currentSidewaysSprintValue = sidewaysSpeedSettings.sprintMultiplier;
        } else {
            currentForwardSprintValue = 1;
            currentSidewaysSprintValue = 1;
        }
    }

    void Rotate() {
        float mouseX = Input.GetAxis("Mouse X") * cameraSettings.mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSettings.mouseSensitivity;

        xRotationAxisAngle += mouseY;

        if (xRotationAxisAngle > cameraSettings.maxVerticalTopViewAngle) {
            xRotationAxisAngle = cameraSettings.maxVerticalTopViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder.transform, -cameraSettings.maxVerticalTopViewAngle);
        } else if (xRotationAxisAngle < -cameraSettings.maxVerticalBottomViewAngle) {
            xRotationAxisAngle = -cameraSettings.maxVerticalBottomViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder.transform, cameraSettings.maxVerticalBottomViewAngle);
        }
        verticalCamHolder.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    void TurnCollidersOnOff(bool state) {
        colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].enabled = state;
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
                controllers[i].nicknameTarget = cam.transform;
                controllers[i].nicknameText.text = PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(controllers[i].photonView.Owner.NickName);
            }
        }
    }
}

[System.Serializable]
public class SpeedSettings {

    public float defaultSpeed, sprintMultiplier;
}

[System.Serializable]
public class CameraSettings {

    public float mouseSensitivity = 1f;
    //[Range(-90, 180)]
    public float maxVerticalTopViewAngle = 90, maxVerticalBottomViewAngle = 90;
}