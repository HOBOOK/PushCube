using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;
	static IStoreController storeController = null;
	static string[] sProductIds;
    public enum StoreType { GoogleStore,OneStore};
    public StoreType storeType;
	void Awake()
	{
        Instance = this;
        sProductIds = new string[] { "remove_ad", "goldencube_10" };
	}

	public void InitStore()
	{
        if(storeController==null)
        {
            if (storeType == StoreType.GoogleStore)
            {
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                builder.AddProduct(sProductIds[0], ProductType.Consumable, new IDs { { sProductIds[0], GooglePlay.Name } });
                builder.AddProduct(sProductIds[1], ProductType.Consumable, new IDs { { sProductIds[1], GooglePlay.Name } });
                UnityPurchasing.Initialize(this, builder);
            }
            // else if (storeType == StoreType.OneStore)
            // {
            //     string base64EncodedPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCRrFYKKco7ZhEDiS+/xsOih0juoHpwRzTr3gwBKCO2N3jgQcg6iAuI4I9R7HvslBfPOFYTrnD6aT5XrnR1XMxSGMLKUOtFyAD74BUFPonMcGlO12imMhUaaUlxuk+DnkPo1KsNjDamvyO2/oYA8dhkkACYWR2zdU34F48U4NWDSQIDAQAB";
            //     Onestore_IapCallManager.connectService(base64EncodedPublicKey);
            // }
        }
	}

    public void Purchase(int index)
    {
        if (storeType == StoreType.GoogleStore)
        {
            if (storeController == null)
            {
                Debug.Log("구매 실패 : 결제 기능 초기화 실패");
            }
            else
                storeController.InitiatePurchase(sProductIds[index]);
        }
        // else
        // {
        //     Onestore_IapCallManager.buyProduct(sProductIds[index], "inapp", "this is test payload!");
        // }
    }

    public void Restore()
    {
        if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
                        Application.platform == RuntimePlatform.WSAPlayerX64 ||
                        Application.platform == RuntimePlatform.WSAPlayerARM)
        {
            CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>()
                .RestoreTransactions();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.tvOS)
        {
            CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>()
                .RestoreTransactions(OnTransactionsRestored);
        }
        else if (Application.platform == RuntimePlatform.Android &&
                 StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
        {
            CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<ISamsungAppsExtensions>()
                .RestoreTransactions(OnTransactionsRestored);
        }
        else if (Application.platform == RuntimePlatform.Android &&
                 StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
        {
            CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMoolahExtension>()
                .RestoreTransactionID((restoreTransactionIDState) =>
                {
                    OnTransactionsRestored(
                        restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed &&
                        restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
                });
        }
        else
        {
            Debug.LogWarning(Application.platform.ToString() +
                             " is not a supported platform for the Codeless IAP restore button");
        }
    }

    void OnTransactionsRestored(bool success)
    {
        GameSystemManager.ShowAlertMessage(new AlertMessage(1, success.ToString(), null, null, null));
        Debug.Log("Transactions restored: " + success);
    }

    #region GoogleStore 함수
    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
        storeController = controller;
        if (PlayerPrefs.HasKey("RestoreIAP"))
        {

        }
        else
        {
            Debug.Log("재설치 복원시작");
            bool check = PlayerData.isRemoveAd;
            extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
                if (result)
                {
                    if(check!=PlayerData.isRemoveAd)
                    {
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, LocalizationManager.GetText("Alert_RestoreIAPSuccess"), null, null, null));
                    }
                }
                else
                {
                    // Restoration failed.
                }
            });
            PlayerPrefs.SetInt("RestoreIAP", 1);
        }
		Debug.Log("결제 기능 초기화");
	}

	void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed" + error);
    }

	PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
	{
		bool isSuccess = true;
#if UNITY_ANDROID && !UNITY_EDITOR
		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
		try
		{
			IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
			for(int i = 0; i < result.Length; i++)
				Analytics.Transaction(result[i].productID, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode, result[i].transactionID, null);
		}
		catch (IAPSecurityException)
		{
			isSuccess = false;
		}
#endif
		if (isSuccess)
		{
            if (e.purchasedProduct.definition.id.Equals(sProductIds[0]))
                PurchaseRemoveAd();
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[1]))
                PurchaseGoldCube();
		}
		else
		{
            Debug.Log("구매 실패 : 비정상 결제");
		}

		return PurchaseProcessingResult.Complete;
	}

	void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
	{
		if (!error.Equals(PurchaseFailureReason.UserCancelled))
		{
			Debug.Log("구매 실패 : " + error);
		}
	}
    #endregion

    public int ProductIdCheck(string id)
    {
        if (id.Equals(sProductIds[0]))
            return 0;
        else if (id.EndsWith(sProductIds[1]))
            return 1;
        else
            return -1;
    }

    public void PurchaseRemoveAd()
    {
        PlayerData.isRemoveAd = true;
        PlayerDataSystem.Save();
        PlayerData.AddCharacter(16);
        AdMobManager.Instance.HideBanner();
        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_BuyRemoveAdYes")), null, GameAssetsManager.instance.CharacterAssets[16].asset, null));
        StartManager.Instance.AddCharToGrid(16);
        Debug.Log("광고 구매완료");
    }

    public void PurchaseGoldCube()
    {
        PlayerData.AddGoldCube(10);
        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_BuyGoldcubeYes")), GameAssetsManager.instance.SpriteAssets[0].sprite, null, null));
        Debug.Log("황금 큐브 구매완료");
    }
}

