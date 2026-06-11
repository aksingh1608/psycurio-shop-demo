using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shopkeeper : MonoBehaviour, IClickable
{
    private static readonly int WaveTrigger = Animator.StringToHash("Wave");
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    public void OnClicked()
    {
        animator.SetTrigger(WaveTrigger);
    }
}