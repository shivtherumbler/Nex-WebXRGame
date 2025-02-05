using UnityEngine;
using WebXR;

public class WebXRPlayerController : MonoBehaviour
{
    public float speed = 3f; // Player movement speed
    public GameObject freezeEffect; // Prefab for frozen enemies
    public float mouseSensitivity = 2f; // Sensitivity of mouse movement

    public GameObject leftHandPrefab;  // Hand model for left hand
    public GameObject rightHandPrefab; // Hand model for right hand
    public Camera playerCamera;


    private WebXRController leftController;
    private WebXRController rightController;
    private CharacterController characterController;

    private GameObject leftHandObject;  // To store the left hand model instance
    private GameObject rightHandObject; // To store the right hand model instance

    private bool isVR = false; // Whether the game is running in VR
    private float pitch = 0f;  // Vertical camera rotation


    void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("❌ CharacterController component is missing on the player!");
        }

        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("❌ Camera component is missing on the player!");
        }

        // Check if VR is supported, improve performance by checking once
        isVR = WebXRManager.Instance != null && WebXRManager.Instance.isSupportedVR;

        // Find WebXR controllers
        FindWebXRControllers();

        // Validate freeze effect prefab
        if (freezeEffect == null)
        {
            Debug.LogError("❌ Freeze Effect Prefab is missing!");
        }

        // Spawn hands for VR
        if (isVR)
        {
            SpawnHands();
        }
    }

    void Update()
    {
        if (isVR)
        {
            HandleMovement();
            HandleAttack();
        }
        else
        {
            HandlePCMovement();
            HandleAttack();
            HandlePCMouseLook();  // Handle mouse look for PC

        }

        // Update hands' positions
        if (isVR)
        {
            UpdateHandsPosition();
        }
    }

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        // **VR Controls (WebXR Joystick)**
        if (rightController != null)
        {
            Vector2 joystickInput = rightController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);
            moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
        }

        // Adjust movement based on camera's forward direction
        if (moveDirection.magnitude > 0)
        {
            // Get the camera's forward and right direction
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            // Flatten the vectors to ignore the vertical axis (up/down)
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normalize them
            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;

            // Calculate movement direction based on camera's orientation
            moveDirection = (cameraForward * moveDirection.y + cameraRight * moveDirection.x).normalized;
        }

        ApplyMovement(moveDirection);
    }

    void HandlePCMovement()
    {
        // **PC Controls (Keyboard)**
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Adjust movement based on camera's forward direction
        if (moveDirection.magnitude > 0)
        {
            // Get the camera's forward and right direction
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            // Flatten the vectors to ignore the vertical axis (up/down)
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normalize them
            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;

            // Calculate movement direction based on camera's orientation
            moveDirection = (cameraForward * moveDirection.z + cameraRight * moveDirection.x).normalized;
        }

        ApplyMovement(moveDirection);
    }

    void HandlePCMouseLook()
    {
        // **PC Mouse Look**
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally (Y-axis rotation)
        transform.Rotate(Vector3.up * mouseX);

        // Limit vertical camera rotation
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);  // Limit looking up and down

        // Apply vertical camera rotation
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    void ApplyMovement(Vector3 moveDirection)
    {
        if (moveDirection.magnitude > 0 && characterController != null)
        {
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    void HandleAttack()
    {
        bool attackPressed = false;

        // **Check for attack button (Trigger)**
        if (isVR && rightController != null)
        {
            attackPressed = rightController.GetButtonDown(WebXRController.ButtonTypes.Trigger);
        }
        else
        {
            attackPressed = Input.GetMouseButtonDown(0); // Left mouse button for PC
        }

        if (attackPressed)
        {
            Debug.Log("🧊 Attack! Freezing Enemy!");
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Raycast to find an enemy
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                FreezeEnemy(hit.collider.gameObject);
            }
        }
    }

    void FreezeEnemy(GameObject enemy)
    {
        if (freezeEffect != null)
        {
            // Replace enemy with frozen effect
            Instantiate(freezeEffect, enemy.transform.position, Quaternion.identity);
            Destroy(enemy); // Remove original enemy
        }
        else
        {
            Debug.LogError("❌ Freeze Effect Prefab is missing!");
        }
    }

    void FindWebXRControllers()
    {
        // Find all WebXRController components in the scene
        WebXRController[] controllers = FindObjectsOfType<WebXRController>();

        foreach (WebXRController controller in controllers)
        {
            if (controller.hand == WebXRControllerHand.LEFT)
            {
                leftController = controller;
            }
            else if (controller.hand == WebXRControllerHand.RIGHT)
            {
                rightController = controller;
            }
        }

        if (leftController == null || rightController == null)
        {
            Debug.LogWarning("⚠️ One or both WebXR controllers are missing!");
        }
    }

    void SpawnHands()
    {
        if (leftHandPrefab != null && rightHandPrefab != null)
        {
            leftHandObject = Instantiate(leftHandPrefab, transform);
            rightHandObject = Instantiate(rightHandPrefab, transform);

            // Position the hands at the controllers
            leftHandObject.transform.localPosition = leftController.transform.localPosition;
            rightHandObject.transform.localPosition = rightController.transform.localPosition;
        }
        else
        {
            Debug.LogError("❌ Left and Right Hand Prefabs are missing!");
        }
    }

    void UpdateHandsPosition()
    {
        // Update the position of the hands based on the controllers
        if (leftController != null && leftHandObject != null)
        {
            leftHandObject.transform.position = leftController.transform.position;
            leftHandObject.transform.rotation = leftController.transform.rotation;
        }

        if (rightController != null && rightHandObject != null)
        {
            rightHandObject.transform.position = rightController.transform.position;
            rightHandObject.transform.rotation = rightController.transform.rotation;
        }
    }
}
