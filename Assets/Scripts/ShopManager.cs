using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private Transform[] counterSlots;
    [SerializeField] private int maxItems = 5;

    [Header("Fly Effect")]
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float arcHeight = 0.8f;
    [SerializeField] private ParticleSystem landingParticlesPrefab;
    [SerializeField] private AudioClip whooshClip;
    [SerializeField] private AudioClip popClip;
    [SerializeField] private AudioSource audioSource;

    private readonly List<ItemData> cart = new();

    private void Awake() => Instance = this;

    public bool CartFull => cart.Count >= maxItems;

    public void TryBuy(ShelfItem shelfItem)
    {
        if (CartFull)
        {
            Debug.Log("Counter is full (5 items max).");
            return;
        }

        Transform slot = counterSlots[cart.Count];
        cart.Add(shelfItem.data);

        GameObject copy = Instantiate(shelfItem.gameObject,
                                      shelfItem.transform.position,
                                      shelfItem.transform.rotation);
        Destroy(copy.GetComponent<ShelfItem>());
        Destroy(copy.GetComponent<Collider>());
        copy.name = shelfItem.data.itemName + "_OnCounter";
        StartCoroutine(FlyToSlot(copy.transform, slot));
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
            eased = eased * eased * (3f - 2f * eased);          // smoothstep
            Vector3 pos = Vector3.Lerp(start, slot.position, eased);
            pos.y += arcHeight * 4f * eased * (1f - eased);     // parabolic arc
            item.position = pos;
            item.Rotate(Vector3.up, 360f * Time.deltaTime);
            yield return null;
        }
        item.SetPositionAndRotation(slot.position, slot.rotation);

        if (popClip) audioSource.PlayOneShot(popClip);
        if (landingParticlesPrefab)
            Instantiate(landingParticlesPrefab, slot.position, Quaternion.identity);
    }

    public string GetReceiptText()
    {
        if (cart.Count == 0)
            return "You haven't picked anything yet!";

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