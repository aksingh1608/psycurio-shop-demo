using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shopkeeper : MonoBehaviour, IClickable
{
    private static readonly int WaveTrigger = Animator.StringToHash("Wave");
    private Animator animator;

    [SerializeField] private SpeechBubble speechBubble;

    /// <summary>True while a persistent message (the receipt) is on screen.</summary>
    private bool persistentActive;

    private void Awake() => animator = GetComponent<Animator>();

    private void Start()
    {
        // Welcome the customer when the scene starts
        Wave();
        Speak("Hey there! Welcome to my shop.", 3.5f);
    }

    public void OnClicked()
    {
        Wave();

        // Don't wipe the receipt/question off the screen just for a greeting
        if (!persistentActive)
            Speak("Hey there!", 2f);
    }

    public void Wave() => animator.SetTrigger(WaveTrigger);

    public void Speak(string text, float duration = 4f)
    {
        if (speechBubble == null) return;
        persistentActive = false;
        speechBubble.Show(text, duration);
    }

    public void SpeakPersistent(string text)
    {
        if (speechBubble == null) return;
        persistentActive = true;
        speechBubble.ShowPersistent(text);
    }
}