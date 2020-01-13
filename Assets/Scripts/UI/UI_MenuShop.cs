using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_MenuShop : MonoBehaviour
{
    public ScrollRect ShopItemScrollView;
    public Transform ItemSelectCheckPanelTransform;
    public Text titleText;
    public Text playerGoldCubeText;
    Transform shopItemParentTransform;
    public GameObject slotShopItemPrefab;
    Dictionary<int, Transform> shopItemSlotList;
    List<Item> shopItemList;
    int selectItem;
    int buyFailCode;
    //슬롯 변수
    Text itemNameText;
    Text itemValueText;
    Image itemImage;

    private void Awake()
    {
        shopItemParentTransform = ShopItemScrollView.GetComponentInChildren<ContentSizeFitter>().transform;
    }

    private void OnEnable()
    {
        RefreshUI();
    }
    private void OnDisable()
    {
        if(selectItem>0)
            shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(false);
    }

    void RefreshUI()
    {
        ItemSelectCheckPanelTransform.gameObject.SetActive(true);
        if (selectItem>0)
            shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(false);
        titleText.color = PushCubeColor.ThemeMainColor;
        foreach(var txt in ItemSelectCheckPanelTransform.GetComponentsInChildren<Text>())
        {
            txt.color = PushCubeColor.ThemeMainColor;
        }
        ItemSelectCheckPanelTransform.gameObject.SetActive(false);
        playerGoldCubeText.color = Color.yellow;
        RefreshShopItemUI();
    }
    void RefreshShopItemUI()
    {
        playerGoldCubeText.text = PlayerData.goldCube.ToString();
        foreach (Transform child in shopItemParentTransform)
        {
            Destroy(child.gameObject);
        }
        shopItemList = ItemManager.Instance.GetShopItemList();
        shopItemSlotList = new Dictionary<int, Transform>();
        for (int i = 0; i < shopItemList.Count; i++)
        {
            GameObject slot = Instantiate(slotShopItemPrefab, shopItemParentTransform);
            slot.GetComponentInChildren<Text>().text = (i + 1).ToString();
            int slotIndex = i;
            slot.transform.GetChild(1).GetComponent<Text>().color = PushCubeColor.ThemeMainColor;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnSelectItem(shopItemList[slotIndex], slotIndex+1);
            });
            itemNameText = slot.transform.GetChild(1).GetComponent<Text>();
            itemValueText = slot.transform.GetChild(2).GetComponentInChildren<Text>();
            itemImage = slot.transform.GetChild(0).GetComponent<Image>();

            itemNameText.text = LocalizationManager.GetText("ItemName" + shopItemList[slotIndex].number);
            itemValueText.text = shopItemList[slotIndex].value.ToString();
            itemImage.sprite = shopItemList[slotIndex].sprite != null ? shopItemList[slotIndex].sprite : itemImage.sprite;
            if (shopItemList[slotIndex].prefab != null)
            {
                GameObject itemPrefab = Instantiate(shopItemList[slotIndex].prefab, itemImage.transform);
                itemPrefab.transform.localScale = new Vector3(200, 200, 200);
                itemPrefab.transform.localRotation = Quaternion.Euler(new Vector3(-20, 45, -20));
                itemPrefab.transform.localPosition = new Vector3(0, 0, -100);
                itemPrefab.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                itemPrefab.SetActive(true);
            }
            slot.SetActive(true);
            shopItemSlotList.Add(i + 1, slot.transform);
        }
    }

    void OnSelectItem(Item item, int keyIndex)
    {
        if (selectItem > 0)
        {
            shopItemSlotList[selectItem].GetComponent<Image>().color = Color.black;
            shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(false);
            selectItem = 0;
        }
        if (IsBuyAbleItem(item))
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            selectItem = keyIndex;
            ItemSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().text = string.Format("{0} {1}", LocalizationManager.GetText("ItemName" + item.number),LocalizationManager.GetText("Alert_BuyCheck"));
            ItemSelectCheckPanelTransform.GetChild(1).gameObject.SetActive(true);
            ItemSelectCheckPanelTransform.gameObject.SetActive(true);
            shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuMiss);
            if(buyFailCode==1)
            {
                ItemSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().text = string.Format("{0}",LocalizationManager.GetText("Alert_BuyLackGoldcube"));
            }
            else if(buyFailCode==2)
            {
                ItemSelectCheckPanelTransform.GetChild(0).GetComponent<Text>().text = string.Format("<color='red'>{0} {1}</color>",item.number*6,LocalizationManager.GetText("Alert_NeedStage"));
            }

            ItemSelectCheckPanelTransform.GetChild(1).gameObject.SetActive(false);
            ItemSelectCheckPanelTransform.gameObject.SetActive(true);
        }
    }
    public void OnClickSelectItemYes()
    {
        if (selectItem > 0)
        {
            SoundManager.PlaySound(SoundManager.Sound.AlertYes);
            Item item = shopItemList[selectItem-1];
            if (PlayerData.goldCube >= item.value)
            {
                PlayerData.UseGoldCube(item.value);
                if(item.type==0)
                {
                    ItemManager.Instance.ItemUseableBuy(item);
                    GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("`{0}`{1}", LocalizationManager.GetText("ItemName" + item.number), LocalizationManager.GetText("Alert_BuySuccess")), item.sprite, null, null));
                }
                else if(item.type==1)
                {
                    PlayerData.AddCharacter(item.number);
                    GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("`{0}`{1}", LocalizationManager.GetText("ItemName"+item.number),LocalizationManager.GetText("Alert_BuySuccess")), null, item.prefab, null));
                }
                ItemSelectCheckPanelTransform.gameObject.SetActive(false);
                shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(false);
                RefreshShopItemUI();

            }
            selectItem = 0;
        }
    }
    public void OnClickSelectItemNo()
    {
        SoundManager.PlaySound(SoundManager.Sound.MenuClick);
        ItemSelectCheckPanelTransform.gameObject.SetActive(false);
        shopItemSlotList[selectItem].GetChild(3).gameObject.SetActive(false);
        selectItem = 0;
        RefreshShopItemUI();
    }

    bool IsBuyAbleItem(Item item)
    {
        if (item.type == 1 && item.number * 6 + 1 > PlayerData.currentStage)
        {
            buyFailCode = 2;
            return false;
        }
        else if (item.value > PlayerData.goldCube)
        {
            buyFailCode = 1;
            return false;
        }
        buyFailCode = 0;
        return true;
    }
}
