using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private Transform[] counterSlots;
    [SerializeField] private int maxItems = 5;

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

        GameObject copy = Instantiate(shelfItem.gameObject, slot.position, slot.rotation);
        Destroy(copy.GetComponent<ShelfItem>());
        copy.name = shelfItem.data.itemName + "_OnCounter";
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