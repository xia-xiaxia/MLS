using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardImageDatabase", menuName = "Card System/Card Image Database")]
public class CardImageDatabase : ScriptableObject
{
    [System.Serializable]
    public class CardImageEntry
    {
        public string cardName; // 直接使用名称如"宫保鸡丁"
        public Sprite image;
    }

    public List<CardImageEntry> imageEntries = new List<CardImageEntry>();

    private Dictionary<string, Sprite> _imageDictionary;

    public void Initialize()
    {
        _imageDictionary = new Dictionary<string, Sprite>();
        foreach (var entry in imageEntries)
        {
            if (!_imageDictionary.ContainsKey(entry.cardName))
            {
                _imageDictionary.Add(entry.cardName, entry.image);
            }
        }
    }

    public Sprite GetSprite(string cardName)
    {
        if (_imageDictionary.TryGetValue(cardName, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogError($"未找到卡牌图片: {cardName}");
            return null; // 或返回默认图片
        }
    }
}