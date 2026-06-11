using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private float maxDistance = 50f;
    private Camera cam;
    private HoverHighlight hovered;

    private void Awake() => cam = Camera.main;

    private void Update()
    {
        if (Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, maxDistance);

        HoverHighlight newHover = hitSomething
            ? hit.collider.GetComponentInParent<HoverHighlight>()
            : null;
        if (newHover != hovered)
        {
            if (hovered != null) hovered.SetHovered(false);
            hovered = newHover;
            if (hovered != null) hovered.SetHovered(true);
        }

        if (hitSomething && Mouse.current.leftButton.wasPressedThisFrame)
            hit.collider.GetComponentInParent<IClickable>()?.OnClicked();
    }
}