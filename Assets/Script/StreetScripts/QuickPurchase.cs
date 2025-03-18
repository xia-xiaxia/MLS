using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public enum Rarity
{
    Green, Blue, Purple, Gold, Rainbow
}

public enum CardType
{
    Recipe, Ingredient, Partner
}

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

public class QuickPurchase : MonoBehaviour
{
    private HashSet<string> collectedCardIDs = new HashSet<string>(); // id�أ�new�й�
    private System.Random random = new System.Random();
    private List<Card> drawnCards = new List<Card>(); 
    public bool hasSelectedMode = false; 
    //public float drawCooldown = 2f; 
    //private float lastDrawTime = 0f; 
    private int consecutiveDrawsWithoutPurple = 0; // ����������ɫ�Ĵ�����5Ϊ���ޣ�
    private int consecutiveDrawsWithoutGold = 0;//������
    private bool isBigPurchaseMode = false; 
    public Button quickPurchaseButton;
    public Button bigPurchaseButton;
    public Button closeButton;
    //public Sprite[] rarityBackgrounds; 
    public GameObject uiPanel;
    public HangoutFlow hangoutFlow;
    public CardImageDatabase cardImageDatabase;

    private List<string> recipePool = new List<string> { "��������", "���Ŷ���", "������Ѽ", "������", "������˿" ,"ˮ����","�Ǵ��Ｙ"};
    private List<string> ingredientPool = new List<string> { "����", "����", "����", "��", "��" ,"ţ��","Ѽ��","����","���ܲ�","����","����","��"};
    private List<string> partnerPool = new List<string> { "���1", "���2", "���3", "���4", "���5" };

    
    // quickȨ��
    private Dictionary<CardType, Dictionary<Rarity, float>> quickPurchaseRates = new Dictionary<CardType, Dictionary<Rarity, float>>
{
    { CardType.Recipe, new Dictionary<Rarity, float>
        { { Rarity.Green, 15f }, { Rarity.Blue, 4f }, { Rarity.Purple, 0.8f }, { Rarity.Gold, 0.175f }, { Rarity.Rainbow, 0.025f } } },

    { CardType.Ingredient, new Dictionary<Rarity, float>
        { { Rarity.Green, 45f }, { Rarity.Blue, 20f }, { Rarity.Purple, 7f }, { Rarity.Gold, 0.95f }, { Rarity.Rainbow, 0.05f } } },

    { CardType.Partner, new Dictionary<Rarity, float>
        { { Rarity.Green, 6.3f }, { Rarity.Gold, 0.7f } } }
};

    //bigȨ��
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
                                     
    public Color recipeColor;
    public Color ingredientColor;
    public Color partnerColor;
    public Color greenColor;
    public Color blueColor;
    public Color purpleColor;
    public Color goldColor;
    public Color rainbowColor;

    void Start()
    {
        collectedCardIDs.Clear();
        quickPurchaseButton.gameObject.SetActive(false);
        bigPurchaseButton.gameObject.SetActive(false);
        quickPurchaseButton.onClick.AddListener(OnQuickPurchaseClick);
        bigPurchaseButton.onClick.AddListener(OnBigPurchaseClick);
        closeButton.onClick.AddListener(CloseUI);
        cardImageDatabase.Initialize();
        DisablePurchaseButtons();
      // LoadCardImages();  // ���ؿ�ƬͼƬ
    }
    
   /* private void LoadCardImages()
    {
        cardImages.Add("Recipe-1", Resources.Load<Sprite>("Images/Recipe_1"));
        cardImages.Add("Recipe-2", Resources.Load<Sprite>("Images/Recipe_2"));
        cardImages.Add("Recipe-3", Resources.Load<Sprite>("Images/Recipe_3"));
        cardImages.Add("Recipe-4", Resources.Load<Sprite>("Images/Recipe_4"));
        cardImages.Add("Recipe-5", Resources.Load<Sprite>("Images/Recipe_5"));

        cardImages.Add("Ingredient-1", Resources.Load<Sprite>("Images/Ingredient_1"));
        cardImages.Add("Ingredient-2", Resources.Load<Sprite>("Images/Ingredient_2"));
        cardImages.Add("Ingredient-3", Resources.Load<Sprite>("Images/Ingredient_3"));
        cardImages.Add("Ingredient-4", Resources.Load<Sprite>("Images/Ingredient_4"));
        cardImages.Add("Ingredient-5", Resources.Load<Sprite>("Images/Ingredient_5"));

        cardImages.Add("Partner-1", Resources.Load<Sprite>("Images/Partner_1"));
        cardImages.Add("Partner-2", Resources.Load<Sprite>("Images/Partner_2"));
        cardImages.Add("Partner-3", Resources.Load<Sprite>("Images/Partner_3"));
        cardImages.Add("Partner-4", Resources.Load<Sprite>("Images/Partner_4"));
        cardImages.Add("Partner-5", Resources.Load<Sprite>("Images/Partner_5"));
    }*/
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
            TMP_Text nameText = cardUI.transform.Find("NameText").GetComponent<TMP_Text>();
            if (!nameText.enabled)
            {
                nameText.enabled = true; 
            }
            Debug.Log("NameText: " + nameText.text);
            TMP_Text typeText = cardUI.transform.Find("TypeText").GetComponent<TMP_Text>();
            TMP_Text rarityText = cardUI.transform.Find("RarityText").GetComponent<TMP_Text>();
            Image newTagImage = cardUI.transform.Find("NewTagImage").GetComponent<Image>(); 

           // Image newTagImage = cardUI.transform.Find("NewTagImage").GetComponent<Image>(); 

            // cardImageDatebase�е�sprite
            Sprite cardSprite = cardImageDatabase.GetSprite(card.name);

            if (cardSprite != null)
            {
                cardImage.sprite = cardSprite; 
            }


            nameText.text = card.name;
            typeText.text = card.type.ToString();
            rarityText.text = card.rarity.ToString();
   
            typeText.color = GetTypeColor(card.type);
            nameText.color = GetTypeColor(card.type);
            rarityText.color = GetRarityColor(card.rarity);
            Canvas.ForceUpdateCanvases();  //ǿ��ˢ��
            string cardID = card.name + "_" + card.rarity.ToString();
            bool isNew = !collectedCardIDs.Contains(cardID); 
            newTagImage.gameObject.SetActive(isNew); 
            if (isNew)
            {
                collectedCardIDs.Add(cardID);  //new��ӵ�id���У�id���䷽��-������
            }
        }
    }

    // ��ͬϡ�жȲ�ͬ�ı���ɫ
    private Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Green: return greenColor;
            case Rarity.Blue: return blueColor;
            case Rarity.Purple: return purpleColor;
            case Rarity.Gold: return goldColor;
            case Rarity.Rainbow: return rainbowColor;
            default: return Color.white;
        }
    }

    // ��ͬ���͵ĵ�ɫ
    private Color GetTypeColor(CardType type)
    {
        switch (type)
        {
            case CardType.Recipe: return recipeColor;
            case CardType.Ingredient: return ingredientColor;
            case CardType.Partner: return partnerColor;
            default: return Color.white;
        }
    }

    //enable�鿨
    public void EnablePurchaseButtons()
    {
        quickPurchaseButton.gameObject.SetActive(true);
        bigPurchaseButton.gameObject.SetActive(true);
        quickPurchaseButton.interactable = true;
        bigPurchaseButton.interactable = true;
        hasSelectedMode = false;
    }
 
    public void DisablePurchaseButtons()
    {
        
        quickPurchaseButton.interactable = false;
        bigPurchaseButton.interactable = false;
    }
    
    private void OnPurchaseClick(bool isBigMode)
    {
        if (hasSelectedMode) return;

        quickPurchaseButton.gameObject.SetActive(false);
        bigPurchaseButton.gameObject.SetActive(false);

        isBigPurchaseMode = isBigMode;
        hasSelectedMode = true;

        drawnCards.Clear();
        List<Card> newCards = DrawCards(10, isBigMode ? bigPurchaseRates : quickPurchaseRates, isBigMode);
        ProcessDrawnCards(newCards);

        uiPanel.SetActive(true);
        closeButton.gameObject.SetActive(true);  
        closeButton.onClick.AddListener(CloseUI); 
    }

    private void OnQuickPurchaseClick()
    {
        OnPurchaseClick(false);  
    }

    private void OnBigPurchaseClick()
    {
        OnPurchaseClick(true); 
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

        // quick ������
        if (!isBigPurchase && !hasRequiredRarity)
        {
            cards[random.Next(cards.Count)] = GetGuaranteedCard(Rarity.Blue, purchaseRates);
        }

        // big  ������
        if (isBigPurchase && !hasRequiredRarity)
        {
            cards[random.Next(cards.Count)] = GetGuaranteedCard(Rarity.Purple, purchaseRates);
        }

        // �󱣵�
        if (isBigPurchase)//bigpurchase�µ�50�α���
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
        else//quickpurchase�µ�50�δ󱣵�
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

    //���������Ӧ��ͬ���࣬quickpurchase�䷽0-20��ʳ��20-93�����93-100���ܹ�����1-100�������
    //bigpurchase�䷽0-30��ʳ��30-85�����85-100
    //Ȼ�����������в�ͬϡ�жȲ�ͬȨ������ȡ��ͬϡ�ж�
   
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
   //���random�ӳ�����ѡname
    private string GetRandomNameForType(CardType type)
    {
        switch (type)
        {
            case CardType.Recipe: return recipePool[random.Next(recipePool.Count)];
            case CardType.Ingredient: return ingredientPool[random.Next(ingredientPool.Count)];
            case CardType.Partner: return partnerPool[random.Next(partnerPool.Count)];
            default: return "δ֪";
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
            Debug.Log($"�鵽: {card.name} - {card.rarity}");     
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
        Debug.Log($"�䷽ {card.name} ת��Ϊ {recipeExperience} �䷽����");
       
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
        Debug.Log($"ʳ�� {card.name} ת��Ϊ {ingredientFragments} ʳ����Ƭ");
       
    }

    private void ConvertPartnerToResource(Card card)
    {
        if (card.rarity == Rarity.Gold)
        {
            // ��ɫ������������飬�浽������
            GameDataManager.Instance.SaveFullPartner(card.name);
            Debug.Log($"����������� {card.name}");
        }
        else
        {
            // ��ɫ��飬����һ�������Ƭ
            GameDataManager.Instance.AddPartnerFragments(1);
            Debug.Log("�����Ƭ�ۼ�");
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
        hangoutFlow.OnCloseButtonClicked();
    }
}
