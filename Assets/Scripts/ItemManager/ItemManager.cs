using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    private List<Item> itemList;
    private void Awake()
    {
        Instance = this;

    }
    public void LoadItemData()
    {
        itemList = new List<Item>();
        LoadUseItemData();
        LoadCharacterData();

    }
    void LoadUseItemData()
    {
        int id;
        for (int i = 0; i < 1; i++)
        {
            id = 101 + i;
            itemList.Add(new Item(id, 1, 0, Resources.Load<Sprite>("ItemResources/"+ id), null));
        }
    }
    void LoadCharacterData()
    {
        for (int i = 2; i < GameAssetsManager.instance.CharacterAssets.Count; i++)
        {
            if(!PlayerData.IsHavingCharacter(i))
                itemList.Add(new Item(i, 5, 1, null, GameAssetsManager.instance.CharacterAssets[i].asset));
        }
    }
    public List<Item> GetItemList()
    {
        if (itemList == null)
            LoadItemData();
        return itemList;
    }
    public List<Item> GetShopItemList()
    {
        if (itemList == null)
            LoadItemData();
        List<Item> shopList = itemList.FindAll(x => x.type == 0 || (x.type == 1 && !IsSpecialCharacter(x.number) && !PlayerData.IsHavingCharacter(x.number)));
        return shopList;
    }

    public void ItemUseableBuy(Item item)
    {
        switch(item.number)
        {
            case 101:
                PlayerData.AddUndoCount(30);
                GameManager.instance.UpdateUI();
                break;
        }
    }

    public bool IsSpecialCharacter(int number)
    {
        int[] speicalCharacterId = { 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        return speicalCharacterId.Contains(number);
    }
}

[System.Serializable]
public struct Item
{
    public int number;
    public int value;
    public int type; // 0:  사용 1: 캐릭터
    public Sprite sprite;
    public GameObject prefab;
    public Item(int nNumber, int nValue, int nType, Sprite nSprite, GameObject nPrefab)
    {
        number = nNumber;
        value = nValue;
        type = nType;
        sprite = nSprite;
        prefab = nPrefab;
    }
}
