using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    private string _currentProcessProductId;

    [Header("EVENT")]
    [SerializeField] private ScriptableStringEvent onProductPurchasedEvent;

    [Header("REFERENCE")]
    [SerializeField] private DataManager dataManager;

    private void Awake()
    {
        InitializeIAP();
    }

    public void InitializeIAP()
    {
#if UNITY_EDITOR
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct("remove_ad", ProductType.NonConsumable);
        builder.AddProduct("gem500", ProductType.Consumable);
        builder.AddProduct("gem1000", ProductType.Consumable);
        builder.AddProduct("gem5000", ProductType.Consumable);
        builder.AddProduct("gem20000", ProductType.Consumable);
        builder.AddProduct("gem50000", ProductType.Consumable);
        builder.AddProduct("gem100000", ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;

        OnProductFetched(controller.products.all);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        onProductPurchasedEvent.Raise(_currentProcessProductId);

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {

    }

    public void BuyProducts(string productId)
    {
        _currentProcessProductId = productId;

        controller.InitiatePurchase(productId);
    }

    private void OnProductFetched(Product[] products)
    {
        Product removeAdProduct = products[0];

        if (IsUserAlreadyHaveAdRemove(removeAdProduct))
        {
            dataManager.IsAdRemoved = true;
            dataManager.SaveIAPData();
        }
    }

    private bool IsUserAlreadyHaveAdRemove(Product product)
    {
        if (product.hasReceipt)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}