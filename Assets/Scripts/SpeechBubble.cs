using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private Coroutine hideRoutine;
    private Camera cam;

    private void Awake() => cam = Camera.main;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    public void Show(string text, float duration)
    {
        SetText(text);
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfter(duration));
    }

    /// <summary>Shows text that stays until Hide() or another Show replaces it.</summary>
    public void ShowPersistent(string text)
    {
        SetText(text);
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = null;
    }

    public void Hide() => gameObject.SetActive(false);

    private void SetText(string text)
    {
        gameObject.SetActive(true);
        label.text = text;
    }

    private IEnumerator HideAfter(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
    }
}