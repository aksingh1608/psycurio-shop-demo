using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShelfItem : MonoBehaviour, IClickable, IHoverInfo
{
    public ItemData data;

    public string HoverText => $"{data.itemName} – {data.price:F2} €";

    public void OnClicked() => ShopManager.Instance.TryBuy(this);
}