using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class TestController : MonoBehaviourPun {
    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform horizontalCamHolder, verticalCamHolder;
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

    Controller controller;
    /*[HideInInspector] */public bool canMove;
    [HideInInspector] public Health health;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform nicknameTarget;
    [HideInInspector] public BodyPartsList robotParts;
    [HideInInspector] public WeaponController weaponsController;

    //replace with bodyparts when ready
    AudioListener audioListeners;
    float currentForwardSprintValue, currentSidewaysSprintValue, xRotationAxisAngle, yRotationAxisAngle;
    bool isGrounded, isSprinting = false;

    public Vector3 defaultHorizontalSwayRotation, defaultVertitalSwayRotation;
    Vector3 lastPos;

    [Header("Testing")]
    public bool disableCamsOnStart;
    public bool keepLocalNicknameTextEnabled;
    public float walkSpeed, walkAnimSpeed, sprintSpeed, sprintAnimSpeed , camSpeed, swayIntensity, swaySmooth, maxSpeedAnim;
    [Range(-90, 90)]
    public float maxLeftHorRot, maxRightHorRot, maxTopVertRot, maxBottomVertRot;
    public Transform horizontalSwayHolder, verticalSwayHolder;
    public GameObject[] meshObjectsToHideLocally, meshObjectsToSetLocal;

    #region Initialization

    private void Awake() {
        xRotationAxisAngle = 0;
        yRotationAxisAngle = 0;
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        if (!photonView.IsMine) {
            Destroy(rigid);
        }
        weaponsController = GetComponent<WeaponController>();
        //weaponsController.Init(controller);
        health = GetComponent<Health>();
        health.controller = controller;
        robotParts = GetComponent<BodyPartsList>();
        if (cam) {
            cam.enabled = false;
            if (localLayerCam) {
                localLayerCam.enabled = false;
            }
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;

        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        }
    }

    private void Start() {
        if (photonView.IsMine && Spawnpoints.sp_Single && PhotonRoomCustomMatchMaking.roomSingle) {
            if (Spawnpoints.sp_Single.spawnpoints.Length > 0) {
                Spawnpoints.sp_Single.SetSpPositionAndRotation(transform, PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom - 1);
            }
        }
        Init();
    }

    void Init() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (IsMineCheck()) {
            if (PhotonRoomCustomMatchMaking.roomSingle) {
                ObjectPool.single_PT.SetPoolOwners(PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom, photonView.ViewID);
            }
            if (!disableCamsOnStart) {
                if (cam) {
                    cam.enabled = true;
                    if (localLayerCam) {
                        localLayerCam.enabled = true;
                    }
                }
                audioListeners.enabled = true;
            }
            for (int i = 0; i < meshObjectsToHideLocally.Length; i++) {
                meshObjectsToHideLocally[i].layer = TagsAndLayersManager.single_TLM.cantBeSeenByPlayer.index;
            }
            for (int i = 0; i < meshObjectsToSetLocal.Length; i++) {
                meshObjectsToSetLocal[i].layer = TagsAndLayersManager.single_TLM.localPlayerLayerInfo.index;
            }
        }
        TurnCollidersOnOff(true);
        //canMove = true;
        //Debug.LogWarning("(bool)Can Move WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
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
    void RPC_SetNicknameTargets() {
        if (photonView.IsMine) {
            Controller[] controllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].nicknameTarget = cam.transform;
                controllers[i].nicknameText.text = Tools.RemoveIdFromNickname(controllers[i].photonView.Owner.NickName);
                int index = TeamManager.single_TM.GetTeamIndex(controllers[i].photonView.Owner.NickName);
                if (index >= 0) {
                    controllers[i].nicknameText.color = TeamManager.single_TM.teams[index].teamColor;
                }
                if (controllers[i] == this && !keepLocalNicknameTextEnabled) {
                    controllers[i].uiLookAtHolder.gameObject.SetActive(false);
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
                        hit.transform.GetComponent<Interactable>().Interact(controller);
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
            float vertical = Input.GetAxis("Vertical") * currentForwardSprintValue * walkSpeed;
            float horizontal = Input.GetAxis("Horizontal") * currentSidewaysSprintValue *walkSpeed;

            if (animator) {
                if (vertical != 0 || horizontal != 0) {
                    animator.SetBool("Walk", !isSprinting);
                    animator.SetBool("Sprint", isSprinting);
                } else {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Sprint", false);
                }
            }

            Vector3 newPos = new Vector3(horizontal, 0, vertical) * Time.deltaTime;
            transform.Translate(newPos);
        }

        if (animator) {
            float speed = Vector3.Distance(transform.position, lastPos);
            lastPos = transform.position;
            float multi = walkAnimSpeed;
            if (isSprinting) {
                multi = sprintAnimSpeed;
            }
            speed /= speed / multi;
            speed = Mathf.Clamp(speed, 0, maxSpeedAnim);
            if (!float.IsNaN(speed)) {
                animator.SetFloat("MoveSpeed", speed);
                print(speed);
            }
        }

        if (nicknameTarget) {
            uiLookAtHolder.LookAt(nicknameTarget);
        }
    }

    private void LateUpdate() {
        if (IsMineCheck() && canMove && !health.isDead) {
           // Rotate();
        }
    }

    void SprintCheck() {
        isSprinting = Input.GetButton("Sprint");

        if (isSprinting) {
            currentForwardSprintValue = sprintSpeed;
            currentSidewaysSprintValue = sprintSpeed;
        } else {
            currentForwardSprintValue = 1;
            currentSidewaysSprintValue = 1;
        }
    }

    #region Rotation

    public bool isClamped = false;

    void Rotate() {
        float a_mouseX = Input.GetAxis("Mouse X") * camSpeed;
        float b_mouseX = a_mouseX;
        float mouseY = Input.GetAxis("Mouse Y") * camSpeed;

        xRotationAxisAngle += mouseY;
        yRotationAxisAngle += a_mouseX;

        //ApplySway(mouseX, mouseY);

        if (xRotationAxisAngle > maxTopVertRot) {
            xRotationAxisAngle = maxTopVertRot;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder, -maxTopVertRot);
        } else if (xRotationAxisAngle < -maxBottomVertRot) {
            xRotationAxisAngle = -maxBottomVertRot;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder, maxBottomVertRot);
        }

        verticalCamHolder.Rotate(Vector3.left * mouseY);

        if (yRotationAxisAngle > maxRightHorRot) {
            yRotationAxisAngle = maxRightHorRot;
            a_mouseX = 0f;
            isClamped = true;
            ClampYRotationAxisToValue(horizontalCamHolder, maxRightHorRot);
        } else if (yRotationAxisAngle < -maxLeftHorRot) {
            yRotationAxisAngle = -maxLeftHorRot;
            isClamped = true;
            a_mouseX = 0f;
            ClampYRotationAxisToValue(horizontalCamHolder, -maxLeftHorRot);
        }

        horizontalCamHolder.transform.Rotate(Vector3.up * a_mouseX);

        if (isClamped) {
            transform.Rotate(Vector3.up * b_mouseX);
            isClamped = false;
        }
    }

    void ApplySway(float mouseX, float mouseY) {
        Quaternion rotX = Quaternion.AngleAxis(-swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(defaultHorizontalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;

        Quaternion verticalTemp = Quaternion.Euler(defaultVertitalSwayRotation);
        Quaternion verticalTargetRotation = verticalTemp * rotY;

        if (horizontalSwayHolder) {
            horizontalSwayHolder.localRotation = Quaternion.Lerp(horizontalSwayHolder.transform.localRotation, horizontalTargetRotation, Time.deltaTime * swaySmooth);
        }

        if (verticalSwayHolder) {
            verticalSwayHolder.localRotation = Quaternion.Lerp(verticalSwayHolder.transform.localRotation, verticalTargetRotation, Time.deltaTime * swaySmooth);
        }
    }

    void ClampXRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }

    void ClampYRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.y = value;
        transform_.localEulerAngles = eulerRotation;
    }

    #endregion

    public void ResetAtStartPosition() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        xRotationAxisAngle = 0;
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
        if (IsMineCheck() && !health.isDead) {
            isMine = true;
        }
        return isMine;
    }

    bool IsMineCheck() {
        bool isMine = false;
        if (photonView.IsMine || playerView.devView) {
            isMine = true;
        }
        return isMine;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer != TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask && photonView.IsMine) {
            isGrounded = true;
            if (robotParts) {
            }
        }
    }

    public void EnDisCams() {
        cam.enabled = !cam.enabled;
        localLayerCam.enabled = !localLayerCam.enabled;
    }
}