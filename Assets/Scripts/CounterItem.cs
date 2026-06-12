using UnityEngine;

public class CounterItem : MonoBehaviour, IClickable, IHoverInfo
{
    private ItemData data;

    public void Init(ItemData itemData) => data = itemData;

    public string HoverText =>
        data != null ? $"{data.itemName} – click to remove" : "Click to remove";

    public void OnClicked() => ShopManager.Instance.RemoveItem(gameObject);
}