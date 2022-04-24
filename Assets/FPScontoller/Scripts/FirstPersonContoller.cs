using System.Collections;
using UnityEngine;

public class FirstPersonContoller : MonoBehaviour
{
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);
    private bool shouldJump => canJump && Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool shouldCrouch =>  Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
    public bool CanMove { get; private set; } = true;

    [Header("Functional Options")]
    [SerializeField] public bool canSprint = true;
    [SerializeField] public bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootSteps = true;
    [SerializeField] private bool enableStaminaBar = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1; //right button
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0; //right button

    [Header("Movement Parametrs")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 5f;
    [SerializeField] private float sprintStaminaUsage = 0.1f;
    [SerializeField] private float jumpStaminaUsage = 10f;

  
    [Header("Look Parametrs")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jump Parametrs")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parametrs")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0.0f,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;


    [Header("Headbob Parametrs")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parametrs")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Footsteps Parametrs")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;


    


    //SLIDER PARAMETRS
    private Vector3 hitPointNormal;
    private Vector3 realSlopeDirection;
    private bool isSliding
    {
           get
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.red);
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2.0f))
            {
                hitPointNormal = slopeHit.normal;
                realSlopeDirection = Vector3.Cross(Vector3.Cross(slopeHit.normal, Vector3.down), slopeHit.normal);
                return Vector3.Angle(realSlopeDirection, Vector3.up) > characterController.slopeLimit;  
            }
            else return false;
        }
    }


    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactLayer = default;
    private Interactable currentInteractable;




    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos=playerCamera.transform.localPosition.y;//for headbob, we want to return original camera position
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();//���� ������������
            HandleMouseLock();//������ ������ �� ��
            if (canJump)//������
                 HandleJump();

            if (canCrouch)
                HandleCrouch();
            if (canUseHeadBob)
                HandleHeadBob();
            if (canZoom)
                HandleZoom();
            if (useFootSteps)
                HandleFootsteps();

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            ApplyFinalMovements(); //������������ ��� ������� � ��������� ������������
        }
    }
    //==================MOVEMENT====================
    private void HandleMovementInput()
    {
        float moveStatement = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;
        if (isSprinting && !isCrouching)//���� �����, ���� ���������, ���� � ��������� ������������
        {
            currentInput = new Vector2(sprintSpeed * Input.GetAxis("Vertical"), moveStatement * Input.GetAxis("Horizontal"));//������� ���������, ��� �����
            if (enableStaminaBar) StaminaBar.instance.UseStamina(sprintStaminaUsage);//�������� �������, ���������� � ����������
 
        }
        else currentInput = new Vector2(moveStatement * Input.GetAxis("Vertical"), moveStatement * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)//is character is not on the ground, we fall
            moveDirection.y -= gravity * Time.deltaTime;

        if (willSlideOnSlopes && isSliding)//if we on slope, we must fall
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;

        characterController.Move(moveDirection * Time.deltaTime); //move into direction
        

    }
    //=================MOUSE ROTATION====================
    private void HandleMouseLock()//���������� �����
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY; //rotation on x (up & down);
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);// ����� ����������� �� �� �����
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);//��
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X")*lookSpeedX, 0);//��
    }
    //==================JUMPING========================
    private void HandleJump()//������
    {
        if (shouldJump)
        {
            if(enableStaminaBar) StaminaBar.instance.UseStamina(jumpStaminaUsage);//�������� �������, ���������� � ���������e
            moveDirection.y = jumpForce;
        }
    }
    //================CROUCHING=======================
    private void HandleCrouch()
    {
        if (shouldCrouch)
            StartCoroutine(CrouchStand());
    }
    private IEnumerator CrouchStand()
    {

        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))//����� ������ ���� ������, ���� ��� ������
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //��� ������� � ������ ����, ���� �������� ����� ������� = 0
        characterController.height = targetHeight;
        characterController.center = targetCenter;
        isCrouching = !isCrouching;
        duringCrouchAnimation = false;
    }
    //=========================HEAD BOB=======================
    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;//if we are not on the ground, we should`t bob
        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)//return abs way |move.direction|
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x, //x pos
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),//Y + sin(timer) (����� ���� 1, ���� -1, ������� ������ ����� ������ ����� � ����)
                playerCamera.transform.localPosition.z);//y pos
        }
    }
    //=================ZOOM===========================
    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))//������ ���
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))//������ ���
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }
    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;//needed fov
        float startingFOV = playerCamera.fieldOfView;//our fov
        float timeElapsed = 0;//time  elapsed

        while (timeElapsed < startingFOV)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed/timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
    //=========================INTERACTION===============
    private void HandleInteractionCheck()//const raycast look for objects
    {
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 6 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))//layer with interactable and check for update interactable objects
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if(currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }

        //On loose focus
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }
    private void HandleInteractionInput()//hit interact key
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactLayer)) {
            currentInteractable.OnInteract();
        }

    }
    //=======================FOOTSTEPS=======
    private void HandleFootsteps() 
    {
        if (!characterController.isGrounded) return;

        if (currentInput == Vector2.zero) return; 

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 2))
            {
                switch(hit.collider.tag)
                {
                    case "footsteps/WOOD":
                        footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break; 
                    case "footsteps/GRASS":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    case "footsteps/METAL":
                        footstepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);//���� ��� ������� �����������, ������ ����� ������
                        break;
                }
            }
            footstepTimer = GetCurrentOffset;
        }
    }
    //================================
}
