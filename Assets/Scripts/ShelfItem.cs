using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShelfItem : MonoBehaviour, IClickable
{
    public ItemData data;

    public void OnClicked()
    {
        ShopManager.Instance.TryBuy(this);
    }
}