using UnityEngine;
using Zinnia.Action;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public GameObject handFlameEffect;  // Flame effect
    [SerializeField] private BooleanAction ThumbTouch;
    [SerializeField] private BooleanAction GripPress, TriggerPress;

    private int ThumbLayer = 1, TriggerLayer = 2, GripLayer = 3;
    private bool ThumbTouched = false, GripPressed = false, TriggerPressed = false;
    private bool isVR = false;
    private bool FlameActive = false;

    void Start()
    {
        // Detect if VR is supported
        if (WebXR.WebXRManager.Instance != null)
        {
            isVR = WebXR.WebXRManager.Instance.isSupportedVR;
            Debug.Log(isVR ? "VR Mode Enabled" : "Non-VR Mode Enabled");
        }
        else
        {
            Debug.LogError("WebXRManager not found! Running in Non-VR Mode.");
        }

        // Initially disable flame effect
        if (handFlameEffect != null) handFlameEffect.SetActive(false);
    }

    void Update()
    {
        if (isVR)
        {
            HandleFlameThrowingVR();
        }

        HandleHandAnimations();
    }

    // VR Mode: Detect Trigger or Grip Press
    private void HandleFlameThrowingVR()
    {
        bool gripPressed = (GripPress != null && GripPress.Value);
        bool triggerPressed = (TriggerPress != null && TriggerPress.Value);
        bool isPressing = gripPressed || triggerPressed;

        // Activate flame effect only if grip or trigger is pressed
        if (handFlameEffect != null)
        {
            handFlameEffect.SetActive(isPressing);
        }
    }

    // Handles Hand Animation
    private void HandleHandAnimations()
    {
        HandleBooleanActionAnimation(ThumbTouch, ref ThumbTouched, ThumbLayer);
        HandleBooleanActionAnimation(GripPress, ref GripPressed, GripLayer);
        HandleBooleanActionAnimation(TriggerPress, ref TriggerPressed, TriggerLayer);
    }

    private void HandleBooleanActionAnimation(BooleanAction action, ref bool state, int layer)
    {
        if (action != null)
        {
            bool isActive = action.Value;
            if (isActive && !state)
            {
                anim.SetLayerWeight(layer, 1);
                state = true;

                // Only activate flame effect for Grip or Trigger layers
                if (layer != ThumbLayer && handFlameEffect != null)
                {
                    handFlameEffect.SetActive(true);
                }
            }
            else if (!isActive && state)
            {
                anim.SetLayerWeight(layer, 0);
                state = false;

                // Only deactivate flame effect for Grip or Trigger layers
                if (layer != ThumbLayer && handFlameEffect != null)
                {
                    handFlameEffect.SetActive(false);
                }
            }
        }
    }
}