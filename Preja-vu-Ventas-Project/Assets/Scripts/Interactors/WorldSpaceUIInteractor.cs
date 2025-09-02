using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WorldSpaceUIInteractor : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 10f;
    public LayerMask interactableLayer;

    private Button currentButton = null;

    void Start()
    {
        if (!playerCamera)
        {
            playerCamera = Camera.main;
        }

        if (!playerCamera)
        {
            Debug.LogError("Camera not assigned.");
            enabled = false;
        }
    }

    void Update()
    {
        UpdateButtonUnderCursor();
        UpdateButtonVisual();
        HandleClick();
    }

    void UpdateButtonUnderCursor()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        int mask = (interactableLayer.value == 0) ? ~0 : interactableLayer.value;

        if (Physics.Raycast(ray, out hit, interactionDistance, mask))
        {
            var btn = hit.collider.GetComponent<Button>() ?? hit.collider.GetComponentInParent<Button>();
            if (btn != currentButton)
            {
                if (currentButton != null)
                    SetButtonColor(currentButton, currentButton.colors.normalColor);

                currentButton = btn;

                if (currentButton != null && currentButton.interactable)
                    SetButtonColor(currentButton, currentButton.colors.highlightedColor);
            }
        }
        else
        {
            if (currentButton != null)
            {
                SetButtonColor(currentButton, currentButton.colors.normalColor);
                currentButton = null;
            }
        }
    }

    void UpdateButtonVisual()
    {
        if (currentButton == null || !currentButton.interactable) return;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            SetButtonColor(currentButton, currentButton.colors.pressedColor);
        }
        else
        {
            SetButtonColor(currentButton, currentButton.colors.highlightedColor);
        }
    }

    void HandleClick()
    {
        if (currentButton == null || !currentButton.interactable) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentButton.onClick.Invoke();
        }
    }

    void SetButtonColor(Button btn, Color color)
    {
        var image = btn.targetGraphic as Image;
        if (image != null)
            image.color = color;
    }
}
