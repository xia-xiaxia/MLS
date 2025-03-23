using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Card
{
    public string name;
    public Rarity rarity;
    public CardType type;

    public Card(string name, Rarity rarity, CardType type)
    {
        this.name = name;
        this.rarity = rarity;
        this.type = type;
    }
}
static class ButtonExtension
{
    public static void ButtonClick
        (
          this Button quickPurchaseButton,
               Button bigPurchaseButton,
               Button closeButton,
               Button confirmButton
        )
    {
        Debug.Log("快速采购按钮被点击");
    }
}

// 抽卡系统 快速采购
public class QuickPurchase : MonoBehaviour
{
    private HashSet<string> collectedCardIDs = new HashSet<string>(); // id池，new有关
    private System.Random random = new System.Random();
    private List<Card> drawnCards = new List<Card>();
    public bool hasSelectedMode = false;
    //public float drawCooldown = 2f; 
    //private float lastDrawTime = 0f; 
    private int consecutiveDrawsWithoutPurple = 0; // 连续不出紫色的次数（5为界限）
    private int consecutiveDrawsWithoutGold = 0;//不出金
    private bool isBigPurchaseMode = false;
    public Button quickPurchaseButton;
    public Button bigPurchaseButton;
    public Button closeButton;
    public Button confirmPurchaseButton;
    public GameObject uiPanel;
    public HangoutFlow hangoutFlow;
    public CardImageDatabase cardImageDatabase;

    private List<string> recipePool = new List<string> { "宫保鸡丁", "麻婆豆腐", "北京烤鸭", "红烧肉", "鱼香肉丝", "水煮鱼", "糖醋里脊" };
    private List<string> ingredientPool = new List<string> { "鸡肉", "花生", "辣椒", "盐", "大豆", "牛肉", "鸭肉", "猪肉", "胡萝卜", "鱼肉", "鸡蛋", "糖" };
    private List<string> partnerPool = new List<string> { "伙伴1", "伙伴2", "伙伴3", "伙伴4", "伙伴5" };


    // quick权重
    private Dictionary<CardType, Dictionary<Rarity, float>> quickPurchaseRates = new Dictionary<CardType, Dictionary<Rarity, float>>
{
    { CardType.Recipe, new Dictionary<Rarity, float>
        { { Rarity.Green, 15f }, { Rarity.Blue, 4f }, { Rarity.Purple, 0.8f }, { Rarity.Gold, 0.175f }, { Rarity.Rainbow, 0.025f } } },

    { CardType.Ingredient, new Dictionary<Rarity, float>
        { { Rarity.Green, 45f }, { Rarity.Blue, 20f }, { Rarity.Purple, 7f }, { Rarity.Gold, 0.95f }, { Rarity.Rainbow, 0.05f } } },

    { CardType.Partner, new Dictionary<Rarity, float>
        { { Rarity.Green, 6.3f }, { Rarity.Gold, 0.7f } } }
};

    //big权重
    private Dictionary<CardType, Dictionary<Rarity, float>> bigPurchaseRates = new Dictionary<CardType, Dictionary<Rarity, float>>
{
    { CardType.Recipe, new Dictionary<Rarity, float>
        { { Rarity.Green, 10f }, { Rarity.Blue, 7f }, { Rarity.Purple, 2.3f }, { Rarity.Gold, 0.575f }, { Rarity.Rainbow, 0.125f } } },

    { CardType.Ingredient, new Dictionary<Rarity, float>
        { { Rarity.Green, 30f }, { Rarity.Blue, 25f }, { Rarity.Purple, 10f }, { Rarity.Gold, 2.45f }, { Rarity.Rainbow, 0.55f } } },

    { CardType.Partner, new Dictionary<Rarity, float>
        { { Rarity.Blue, 10.3f }, { Rarity.Gold, 1.7f } } }
};
    private Dictionary<string, Sprite> cardImages = new Dictionary<string, Sprite>();

    public Transform upperRowParent;
    public Transform lowerRowParent;
    public GameObject cardPrefab;
    void Start()
    {
        collectedCardIDs.Clear();
        quickPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(false));
        bigPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(true));
        confirmPurchaseButton.onClick.AddListener(ConfirmPurchase);
        closeButton.onClick.AddListener(CloseUI);
        confirmPurchaseButton.gameObject.SetActive(false);
        cardImageDatabase.Initialize();
        //  EnablePurchaseButtons();
    }
    public void DisplayCards(List<Card> cards)
    {
        //clear
        foreach (Transform child in upperRowParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lowerRowParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            Transform parent = i < 5 ? upperRowParent : lowerRowParent;

            GameObject cardUI = Instantiate(cardPrefab, parent);
            Image cardImage = cardUI.transform.Find("cardImage").GetComponent<Image>();
            TextMeshProUGUI nameText = cardUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            if (!nameText.enabled)
            {
                nameText.enabled = true;
            }
            //  Debug.Log("NameText: " + nameText.text);
            // TextMeshProUGUI typeText = cardUI.transform.Find("TypeText").GetComponent<TextMeshProUGUI>();
            //  TextMeshProUGUI rarityText = cardUI.transform.Find("RarityText").GetComponent<TextMeshProUGUI>();
            Image newTagImage = cardUI.transform.Find("NewTagImage").GetComponent<Image>();
            Sprite cardSprite = cardImageDatabase.GetSprite(card.name);

            if (cardSprite != null)
            {
                cardImage.sprite = cardSprite;
            }


            nameText.text = card.rarity.ToString();


            Canvas.ForceUpdateCanvases();
            string cardID = card.name + "_" + card.rarity.ToString();
            bool isNew = !collectedCardIDs.Contains(cardID);
            newTagImage.gameObject.SetActive(isNew);
            if (isNew)
            {
                collectedCardIDs.Add(cardID);  //new完加到id池中，id是配方三-绿这种
            }
        }
    }

    public void DisablePurchaseButtons()
    {
        quickPurchaseButton.gameObject.SetActive(false);
        bigPurchaseButton.gameObject.SetActive(false);
        confirmPurchaseButton.gameObject.SetActive(false);
    }
    private void SelectPurchaseMode(bool isBigPurchase)
    {
        isBigPurchaseMode = isBigPurchase;
        confirmPurchaseButton.gameObject.SetActive(true); // 选择模式后显示确认按钮

    }
    private void ConfirmPurchase()
    {
        DisablePurchaseButtons();
        confirmPurchaseButton.gameObject.SetActive(false);
        drawnCards.Clear();
        List<Card> newCards = DrawCards(10, isBigPurchaseMode ? bigPurchaseRates : quickPurchaseRates, isBigPurchaseMode);
        ProcessDrawnCards(newCards);

        uiPanel.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }
    private List<Card> DrawCards(int count, Dictionary<CardType, Dictionary<Rarity, float>> purchaseRates, bool isBigPurchase)
    {
        List<Card> cards = new List<Card>();
        bool hasRequiredRarity = false;

        for (int i = 0; i < count; i++)
        {
            Card card = GetRandomCard(purchaseRates, isBigPurchase);
            cards.Add(card);

            if (!isBigPurchase && card.rarity >= Rarity.Blue) hasRequiredRarity = true;
            if (isBigPurchase && card.rarity >= Rarity.Purple) hasRequiredRarity = true;
        }

        // quick 必有蓝
        if (!isBigPurchase && !hasRequiredRarity)
        {
            cards[random.Next(cards.Count)] = GetGuaranteedCard(Rarity.Blue, purchaseRates);
        }

        // big  必有紫
        if (isBigPurchase && !hasRequiredRarity)
        {
            cards[random.Next(cards.Count)] = GetGuaranteedCard(Rarity.Purple, purchaseRates);
        }

        // 大保底
        if (isBigPurchase)//bigpurchase下的50次保底
        {
            if (consecutiveDrawsWithoutGold >= 50)
            {
                cards[cards.Count - 1] = GetGuaranteedCard(Rarity.Gold, purchaseRates);
                consecutiveDrawsWithoutGold = 0;
            }
            else if (!hasRequiredRarity)
            {
                consecutiveDrawsWithoutGold++;
            }
        }
        else//quickpurchase下的50次大保底
        {
            if (consecutiveDrawsWithoutPurple >= 50)
            {
                cards[cards.Count - 1] = GetGuaranteedCard(Rarity.Purple, purchaseRates);
                consecutiveDrawsWithoutPurple = 0;
            }
            else if (!hasRequiredRarity)
            {
                consecutiveDrawsWithoutPurple++;
            }
        }

        return cards;
    }


    private Card GetRandomCard(Dictionary<CardType, Dictionary<Rarity, float>> purchaseRates, bool isBigPurchase)
    {
        float randomTypeValue = (float)(random.NextDouble() * 100);
        CardType selectedType = isBigPurchase ? RandomlySelectBigPurchaseType(randomTypeValue) : RandomlySelectQuickPurchaseType(randomTypeValue);

        float randomRarityValue = (float)(random.NextDouble() * 100);
        Rarity selectedRarity = RandomlySelectRarity(selectedType, randomRarityValue, purchaseRates);

        string selectedName = GetRandomNameForType(selectedType);

        return new Card(selectedName, selectedRarity, selectedType);
    }

    //用随机数对应不同种类，quickpurchase配方0-20，食材20-93，伙伴93-100，总共产生1-100的随机数
    //bigpurchase配方0-30，食材30-85，伙伴85-100
    //然后再用种类中不同稀有度不同权重来抽取不同稀有度

    private CardType RandomlySelectQuickPurchaseType(float randomValue)
    {
        if (randomValue < 20) return CardType.Recipe;
        else if (randomValue < 93) return CardType.Ingredient;
        else return CardType.Partner;
    }
    private CardType RandomlySelectBigPurchaseType(float randomValue)
    {
        if (randomValue < 30) return CardType.Recipe;
        else if (randomValue < 85) return CardType.Ingredient;
        else return CardType.Partner;
    }

    private Rarity RandomlySelectRarity(CardType type, float randomValue, Dictionary<CardType, Dictionary<Rarity, float>> rates)
    {
        float cumulative = 0f;
        foreach (var rarity in rates[type])
        {
            cumulative += rarity.Value;
            if (randomValue <= cumulative)
                return rarity.Key;
        }
        return Rarity.Green;
    }
    //随机random从池中挑选name
    private string GetRandomNameForType(CardType type)
    {
        switch (type)
        {
            case CardType.Recipe: return recipePool[random.Next(recipePool.Count)];
            case CardType.Ingredient: return ingredientPool[random.Next(ingredientPool.Count)];
            case CardType.Partner: return partnerPool[random.Next(partnerPool.Count)];
            default: return "未知";
        }
    }
    private Card GetGuaranteedCard(Rarity rarity, Dictionary<CardType, Dictionary<Rarity, float>> purchaseRates)
    {
        Card card;
        do
        {
            card = GetRandomCard(purchaseRates, isBigPurchaseMode);
        } while (card.rarity < rarity);
        return card;
    }

    private void ProcessDrawnCards(List<Card> drawnCards)
    {

        foreach (var card in drawnCards)
        {
            Debug.Log($"抽到: {card.name} - {card.rarity}");
            if (card.type == CardType.Recipe)
                ConvertRecipeToResource(card);
            else if (card.type == CardType.Ingredient)
                ConvertIngredientToResource(card);
            else if (card.type == CardType.Partner)
                ConvertPartnerToResource(card);
        }
        DisplayCards(drawnCards);
    }

    private void ConvertRecipeToResource(Card card)
    {
        int recipeExperience = 0;
        switch (card.rarity)
        {
            case Rarity.Green: recipeExperience = 1; break;
            case Rarity.Blue: recipeExperience = 2; break;
            case Rarity.Purple: recipeExperience = 3; break;
            case Rarity.Gold: recipeExperience = 5; break;
            case Rarity.Rainbow: recipeExperience = 10; break;
        }
        GameDataManager.Instance.AddRecipeExperience(recipeExperience);
        Debug.Log($"配方 {card.name} 转换为 {recipeExperience} 配方经验");

    }
    private void ConvertIngredientToResource(Card card)
    {
        int ingredientFragments = 0;
        switch (card.rarity)
        {
            case Rarity.Green: ingredientFragments = 1; break;
            case Rarity.Blue: ingredientFragments = 2; break;
            case Rarity.Purple: ingredientFragments = 4; break;
            case Rarity.Gold: ingredientFragments = 8; break;
            case Rarity.Rainbow: ingredientFragments = 15; break;
        }
        GameDataManager.Instance.AddIngredientExperience(ingredientFragments);
        Debug.Log($"食材 {card.name} 转换为 {ingredientFragments} 食材碎片");

    }

    private void ConvertPartnerToResource(Card card)
    {
        if (card.rarity == Rarity.Gold)
        {
            // 金色，保存完整伙伴，存到伙伴池中
            GameDataManager.Instance.SaveFullPartner(card.name);
            Debug.Log($"保存完整伙伴 {card.name}");
        }
        else
        {
            // 绿色伙伴，增加一个伙伴碎片
            GameDataManager.Instance.AddPartnerFragments(1);
            Debug.Log("伙伴碎片累加");
        }
    }
    private void CloseUI()
    {
        uiPanel.SetActive(false);
        foreach (Transform child in upperRowParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lowerRowParent)
        {
            Destroy(child.gameObject);
        }

        DisablePurchaseButtons();
        collectedCardIDs.Clear();
    }
}
