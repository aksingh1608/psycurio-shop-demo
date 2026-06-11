using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private Transform[] counterSlots;
    [SerializeField] private int maxItems = 5;
    [SerializeField] private Shopkeeper shopkeeper;

    [Header("Fly Effect")]
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float arcHeight = 0.8f;
    [SerializeField] private ParticleSystem landingParticlesPrefab;
    [SerializeField] private AudioClip whooshClip;
    [SerializeField] private AudioClip popClip;
    [SerializeField] private AudioSource audioSource;

    private readonly List<ItemData> cart = new();
    private readonly List<GameObject> spawnedItems = new();
    private bool clearing;

    private void Awake() => Instance = this;

    public bool CartFull => cart.Count >= maxItems;

    public void TryBuy(ShelfItem shelfItem)
    {
        if (clearing) return;

        if (CartFull)
        {
            shopkeeper.Speak("My counter is full!\nClick the register to check out.");
            return;
        }

        Transform slot = counterSlots[cart.Count];
        cart.Add(shelfItem.data);

        GameObject copy = Instantiate(shelfItem.gameObject,
                                      shelfItem.transform.position,
                                      shelfItem.transform.rotation);
        Destroy(copy.GetComponent<ShelfItem>());
        Destroy(copy.GetComponent<HoverHighlight>());
        Destroy(copy.GetComponent<Collider>());
        copy.name = shelfItem.data.itemName + "_OnCounter";
        spawnedItems.Add(copy);
        StartCoroutine(FlyToSlot(copy.transform, slot));
    }

    public void Checkout()
    {
        if (clearing) return;

        if (cart.Count == 0)
        {
            shopkeeper.Speak("You haven't picked anything yet!\nClick an item on the shelf.");
            return;
        }

        shopkeeper.Speak(GetReceiptText(), 4f);
        StartCoroutine(FinishSale());
    }

    private IEnumerator FinishSale()
    {
        clearing = true;
        yield return new WaitForSeconds(4.5f);

        foreach (GameObject go in spawnedItems)
            if (go != null) Destroy(go);
        spawnedItems.Clear();
        cart.Clear();

        shopkeeper.Speak("Thank you!\nNext customer, please!", 2.5f);
        clearing = false;
    }

    private IEnumerator FlyToSlot(Transform item, Transform slot)
    {
        if (whooshClip) audioSource.PlayOneShot(whooshClip);

        Vector3 start = item.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float eased = Mathf.Clamp01(t);
            eased = eased * eased * (3f - 2f * eased);
            Vector3 pos = Vector3.Lerp(start, slot.position, eased);
            pos.y += arcHeight * 4f * eased * (1f - eased);
            if (item == null) yield break;
            item.position = pos;
            item.Rotate(Vector3.up, 360f * Time.deltaTime);
            yield return null;
        }
        if (item == null) yield break;
        item.SetPositionAndRotation(slot.position, slot.rotation);

        if (popClip) audioSource.PlayOneShot(popClip);
        if (landingParticlesPrefab)
            Instantiate(landingParticlesPrefab, slot.position, Quaternion.identity);
    }

    private string GetReceiptText()
    {
        float total = 0f;
        var counts = new Dictionary<string, int>();
        foreach (ItemData item in cart)
        {
            total += item.price;
            counts[item.itemName] = counts.TryGetValue(item.itemName, out int c) ? c + 1 : 1;
        }

        var sb = new StringBuilder("You chose: ");
        var parts = new List<string>();
        foreach (var kvp in counts)
            parts.Add(kvp.Value > 1 ? $"{kvp.Value}x {kvp.Key}" : kvp.Key);
        sb.Append(string.Join(", ", parts));
        sb.Append($"\nThat will be {total:F2} €, please!");
        return sb.ToString();
    }
}