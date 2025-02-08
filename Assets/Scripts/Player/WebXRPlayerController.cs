using UnityEngine;
using WebXR;

public class WebXRPlayerController : MonoBehaviour
{
    public float speed = 3f;
    public float rotationAngle = 30f; // Snap rotation angle
    public Transform vrCamera;
    public GameObject fireEffect; // Fire effect prefab

    private Rigidbody rb;
    private WebXRController leftController;
    private WebXRController rightController;
    private bool isVR = false;
    private bool fireActive = false;
    private bool canRotate = true; // Prevents continuous rotation from a single swipe

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Missing Rigidbody on the Player");
        }
        else
        {
            rb.freezeRotation = true; // Prevents unwanted rotation
        }

        if (WebXRManager.Instance != null && WebXRManager.Instance.isSupportedVR)
        {
            isVR = true;
            Debug.Log("VR is supported and enabled.");
            FindWebXRControllers();
        }
        else
        {
            isVR = false;
            Debug.Log("VR is not supported. Using keyboard controls.");
        }

        if (fireEffect != null)
        {
            fireEffect.SetActive(false); // Ensure fire effect is initially off
        }
    }

    private void Update()
    {
        if (isVR)
        {
            HandleVRMovement();
            HandleVRFire();
        }
        else
        {
            HandleKeyboardMovement();
            HandleNonVRFire();
        }
    }

    private void HandleVRMovement()
    {
        if (rightController == null) return;

        Vector2 joystickInput = rightController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);

        // Movement logic
        if (joystickInput.magnitude > 0.1f)
        {
            Vector3 moveDirection = vrCamera.forward * joystickInput.y + vrCamera.right * joystickInput.x;
            moveDirection.y = 0;
            rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
        }

        // Snap Rotation logic using thumbstick X-axis
        if (canRotate)
        {
            if (joystickInput.x > 0.5f) // Right swipe
            {
                transform.Rotate(0, rotationAngle, 0);
                canRotate = false; // Prevents continuous rotation from holding the thumbstick
            }
            else if (joystickInput.x < -0.5f) // Left swipe
            {
                transform.Rotate(0, -rotationAngle, 0);
                canRotate = false;
            }
        }
        else if (Mathf.Abs(joystickInput.x) < 0.2f) // Reset canRotate when joystick is centered
        {
            canRotate = true;
        }
    }

    private void HandleKeyboardMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        Vector3 cameraForward = vrCamera.forward;
        Vector3 cameraRight = vrCamera.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 adjustedMoveDirection = cameraForward * moveZ + cameraRight * moveX;

        if (adjustedMoveDirection.magnitude > 0)
        {
            rb.linearVelocity = new Vector3(adjustedMoveDirection.x * speed, rb.linearVelocity.y, adjustedMoveDirection.z * speed);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, -rotationAngle, 0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(0, rotationAngle, 0);
        }
    }

    private void HandleVRFire()
    {
        if (rightController == null) return;
        bool gripPressed = rightController.GetButton(WebXRController.ButtonTypes.Grip);
        bool triggerPressed = rightController.GetButton(WebXRController.ButtonTypes.Trigger);

        if ((gripPressed || triggerPressed) && !fireActive)
        {
            StartFire();
        }
        else if (!gripPressed && !triggerPressed && fireActive)
        {
            StopFire();
        }
    }

    private void HandleNonVRFire()
    {
        if (Input.GetKeyDown(KeyCode.F) && !fireActive)
        {
            StartFire();
        }
        else if (Input.GetKeyUp(KeyCode.F) && fireActive)
        {
            StopFire();
        }
    }

    private void StartFire()
    {
        if (fireEffect != null)
        {
            fireEffect.SetActive(true);
            fireActive = true;
        }
    }

    private void StopFire()
    {
        if (fireEffect != null)
        {
            fireEffect.SetActive(false);
            fireActive = false;
        }
    }

    private void FindWebXRControllers()
    {
        WebXRController[] controllers = FindObjectsOfType<WebXRController>();

        foreach (WebXRController controller in controllers)
        {
            if (controller.hand == WebXRControllerHand.LEFT)
            {
                leftController = controller;
                Debug.Log("✅ Left Controller Assigned: " + leftController.name);
            }
            else if (controller.hand == WebXRControllerHand.RIGHT)
            {
                rightController = controller;
                Debug.Log("✅ Right Controller Assigned: " + rightController.name);
            }
        }

        if (leftController == null || rightController == null)
        {
            Debug.LogError("❌ WebXR controllers are missing!");
        }
    }
}
