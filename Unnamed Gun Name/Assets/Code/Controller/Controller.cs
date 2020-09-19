using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody), typeof(Health))]
[RequireComponent(typeof(PlayerView), typeof(PhotonView), typeof(WeaponController))] 
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform horizontalBodyRotationHolder, verticalChestRotationHolder, horizontalCamRotationHolder, verticalCamRotationHolder;
    public Text nicknameText;
    public Transform uiLookAtHolder;
    public Animator animator;
    public float interactRange;

    [Space]
    public SpeedSettings forwardsSpeedSettings;
    public SpeedSettings sidewaysSpeedSettings;

    public float jumpVelocity;

    [Space]
    public CameraSettings cameraSettings;
    public SwaySettings swaySettings;

    [Header("Local Settings")]
    public bool hideCursorOnStart;
    public bool keepLocalMeshesEnabled;
    
    [HideInInspector] public bool canMove;
    [HideInInspector] public Health health;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform nicknameTarget;
    [HideInInspector] public BodyPartsList robotParts;
    [HideInInspector] public WeaponController weaponsController;


    //replace with bodyparts when ready
    [HideInInspector] public MeshRenderer[] gos;
    Camera[] cams;
    AudioListener audioListeners;
    float currentForwardSprintValue, currentSidewaysSprintValue, xRotationAxisAngle;
    bool isGrounded, isSprinting = false;

    Vector3 defaultHorizontalSwayRotation, defaultVertitalSwayRotation, lastPos;

    [Header("Testing")]
    public bool disableCamsOnStart;

    #region Initialization

    private void Awake() {
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
        cams = GetComponentsInChildren<Camera>();
        weaponsController = GetComponent<WeaponController>();
        weaponsController.Init(this);
        health = GetComponent<Health>();
        health.controller = this;
        animator = GetComponent<Animator>();
        robotParts = GetComponent<BodyPartsList>();
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;

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

        Init();
    }

    void Init() {
        gos = GetComponentsInChildren<MeshRenderer>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (IsMineCheck()) {
            if (PhotonRoomCustomMatchMaking.roomSingle) {
                ObjectPool.single_PT.SetPoolOwners(PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom, photonView.ViewID);
            }
            rigid.useGravity = true;
            if (!disableCamsOnStart) {
                for (int i = 0; i < cams.Length; i++) {
                    cams[i].enabled = true;
                }
                audioListeners.enabled = true;
            }
            List<GameObject> meshObjects = new List<GameObject>();
            for (int i = 0; i < gos.Length; i++) {
                meshObjects.Add(gos[i].gameObject);
            }
            Tools.SetLocalOrGlobalLayers(meshObjects.ToArray(), false);
            defaultHorizontalSwayRotation = new Vector3(swaySettings.defaultCamHolderRotation.x, swaySettings.defaultParentRotation.y, 0);
            defaultVertitalSwayRotation = new Vector3(swaySettings.defaultCamHolderRotation.x, swaySettings.defaultParentRotation.y, 0);
        } else {
            rigid.isKinematic = false;
        }
        TurnCollidersOnOff(true);
        canMove = true;
        Debug.LogWarning("(bool)Can Move WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void TurnCollidersOnOff(bool state) {
        colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].enabled = state;
        }
    }

    [PunRPC]
    void TestPrint() {
        print("print");
    }

    [PunRPC]
    void RPC_SetNicknameTargets() {
        if (photonView.IsMine) {
            Controller[] controllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].nicknameTarget = cam.transform;
                controllers[i].nicknameText.text = Tools.RemoveIdFromNickname(controllers[i].photonView.Owner.NickName);
                int index = TeamManager.single_TM.GetTeamIndex(controllers[i].photonView.Owner.NickName);
                if(index >= 0) {
                    controllers[i].nicknameText.color = TeamManager.single_TM.teams[index].teamColor;
                }
            }
        }
    }
    #endregion

    private void Update() {
        if (IsMineAndAliveCheck()) {

            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactRange, ~TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask)) {
                    if (hit.transform.CompareTag(TagsAndLayersManager.single_TLM.interactableTag)) {
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }

            if (Input.GetButtonDown("Jump")) {
                if (isGrounded) {
                    rigid.velocity = Vector3.up * jumpVelocity;
                    isGrounded = false;
                }
            }
        }
    }

    private void FixedUpdate() {
        if (IsMineCheck() && canMove && !health.isDead) {
            Rotate();

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * currentForwardSprintValue * forwardsSpeedSettings.defaultSpeed;
            float horizontal = Input.GetAxis("Horizontal") * currentSidewaysSprintValue * sidewaysSpeedSettings.defaultSpeed;

            if (animator) {
                if(vertical != 0 || horizontal != 0) {
                    animator.SetBool(robotParts.walk, !isSprinting);
                    animator.SetBool(robotParts.sprint, isSprinting);
                }
            }

            Vector3 newPos = new Vector3(horizontal, 0, vertical) * Time.deltaTime;
            transform.Translate(newPos);
        }        

        if (animator) {
            float speed = Vector3.Distance(transform.position, lastPos) * 50;
            lastPos = transform.position;
            speed /= forwardsSpeedSettings.defaultSpeed * currentForwardSprintValue;

            animator.SetFloat("MoveSpeed", speed);
        }

        if (nicknameTarget) {
            uiLookAtHolder.LookAt(nicknameTarget);
        }
    }
    
    void SprintCheck() {
        isSprinting = Input.GetButton("Sprint");

        if (isSprinting) {
            currentForwardSprintValue = forwardsSpeedSettings.sprintMultiplier;
            currentSidewaysSprintValue = sidewaysSpeedSettings.sprintMultiplier;
        } else {
            currentForwardSprintValue = 1;
            currentSidewaysSprintValue = 1;
        }
    }

    #region Rotation

    void Rotate() {
        float sense = cameraSettings.mouseSensitivity, invert = GetInvertMultiplier(cameraSettings.invertVerticalCam);
        float mouseX = Input.GetAxis("Mouse X") * sense * invert;
        float mouseY = Input.GetAxis("Mouse Y") * sense * invert;

        xRotationAxisAngle += mouseY;

        ApplySway(mouseX, mouseY);

        if (xRotationAxisAngle > cameraSettings.maxVerticalTopViewAngle) {
            xRotationAxisAngle = cameraSettings.maxVerticalTopViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalChestRotationHolder.transform, -cameraSettings.maxVerticalTopViewAngle);
        } else if (xRotationAxisAngle < -cameraSettings.maxVerticalBottomViewAngle) {
            xRotationAxisAngle = -cameraSettings.maxVerticalBottomViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalChestRotationHolder.transform, cameraSettings.maxVerticalBottomViewAngle);
        }
        verticalChestRotationHolder.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ApplySway(float mouseX, float mouseY) {
        Quaternion rotX = Quaternion.AngleAxis(-swaySettings.swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swaySettings.swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(defaultHorizontalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;

        Quaternion verticalTemp = Quaternion.Euler(defaultVertitalSwayRotation);
        Quaternion verticalTargetRotation = verticalTemp * rotY;

        swaySettings.horizontalSwayHolder.localRotation = Quaternion.Lerp(swaySettings.horizontalSwayHolder.transform.localRotation, horizontalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
        swaySettings.verticalSwayHolder.localRotation = Quaternion.Lerp(swaySettings.verticalSwayHolder.transform.localRotation, verticalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
    }

    void ClampXRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }

    #endregion

    public void ResetAtStartPosition() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        xRotationAxisAngle = 0;
    }

    public void ResetToStart() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        verticalChestRotationHolder.rotation = Quaternion.identity;
    }

    int GetInvertMultiplier(bool shouldInvert) {
        int invert = 1;
        if (shouldInvert) {
            invert = -1;
        }
        return invert;
    }

    bool IsMineAndAliveCheck() {
        bool isMine = false;
        if(IsMineCheck() && !health.isDead) {
            isMine = true;
        }
        return isMine;
    }
    
    bool IsMineCheck() {
        bool isMine = false;
        if(photonView.IsMine || playerView.devView) {
            isMine = true;
        }
        return isMine;
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer != TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask && photonView.IsMine) {
            isGrounded = true;
            if (robotParts) {
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

    public bool invertVerticalCam;
    public float mouseSensitivity = 1f;
    //[Range(-90, 180)]
    public float maxVerticalTopViewAngle = 90, maxVerticalBottomViewAngle = 90;
}

[System.Serializable]
public class SwaySettings {
    public Transform horizontalSwayHolder, verticalSwayHolder;
    public float swayIntensity, swaySmooth;

    public Vector3 defaultParentRotation, defaultCamHolderRotation;
}