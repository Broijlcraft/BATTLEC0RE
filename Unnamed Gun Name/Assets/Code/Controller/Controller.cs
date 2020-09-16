using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody), typeof(Health))]
[RequireComponent(typeof(PlayerView), typeof(PhotonView), typeof(WeaponController))] 
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform horizontalCamHolder, verticalHolder;
    public Text nicknameText;
    public Transform uiLookAtHolder;
    public float interactRange;
    public Animator animator;

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

    Vector3 defaultHorizontalSwayRotation, defaultVertitalSwayRotation;

    [Header("Testing")]
    public bool disableCamsOnStart;

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
        if (photonView.IsMine || playerView.devView) {
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
            TurnCollidersOnOff(true);
        } else {
            rigid.isKinematic = false;
        }
        canMove = true;
        Debug.LogWarning("(bool)Can Move WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update() {
        if ((photonView.IsMine || playerView.devView) && !health.isDead) {
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

    Vector3 lastPos = Vector3.zero;

    private void FixedUpdate() {
        float speed = Vector3.Distance(transform.position, lastPos) * 50;
        lastPos = transform.position;
        print(speed);
        //animator.SetFloat("MoveSpeed", speed);
        if (((canMove && photonView.IsMine) || playerView.devView) && !health.isDead) {
            Rotate();

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * currentForwardSprintValue * forwardsSpeedSettings.defaultSpeed;
            float horizontal = Input.GetAxis("Horizontal") * currentSidewaysSprintValue * sidewaysSpeedSettings.defaultSpeed;

            if(vertical == 0 && horizontal == 0) {
                animator.SetBool(robotParts.walk, false);
                animator.SetBool(robotParts.sprint, false);
            }

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
            animator.SetBool(robotParts.sprint, true);
            animator.SetBool(robotParts.walk, false);
            isSprinting = true;
        } else {
            currentForwardSprintValue = 1;
            currentSidewaysSprintValue = 1;
            animator.SetBool(robotParts.sprint, false);
            animator.SetBool(robotParts.walk, true);
            isSprinting = false;
        }
    }

    void Rotate() {
        float sense = cameraSettings.mouseSensitivity, invert = GetInvertMultiplier(cameraSettings.invertVerticalCam);
        float mouseX = Input.GetAxis("Mouse X") * sense * -invert;
        float mouseY = Input.GetAxis("Mouse Y") * sense * invert;

        Quaternion rotX = Quaternion.AngleAxis(-swaySettings.swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swaySettings.swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(defaultHorizontalSwayRotation);
        Quaternion verticalTemp = Quaternion.Euler(defaultVertitalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;
        Quaternion verticalTargetRotation = verticalTemp * rotY;
        swaySettings.horizontalSwayHolder.localRotation = Quaternion.Lerp(swaySettings.horizontalSwayHolder.transform.localRotation, horizontalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
        swaySettings.verticalSwayHolder.localRotation = Quaternion.Lerp(swaySettings.verticalSwayHolder.transform.localRotation, verticalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);

        xRotationAxisAngle += mouseY;

        if (xRotationAxisAngle > cameraSettings.maxVerticalTopViewAngle) {
            xRotationAxisAngle = cameraSettings.maxVerticalTopViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalHolder.transform, -cameraSettings.maxVerticalTopViewAngle);
        } else if (xRotationAxisAngle < -cameraSettings.maxVerticalBottomViewAngle) {
            xRotationAxisAngle = -cameraSettings.maxVerticalBottomViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalHolder.transform, cameraSettings.maxVerticalBottomViewAngle);
        }
        verticalHolder.transform.Rotate(Vector3.left * mouseY);
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
        verticalHolder.localRotation = Quaternion.identity;
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

    public void ResetToStart() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        verticalHolder.rotation = Quaternion.identity;
        cam.transform.rotation = Quaternion.identity;
    }

    int GetInvertMultiplier(bool shouldInvert) {
        int invert = 1;
        if (shouldInvert) {
            invert = -1;
        }
        return invert;
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