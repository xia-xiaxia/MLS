using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
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
    private int consecutiveDrawsWithoutPurple = 0; // ����������ɫ�Ĵ�����5��
    private int consecutiveDrawsWithoutGold = 0;//������
    private bool isBigPurchaseMode = false; 
    public Button quickPurchaseButton;
    public Button bigPurchaseButton;
    public Button closeButton;
    public Button confirmPurchaseButton;
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

    public RawImage videoDisplay; // ��ʾ��Ƶ�� UI
    public VideoPlayer videoPlayer; // ������

    void Start()
    {
        collectedCardIDs.Clear();
        quickPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(false));
        bigPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(true));
        confirmPurchaseButton.onClick.AddListener(ConfirmPurchase);
        closeButton.onClick.AddListener(CloseUI);
        confirmPurchaseButton.gameObject.SetActive(false);
        cardImageDatabase.Initialize();
        videoDisplay.gameObject.SetActive(false); // ��������Ƶ UI
                                                 
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
                collectedCardIDs.Add(cardID);  //new��ӵ�id���У�id���䷽��-������
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
        confirmPurchaseButton.gameObject.SetActive(true); // ѡ��ģʽ����ʾȷ�ϰ�ť

    }
    private void ConfirmPurchase()
    {
        DisablePurchaseButtons(); 
        confirmPurchaseButton.gameObject.SetActive(false); 
        drawnCards.Clear();
        StartCoroutine(PlayVideoAndDrawCards());
    }
    private IEnumerator PlayVideoAndDrawCards()
    {
        videoDisplay.gameObject.SetActive(true);  // ��ʾ Raw Image
        videoPlayer.Play(); // ��ʼ������Ƶ

        // �ȴ���Ƶ�������
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // �ر���Ƶ UI
        videoDisplay.gameObject.SetActive(false);

        // ���г鿨
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
        collectedCardIDs.Clear();  
    }
}
