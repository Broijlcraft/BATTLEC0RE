using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class Controller : MonoBehaviourPun {
    public static Controller single_CLocal;
    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform horizontalCamHolder, verticalCamHolder;
    public Text nicknameText;
    public Transform uiLookAtHolder;
    public Animator animator;
    public float interactRange;
    [Space]
    public float speedToSprintFrom;
    [Space]
    public MoveSettings moveSettings;
    public CameraSettings cameraSettings;
    public SwaySettings swaySettings;

    [Header("Local Settings")]
    public bool hideCursorOnStart;
    public bool keepLocalMeshesEnabled;

    /*[HideInInspector] */public bool canMove;
    [HideInInspector] public Health health;
    [HideInInspector] public Rigidbody rigid;
    /*[HideInInspector]*/ public Collider[] colliders;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform nicknameTarget;
    /*[HideInInspector] */public BodyPartsList robotParts;
    [HideInInspector] public WeaponController weaponsController;

    //replace with bodyparts when ready
    AudioListener audioListeners;
    float currentForwardSprintValue, currentSidewaysSprintValue, xRotationAxisAngle, yRotationAxisAngle;
    public bool isGrounded, isSprinting = false;
    int invertMultiplier;
    public Vector3 defaultHorizontalSwayRotation, defaultVertitalSwayRotation;
    Vector3 lastPos;

    [Header("Testing")]
    public bool disableCamsOnStart;
    public bool keepLocalNicknameTextEnabled;
    public Transform horizontalSwayHolder, verticalSwayHolder;
    public GameObject[] meshObjectsToHideLocally, meshObjectsToSetLocal;

    #region Initialization

    private void Awake() {
        xRotationAxisAngle = 0;
        yRotationAxisAngle = 0;
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        if (IsMineCheck()) {
            single_CLocal = this;
            InvertCamMovement();
        } else {
            Destroy(rigid);
        }
        weaponsController = GetComponent<WeaponController>();
        weaponsController.Init(this);
        health = GetComponent<Health>();
        health.controller = this;
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
            rigid.useGravity = true;
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
        } else {
            for (int i = 0; i < meshObjectsToHideLocally.Length; i++) {
                meshObjectsToHideLocally[i].layer = TagsAndLayersManager.single_TLM.playerLayerInfo.index;
            }
            for (int i = 0; i < meshObjectsToSetLocal.Length; i++) {
                meshObjectsToSetLocal[i].layer = TagsAndLayersManager.single_TLM.playerLayerInfo.index;
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
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }

            if (Input.GetButtonDown("Jump")) {
                if (isGrounded) {
                    //animator.ResetTrigger("JumpLand");
                    //animator.ResetTrigger("Walk");
                    //animator.ResetTrigger("Sprint");
                    //animator.SetTrigger("Jump");
                    Vector3 velocity = rigid.velocity;
                    velocity.y = moveSettings.jumpVelocity;
                    rigid.velocity = velocity;
                    //isGrounded = false;
                }
            }
        }
    }

    float multi;

    public void InvertCamMovement() {
        invertMultiplier = PlayerPrefs.GetInt("InvertCam", 1);
    }

    private void FixedUpdate() {
        if (IsMineCheck() && canMove && !health.isDead) {
            Rotate();

            if (isGrounded || true) {
                SprintCheck();
                float vertical = Input.GetAxis("Vertical") * currentForwardSprintValue * moveSettings.forwardSpeed;
                float horizontal = Input.GetAxis("Horizontal") * currentSidewaysSprintValue * moveSettings.sidewaysSpeed;
                
                Vector3 newPos = new Vector3(horizontal, 0, vertical) * Time.deltaTime;
                transform.Translate(newPos);
            }
        }

        if (animator && isGrounded && isGrounded) {
            float speed = Vector3.Distance(transform.position, lastPos);
            lastPos = transform.position;
            if (/*speed > speedToSprintFrom*/speed > 0.001f) {
                //print("sprint");
                animator.SetBool("Sprint", true);
                animator.SetBool("Walk", false);
                multi = moveSettings.sprintAnimationSpeed;
            }/* else if (speed > 0.001f) {
                //print("walk");
                animator.SetBool("Walk", true);
                animator.SetBool("Sprint", false);
                multi = moveSettings.walkAnimationSpeed;
            }*/ else {
                animator.SetBool("Walk", false);
                animator.SetBool("Sprint", false);
                multi = 1;
            }

            speed /= speed / multi;
            speed = Mathf.Clamp(speed, 0, moveSettings.maxAnimationWalkSpeed);
            if (!float.IsNaN(speed)) {
                animator.SetFloat("MoveSpeed", speed);
            }
        }

        if (nicknameTarget) {
            uiLookAtHolder.LookAt(nicknameTarget);
        }
    }

    void SprintCheck() {
        isSprinting = Input.GetButton("Sprint");

        if (isSprinting) {
            currentForwardSprintValue = moveSettings.forwardSprintSpeed;
            currentSidewaysSprintValue = moveSettings.sidewaysSprintSpeed;
        } else {
            currentForwardSprintValue = 1;
            currentSidewaysSprintValue = 1;
        }
    }

    #region Rotation

    public bool isClamped = false;

    void Rotate() {
        //int multiplier = 0 + Tools.BoolToInt()
        float a_mouseX = Input.GetAxis("Mouse X") * cameraSettings.mouseSensitivity * invertMultiplier;
        float b_mouseX = a_mouseX;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSettings.mouseSensitivity * invertMultiplier;

        xRotationAxisAngle += mouseY;
        yRotationAxisAngle += a_mouseX;

        //ApplySway(mouseX, mouseY);

        if (xRotationAxisAngle > cameraSettings.maxVerticalTopViewAngle) {
            xRotationAxisAngle = cameraSettings.maxVerticalTopViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder, -cameraSettings.maxVerticalTopViewAngle);
        } else if (xRotationAxisAngle < -cameraSettings.maxVerticalBottomViewAngle) {
            xRotationAxisAngle = -cameraSettings.maxVerticalBottomViewAngle;
            mouseY = 0f;
            ClampXRotationAxisToValue(verticalCamHolder, cameraSettings.maxVerticalBottomViewAngle);
        }

        verticalCamHolder.Rotate(Vector3.left * mouseY);

        if (yRotationAxisAngle > cameraSettings.maxRightHorizontalViewAngle) {
            yRotationAxisAngle = cameraSettings.maxRightHorizontalViewAngle;
            a_mouseX = 0f;
            isClamped = true;
            ClampYRotationAxisToValue(horizontalCamHolder, cameraSettings.maxRightHorizontalViewAngle);
        } else if (yRotationAxisAngle < -cameraSettings.maxLeftHorizontalViewAngle) {
            yRotationAxisAngle = -cameraSettings.maxLeftHorizontalViewAngle;
            isClamped = true;
            a_mouseX = 0f;
            ClampYRotationAxisToValue(horizontalCamHolder, -cameraSettings.maxLeftHorizontalViewAngle);
        }

        horizontalCamHolder.transform.Rotate(Vector3.up * a_mouseX);

        if (isClamped) {
            transform.Rotate(Vector3.up * b_mouseX);
            isClamped = false;
        }
    }

    void ApplySway(float mouseX, float mouseY) {
        Quaternion rotX = Quaternion.AngleAxis(-swaySettings.swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swaySettings.swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(defaultHorizontalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;

        Quaternion verticalTemp = Quaternion.Euler(defaultVertitalSwayRotation);
        Quaternion verticalTargetRotation = verticalTemp * rotY;

        if (horizontalSwayHolder) {
            horizontalSwayHolder.localRotation = Quaternion.Lerp(horizontalSwayHolder.transform.localRotation, horizontalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
        }

        if (verticalSwayHolder) {
            verticalSwayHolder.localRotation = Quaternion.Lerp(verticalSwayHolder.transform.localRotation, verticalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
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

    public bool IsMineAndAliveCheck() {
        bool isMine = false;
        if (IsMineCheck() && !health.isDead) {
            isMine = true;
        }
        return isMine;
    }

    public bool IsMineCheck() {
        bool isMine = false;
        if (photonView.IsMine || playerView.devView) {
            isMine = true;
        }
        return isMine;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!isGrounded) {
            isGrounded = true;
            animator.SetTrigger("JumpLand");
            animator.ResetTrigger("Jump");
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (isGrounded) {
            isGrounded = false;
            animator.SetTrigger("Jump");
            animator.ResetTrigger("JumpLand");
        }
    }

    public void EnDisCams() {
        cam.enabled = !cam.enabled;
        localLayerCam.enabled = !localLayerCam.enabled;
    }
}

[System.Serializable]
public class MoveSettings {
    public float maxAnimationWalkSpeed;
    public float forwardSpeed, forwardSprintSpeed;
    public float sidewaysSpeed, sidewaysSprintSpeed;
    [Range(-10,10)]
    public float walkAnimationSpeed, sprintAnimationSpeed;
    [Header("Jumping")]
    public float jumpVelocity;
}

[System.Serializable]
public class CameraSettings {

    public bool invertVerticalCam;
    public float mouseSensitivity, maxVerticalTopViewAngle = 90, maxVerticalBottomViewAngle = 90, maxLeftHorizontalViewAngle, maxRightHorizontalViewAngle;
}

[System.Serializable]
public class SwaySettings {
    public Transform horizontalSwayHolder, verticalSwayHolder;
    public float swayIntensity, swaySmooth;

    public Vector3 defaultParentRotation, defaultCamHolderRotation;
}