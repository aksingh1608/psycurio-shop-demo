using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CashRegister : MonoBehaviour, IClickable
{
    public void OnClicked()
    {
        ShopManager.Instance.Checkout();
    }
}