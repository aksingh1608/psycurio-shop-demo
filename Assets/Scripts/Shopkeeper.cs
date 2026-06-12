using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shopkeeper : MonoBehaviour, IClickable
{
    private static readonly int WaveTrigger = Animator.StringToHash("Wave");
    private Animator animator;

    [SerializeField] private SpeechBubble speechBubble;

    private void Awake() => animator = GetComponent<Animator>();

    public void OnClicked() => Wave();

    public void Wave() => animator.SetTrigger(WaveTrigger);

    public void Speak(string text, float duration = 4f)
    {
        if (speechBubble != null) speechBubble.Show(text, duration);
    }

    public void SpeakPersistent(string text)
    {
        if (speechBubble != null) speechBubble.ShowPersistent(text);
    }
}