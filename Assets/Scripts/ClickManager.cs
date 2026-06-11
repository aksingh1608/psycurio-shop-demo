using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central mouse-click raycaster. Finds IClickable on the hit object
/// (or its parents) and invokes it.
/// </summary>
public class ClickManager : MonoBehaviour
{
    [SerializeField] private float maxDistance = 50f;
    private Camera cam;

    private void Awake() => cam = Camera.main;

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            hit.collider.GetComponentInParent<IClickable>()?.OnClicked();
        }
    }
}