using UnityEngine;

/// <summary>An item placed on the counter; click to remove it from the cart.</summary>
public class CounterItem : MonoBehaviour, IClickable
{
    public void OnClicked()
    {
        ShopManager.Instance.RemoveItem(gameObject);
    }
}