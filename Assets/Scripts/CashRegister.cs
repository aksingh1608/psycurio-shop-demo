using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CashRegister : MonoBehaviour, IClickable
{
    [SerializeField] private Shopkeeper shopkeeper;

    public void OnClicked()
    {
        shopkeeper.Speak(ShopManager.Instance.GetReceiptText());
    }
}