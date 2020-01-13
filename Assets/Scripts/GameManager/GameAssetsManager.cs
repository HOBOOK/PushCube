using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameAssetsManager : MonoBehaviour
{
    public static GameAssetsManager instance;
    private void Awake()
    {
        instance = this;
        CharacterAssets = new List<CharacterAssets>();
        Transform[] charPrefabs = Resources.LoadAll<Transform>("CharacterData");
        for(int i = 0; i < charPrefabs.Length; i++)
        {
            CharacterAssets.Add(new CharacterAssets(i, charPrefabs[i].gameObject));
        }
    }
    public SoundAudioClip[] SoundAudioClips;
    public List<CharacterAssets> CharacterAssets = new List<CharacterAssets>();
    public List<SpriteAssets> SpriteAssets = new List<SpriteAssets>();
    public List<Transform> GetCharacterAssetsTransformList()
    {
        List<Transform> charList = new List<Transform>();
        foreach(var i in CharacterAssets)
        {
            charList.Add(i.asset.transform);
        }
        return charList;
    }
    public Transform GetCurrentCharacterAsset()
    {
        return CharacterAssets[PlayerData.currentCharacter].asset.transform;
    }
}

[System.Serializable]
public struct CharacterAssets
{
    public int id;
    public GameObject asset;
    public CharacterAssets(int nId, GameObject nAsset)
    {
        id = nId;
        asset = nAsset;
    }
}

[System.Serializable]
public struct SpriteAssets
{
    public string name;
    public Sprite sprite;
    public SpriteAssets(string nName, Sprite nSprite)
    {
        name = nName;
        sprite = nSprite;
    }
}
