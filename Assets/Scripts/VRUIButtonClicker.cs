using UnityEngine;
using UnityEngine.UI;
using WebXR;

public class VRUIButtonClicker : MonoBehaviour
{
    public WebXRController rightController; // Assign in Unity
    public LayerMask uiLayerMask; // Set to "UI"

    private void Update()
    {
        if (rightController.GetButtonDown(WebXRController.ButtonTypes.Trigger))
        {
            TryClickButton();
        }
    }

    private void TryClickButton()
    {
        RaycastHit hit;
        Ray ray = new Ray(rightController.transform.position, rightController.transform.forward);

        if (Physics.Raycast(ray, out hit, 10f, uiLayerMask))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
                Debug.Log("Button Clicked!");
            }
        }
    }
}
