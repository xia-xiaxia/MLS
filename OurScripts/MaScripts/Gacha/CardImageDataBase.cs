using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardImageDatabase", menuName = "Card System/Card Image Database")]
public class CardImageDatabase : ScriptableObject
{
    [System.Serializable]
    public class CardImageEntry
    {
        public string cardName; // ֱ��ʹ������"��������"
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
            Debug.LogError($"δ�ҵ�����ͼƬ: {cardName}");
            return null;
        }
    }
}