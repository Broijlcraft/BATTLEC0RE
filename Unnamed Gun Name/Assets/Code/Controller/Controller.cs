using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Controller : MonoBehaviourPun {
    public static Controller single_CLocal;
    public PlayerView playerView;
    public Camera cam, localLayerCam;
    public Transform verticalCamHolder, uiLookAtHolder;
    public Text nicknameText;
    public Animator animator;
    [Space]
    public float interactRange;
    [Space]
    public MoveSettings moveSettings;
    public CameraSettings cameraSettings;
    public SwaySettings swaySettings;
    public GameObject[] meshObjectsToSetLocal;

    [Header("Local Settings")]
    public bool hideCursorOnStart;

    /*[HideInInspector]*/ public bool canMove, isActive;
    [HideInInspector] public Health health;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Transform nicknameTarget;
    [HideInInspector] public WeaponController weaponsController;
    [HideInInspector] public BodyPartsList bpList;

    //replace with bodyparts when ready
    bool isGrounded;
    int invertMultiplier;
    AudioListener audioListeners;
    float currentSpeed, xRotationAxisAngle, vertical = 0, horizontal = 0, horSpeed = 0, verSpeed = 0;

    [Header("Testing")]
    public bool keepLocalNicknameTextEnabled;

    #region Initialization
    private void Awake() {
        if (!playerView.devView) {
            canMove = false;
            isGrounded = true;
        }
        xRotationAxisAngle = 0;
        EnableColliders(false);
        rigid = GetComponent<Rigidbody>();

        bpList = GetComponent<BodyPartsList>();
        bpList.Init();

        if (IsMineCheck()) {
            Controller.single_CLocal = this;
            InvertCamMovement();
        } else {
            rigid.useGravity = false;
        }

        weaponsController = GetComponent<WeaponController>();
        weaponsController.controller = this;

        health = GetComponent<Health>();
        health.controller = this;

        bool isMine = photonView.IsMine;
        if (cam) {
            cam.enabled = isMine;
            if (localLayerCam) {
                localLayerCam.enabled = isMine;
            }
        }

        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = isMine;

        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        }
    }

    private void Start() {
        EnableColliders(true);
        if (photonView.IsMine) {
            if (Spawnpoints.sp_Single && PhotonRoomCustomMatchMaking.roomSingle) {
                if (Spawnpoints.sp_Single.spawnpoints.Length > 0) {
                    Spawnpoints.sp_Single.SetSpPositionAndRotation(transform, PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom - 1);
                }
            }
            if (hideCursorOnStart) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            animator.SetFloat("HorizontalInput", 0f);
            animator.SetFloat("VerticalInput", 0f);
            animator.SetFloat("MoveSpeed", 0f);
        }
    }

    public void Init() {
        photonView.RPC(nameof(RPC_Init), RpcTarget.All);
    }

    [PunRPC]
    void RPC_Init() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (IsMineCheck()) {
            weaponsController.Init();
            bpList.SetMeshes(BodyPartsManager.single_bpm.currentSelectedRobot);
            rigid.useGravity = true;
            if (PhotonRoomCustomMatchMaking.roomSingle) {
                ObjectPool.single_PT.SetPoolOwners(PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom, photonView.ViewID);
            }
            isGrounded = true;

            if (TagsAndLayersManager.single_TLM) {
                for (int i = 0; i < meshObjectsToSetLocal.Length; i++) {
                    meshObjectsToSetLocal[i].layer = TagsAndLayersManager.single_TLM.localPlayerLayerInfo.index;
                }
            }
        } else {
            if (TagsAndLayersManager.single_TLM) {
                for (int i = 0; i < meshObjectsToSetLocal.Length; i++) {
                    meshObjectsToSetLocal[i].layer = TagsAndLayersManager.single_TLM.playerLayerInfo.index;
                }
            }
        }
        isActive = true;
    }

    void EnableColliders(bool state) {
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
        if (IsMineAndAlive() && isActive) {
            if (Input.GetButtonDown("Interact")) {
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactRange, ~TagsAndLayersManager.single_TLM.localPlayerLayerInfo.layerMask)) {
                    if (hit.transform.CompareTag(TagsAndLayersManager.single_TLM.interactableTag)) {
                        hit.transform.GetComponent<Interactable>().Interact(this);
                    }
                }
            }

            if (isGrounded) {
                if (Input.GetButtonDown("Jump")) {
                    rigid.velocity = new Vector3(0f, 5f, 0f);
                    animator.SetTrigger("Jump");
                    isGrounded = false;
                }

            }
            if (!Physics.Raycast(transform.position + moveSettings.jumpOffsetCheck, Vector3.down, moveSettings.distanceCheck, moveSettings.walkableLayers)) {
                isGrounded = false;
            }
        }
        horSpeed = Input.GetAxis("Horizontal");
        verSpeed = Input.GetAxis("Vertical");
    }

    public void InvertCamMovement() {
        invertMultiplier = PlayerPrefs.GetInt("InvertCam", 1);
    }

    private void FixedUpdate() {
        bool imaa = IsMineAndAlive();
        if (imaa && isActive) {
            SprintCheck();

            float animSpeed = currentSpeed / moveSettings.sprintSpeed;
            animSpeed = Mathf.Clamp(animSpeed, 0, 1);

            if (canMove) {
                Rotate();
                if (isGrounded || true) {
                    animator.SetFloat("HorizontalInput", horSpeed);
                    animator.SetFloat("VerticalInput", verSpeed);

                    horizontal = horSpeed * currentSpeed;
                    vertical = verSpeed * currentSpeed;

                    Vector3 newPos = new Vector3(horizontal, 0, vertical) * Time.deltaTime;
                    transform.Translate(newPos);
                }
            } else {
                animSpeed = 0;
            }
            animator.SetFloat("MoveSpeed", animSpeed);

        } else if (!IsMineCheck()) {
            rigid.velocity = Vector3.zero;
        }

        if (nicknameTarget) {
            uiLookAtHolder.LookAt(nicknameTarget);
        }
    }

    void SprintCheck() {
        if ((horSpeed > 0 || horSpeed < 0) || (verSpeed > 0 || verSpeed < 0)) {
            if (Input.GetButton("Sprint")) {
                if (currentSpeed < moveSettings.sprintSpeed) {
                    currentSpeed += moveSettings.sprintAccelerate * Time.deltaTime;
                }
            } else {
                if (currentSpeed < moveSettings.walkingSpeed - moveSettings.walkAccelerate * Time.deltaTime) {
                    currentSpeed += moveSettings.walkAccelerate * Time.deltaTime;
                } else if (currentSpeed > moveSettings.walkingSpeed + moveSettings.sprintAccelerate * Time.deltaTime) {
                    currentSpeed -= moveSettings.sprintSpeed * Time.deltaTime;
                } else if (currentSpeed > moveSettings.walkingSpeed - moveSettings.walkAccelerate * Time.deltaTime && currentSpeed < moveSettings.walkingSpeed + moveSettings.sprintAccelerate * Time.deltaTime) {
                    currentSpeed = moveSettings.walkingSpeed;
                }
            }
        } else {
            if (currentSpeed > 0) {
                currentSpeed -= moveSettings.walkAccelerate * Time.deltaTime;
            }
            if (currentSpeed < 0) {
                currentSpeed = 0;
            }
        }
    }

    #region Rotation
    void Rotate() {
        //int multiplier = 0 + Tools.BoolToInt()
        float a_mouseX = Input.GetAxis("Mouse X") * cameraSettings.mouseSensitivity * invertMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSettings.mouseSensitivity * invertMultiplier;

        xRotationAxisAngle += mouseY;

        ApplySway(a_mouseX, mouseY);

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
        transform.Rotate(Vector3.up * a_mouseX);
    }

    void ApplySway(float mouseX, float mouseY) {
        Quaternion rotX = Quaternion.AngleAxis(-swaySettings.swayIntensity * mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(swaySettings.swayIntensity * mouseY, Vector3.right);

        Quaternion horizontalTemp = Quaternion.Euler(swaySettings.defaultHorizontalSwayRotation);
        Quaternion horizontalTargetRotation = horizontalTemp * rotX;

        Quaternion verticalTemp = Quaternion.Euler(swaySettings.defaultVertitalSwayRotation);
        Quaternion verticalTargetRotation = verticalTemp * rotY;

        Transform horizontalSwayHolder = swaySettings.horizontalSwayHolder;
        Transform verticalSwayHolder = swaySettings.verticalSwayHolder;

        if (horizontalSwayHolder) {
            horizontalSwayHolder.localRotation = Quaternion.Lerp(horizontalSwayHolder.localRotation, horizontalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
        }

        if (verticalSwayHolder) {
            verticalSwayHolder.localRotation = Quaternion.Lerp(verticalSwayHolder.localRotation, verticalTargetRotation, Time.deltaTime * swaySettings.swaySmooth);
        }
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
        verticalCamHolder.localRotation = Quaternion.identity;
        xRotationAxisAngle = 0;
    }

    public bool IsMineAndAlive() {
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
        if (IsMineAndAlive() && !isGrounded) {
            animator.ResetTrigger("Jump");
            animator.SetBool("JumpLand", true);
            isGrounded = true;
            StopCoroutine(CheckLanding());
            StartCoroutine(CheckLanding());
        }
    }

    IEnumerator CheckLanding() {
        yield return new WaitForSeconds(0.1f);
        canMove = true;
        animator.SetBool("JumpLand", false);
    }

    private void OnDrawGizmosSelected() {
        Debug.DrawRay(transform.position + moveSettings.jumpOffsetCheck, Vector3.down * moveSettings.distanceCheck, Color.cyan);
    }
}

[System.Serializable]
public class MoveSettings {
    public float walkingSpeed;
    public float walkAccelerate;
    [Space]
    public float sprintSpeed;
    public float sprintAccelerate;
    [Header("Jumping")]
    public float jumpVelocity;
    public float distanceCheck;
    public Vector3 jumpOffsetCheck;
    public LayerMask walkableLayers;
}

[System.Serializable]
public class CameraSettings {
    public bool invertVerticalCam;
    public float mouseSensitivity, maxVerticalTopViewAngle = 90, maxVerticalBottomViewAngle = 90;
}

[System.Serializable]
public class SwaySettings {
    public Transform horizontalSwayHolder, verticalSwayHolder;
    public float swayIntensity, swaySmooth;

    public Vector3 defaultHorizontalSwayRotation, defaultVertitalSwayRotation;
}