using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private enum CheckoutState { Shopping, Confirming }

    [SerializeField] private Transform[] counterSlots;
    [SerializeField] private int maxItems = 5;
    [SerializeField] private Shopkeeper shopkeeper;
    [SerializeField] private GameObject checkoutButton;
    [SerializeField] private TextMeshProUGUI checkoutLabel;
    [SerializeField] private GameObject resetButton;

    [Header("Fly Effect")]
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float arcHeight = 0.8f;
    [SerializeField] private ParticleSystem landingParticlesPrefab;
    [SerializeField] private AudioClip whooshClip;
    [SerializeField] private AudioClip popClip;
    [SerializeField] private AudioSource audioSource;

    private readonly List<ItemData> cart = new();
    private readonly List<GameObject> spawnedItems = new();
    private CheckoutState state = CheckoutState.Shopping;

    private void Awake()
    {
        Instance = this;
        UpdateButtons();
    }

    public void TryBuy(ShelfItem shelfItem)
    {
        if (cart.Count >= maxItems)
        {
            shopkeeper.Speak("My counter is full!\nClick an item on the counter to remove it,\nreset, or check out.");
            return;
        }

        Transform slot = counterSlots[cart.Count];
        cart.Add(shelfItem.data);

        GameObject copy = Instantiate(shelfItem.gameObject,
                                      shelfItem.transform.position,
                                      shelfItem.transform.rotation);
        Destroy(copy.GetComponent<ShelfItem>());
        copy.AddComponent<CounterItem>();
        Collider col = copy.GetComponent<Collider>();
        col.enabled = false;
        copy.name = shelfItem.data.itemName + "_OnCounter";
        spawnedItems.Add(copy);
        StartCoroutine(FlyToSlot(copy.transform, slot, col));

        UpdateButtons();
        RefreshSpeech();

        // Counter just became full -> move straight to the question
        if (cart.Count >= maxItems && state == CheckoutState.Shopping)
            BeginCheckout();
    }

    /// <summary>Called by CounterItem when the user clicks an item on the counter.</summary>
    public void RemoveItem(GameObject counterObject)
    {
        int index = spawnedItems.IndexOf(counterObject);
        if (index < 0) return;

        cart.RemoveAt(index);
        spawnedItems.RemoveAt(index);
        Destroy(counterObject);
        if (popClip) audioSource.PlayOneShot(popClip);

        for (int i = 0; i < spawnedItems.Count; i++)
            if (spawnedItems[i] != null)
                spawnedItems[i].transform.SetPositionAndRotation(
                    counterSlots[i].position, counterSlots[i].rotation);

        UpdateButtons();
        RefreshSpeech();
    }

    /// <summary>Register click behaves like the checkout button.</summary>
    public void OnRegisterClicked()
    {
        if (cart.Count == 0)
        {
            shopkeeper.Speak("You haven't picked anything yet!\nClick an item on the shelf.");
            return;
        }
        if (state == CheckoutState.Shopping) BeginCheckout();
    }

    /// <summary>Wired to the checkout UI button (both stages).</summary>
    public void OnCheckoutButtonPressed()
    {
        if (cart.Count == 0) return;

        if (state == CheckoutState.Shopping)
            BeginCheckout();
        else
            CompletePurchase();
    }

    /// <summary>Wired to the Reset UI button.</summary>
    public void ResetCounter()
    {
        if (cart.Count == 0) return;
        ClearCounter();
        shopkeeper.Speak("All cleared!\nStart fresh whenever you like.", 3f);
    }

    private void BeginCheckout()
    {
        state = CheckoutState.Confirming;
        if (checkoutLabel != null) checkoutLabel.text = "Yes, pack it up!";
        shopkeeper.SpeakPersistent(GetReceiptText() + "\nShall I pack everything up for you?");
    }

    private void CompletePurchase()
    {
        string finalText = GetReceiptText() + "\nAll packed! Thank you —\nnext customer, please!";
        ClearCounter();
        shopkeeper.Wave();
        shopkeeper.Speak(finalText, 5f);
    }

    private void ClearCounter()
    {
        foreach (GameObject go in spawnedItems)
            if (go != null) Destroy(go);
        spawnedItems.Clear();
        cart.Clear();
        state = CheckoutState.Shopping;
        if (checkoutLabel != null) checkoutLabel.text = "Checkout";
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool hasItems = cart.Count > 0;
        if (checkoutButton != null) checkoutButton.SetActive(hasItems);
        if (resetButton != null) resetButton.SetActive(hasItems);
    }

    private void RefreshSpeech()
    {
        if (state != CheckoutState.Confirming) return;

        if (cart.Count == 0)
        {
            state = CheckoutState.Shopping;
            if (checkoutLabel != null) checkoutLabel.text = "Checkout";
            shopkeeper.Speak("Your counter is empty again —\npick something from the shelf!", 3f);
            return;
        }
        shopkeeper.SpeakPersistent(GetReceiptText() + "\nShall I pack everything up for you?");
    }

    private IEnumerator FlyToSlot(Transform item, Transform slot, Collider col)
    {
        if (whooshClip) audioSource.PlayOneShot(whooshClip);

        Vector3 start = item.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float eased = Mathf.Clamp01(t);
            eased = eased * eased * (3f - 2f * eased);
            if (item == null) yield break;
            Vector3 pos = Vector3.Lerp(start, slot.position, eased);
            pos.y += arcHeight * 4f * eased * (1f - eased);
            item.position = pos;
            item.Rotate(Vector3.up, 360f * Time.deltaTime);
            yield return null;
        }
        if (item == null) yield break;
        item.SetPositionAndRotation(slot.position, slot.rotation);
        if (col != null) col.enabled = true;

        if (popClip) audioSource.PlayOneShot(popClip);
        if (landingParticlesPrefab)
            Instantiate(landingParticlesPrefab, slot.position, Quaternion.identity);
    }

    private string GetReceiptText()
    {
        float total = 0f;
        var counts = new Dictionary<string, (int count, float price)>();
        foreach (ItemData item in cart)
        {
            total += item.price;
            counts[item.itemName] = counts.TryGetValue(item.itemName, out var c)
                ? (c.count + 1, item.price)
                : (1, item.price);
        }

        var sb = new StringBuilder("Your purchase:\n");
        foreach (var kvp in counts)
        {
            float lineTotal = kvp.Value.count * kvp.Value.price;
            sb.AppendLine(kvp.Value.count > 1
                ? $"{kvp.Value.count}x {kvp.Key} – {lineTotal:F2} €"
                : $"{kvp.Key} – {lineTotal:F2} €");
        }
        sb.Append($"<b>Total: {total:F2} €</b>");
        return sb.ToString();
    }
}