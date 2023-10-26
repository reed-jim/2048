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

    [SerializeField] private ScriptableStringEvent onProductPurchasedEvent;

    private void Awake()
    {
        InitializeIAP();
    }

    public void InitializeIAP()
    {
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct("remove_ad", ProductType.NonConsumable);
        builder.AddProduct("gem200", ProductType.Consumable);
        builder.AddProduct("gem1000", ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
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
        Debug.Log("purchased failed: " + failureDescription.message);
    }

    public void BuyProducts(string productId)
    {
        _currentProcessProductId = productId;
        
        controller.InitiatePurchase(productId);
    }
}