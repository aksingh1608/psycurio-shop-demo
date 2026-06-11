using UnityEngine;

/// <summary>Grows the object slightly while the mouse hovers it — click affordance.</summary>
public class HoverHighlight : MonoBehaviour
{
    [SerializeField] private float hoverScale = 1.15f;
    private Vector3 baseScale;

    private void Awake() => baseScale = transform.localScale;

    public void SetHovered(bool on)
    {
        transform.localScale = on ? baseScale * hoverScale : baseScale;
    }
}