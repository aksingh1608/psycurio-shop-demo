using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Small info card that follows the mouse while hovering items.</summary>
public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        if (panel.gameObject.activeSelf && Mouse.current != null)
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            panel.position = mouse + new Vector2(18f, -12f);
        }
    }

    public void Show(string text)
    {
        label.text = text;
        panel.gameObject.SetActive(true);
    }

    public void Hide() => panel.gameObject.SetActive(false);
}