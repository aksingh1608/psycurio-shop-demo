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
        gameObject.SetActive(true);
        label.text = text;
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfter(duration));
    }

    private IEnumerator HideAfter(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
    }
}