using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct CardWeightData
{
    public GameCard card;
    public int weight;
}
public class CHand : MonoBehaviour
{
    #region Inspector
    [SerializeField] private HorizontalLayoutGroup _layout;
    [SerializeField] private OnMouseTooltipCard _tooltipClass;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private float _canvasWidth;
    #endregion

    private List<GameCard> AllCards;
    private List<CardWeightData> _cardDeck;
    private Dictionary<int, GameCard> AllCardsDict;
    private int _totalWeight;

    public List<CardWeightData> CardDeck
    {
        get { return _cardDeck; }
        protected set { _cardDeck = value; }
    }

    private void Awake()
    {
        AllCards = new List<GameCard>();
        AllCardsDict = new Dictionary<int, GameCard>();
        LoadAllCards();
        LoadStartDeck();
        Logger.Log($"카드 불러옴 : {AllCards.Count}, {AllCardsDict.Count}");
    }

    void Start()
    {
        
    }

    void Update()
    {
        int handSize = GetHandSize();
        if (handSize < 5)
        {
            _layout.spacing = -320;
        }
        else
        {
            _layout.spacing = -320 + (_canvasWidth - (handSize * 320)) / (handSize - 1);
        }
    }

    public int GetHandSize()
    {
        return this.transform.childCount;
    }

    public List<GameCard> GetAllHand()
    {
        List<GameObject> handObjects = new List<GameObject>();
        List<GameCard> handCards = new List<GameCard>();
        foreach (Transform child in transform)
        {
            handObjects.Add(child.gameObject);
        }
        foreach (GameObject handObject in handObjects)
        {
            CCard cardComponent = handObject.GetComponent<CCard>();
            if (cardComponent != null)
            {
                handCards.Add(cardComponent.Card);
            }
        }
        return handCards;
    }

    public List<int> GetAllHandByInt()
    {
        List<GameObject> handObjects = new List<GameObject>();
        List<int> handCards = new List<int>();
        foreach (Transform child in transform)
        {
            handObjects.Add(child.gameObject);
        }
        foreach (GameObject handObject in handObjects)
        {
            CCard cardComponent = handObject.GetComponent<CCard>();
            if (cardComponent != null)
            {
                handCards.Add(cardComponent.CardId);
            }
        }
        return handCards;
    }

    public List<CCard> GetAllHandByObject()
    {
        List<GameObject> handObjects = new List<GameObject>();
        List<CCard> handCards = new List<CCard>();
        foreach (Transform child in transform)
        {
            handObjects.Add(child.gameObject);
        }
        foreach (GameObject handObject in handObjects)
        {
            CCard cardComponent = handObject.GetComponent<CCard>();
            if (cardComponent != null)
            {
                handCards.Add(cardComponent);
            }
        }
        return handCards;
    }

    private void LoadAllCards()
    {
        GameCard[] gameCards = Resources.LoadAll<GameCard>("CardData");
        AllCards = new List<GameCard>(gameCards);
        foreach (GameCard card in gameCards)
        {
            if (!AllCardsDict.ContainsKey(card.CardId))
            {
                AllCardsDict[card.CardId] = card;
            }
            else
            {
                Logger.Error("이미 존재하는 카드");
            }
        }
    }

    public List<int> GetCardDeckByInt()
    {
        List<int> deckCards = new List<int>();
        foreach (CardWeightData card in _cardDeck)
        {
            deckCards.Add(card.card.CardId);
        }
        return deckCards;
    }

    private void LoadStartDeck()
    {
        GameCard[] _gameCards = Resources.LoadAll<GameCard>("CardData");
        _cardDeck = new List<CardWeightData>();
        foreach (GameCard card in _gameCards)
        {
            _cardDeck.Add(new CardWeightData { card = card, weight = card.Weight });
        }
        CalculateTotalWeight();
    }

    public void CalculateTotalWeight()
    {
        _totalWeight = 0;
        foreach (CardWeightData data in _cardDeck)
        {
            _totalWeight += data.weight;
        }
    }
    
    public void LoadSavedDeck(List<int> cardId)
    {
        _cardDeck = new List<CardWeightData>();
        for(int i = 0; i < cardId.Count; i++)
        {
            GameCard card = AllCardsDict[cardId[i]];
            _cardDeck.Add(new CardWeightData { card = card, weight = card.Weight });
        }
        CalculateTotalWeight();
    }

    public void ClearHand()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.transform.SetParent(null);
            Destroy(child);
        }
    }

    public void LoadSavedHand(List<int> cardId)
    {
        ClearHand();
        for (int i = 0; i < cardId.Count; i++)
        {
            AddCard(cardId[i], true);
        }
    }

    public bool AddCard()
    {
        if (GetHandSize() >= 10)
        {
            Logger.Error("패 가득참");
            return false;
        }
        if (_cardDeck.Count == 0 || _cardDeck == null)
        {
            Logger.Error("덱 없음");
            return false;
        }
        int weightedIndex = UnityEngine.Random.Range(0, _totalWeight);
        GameCard selectedCard = null;

        for (int i = 0; i < _cardDeck.Count; i++)
        {
            weightedIndex -= _cardDeck[i].weight;
            if (weightedIndex < 0)
            {
                selectedCard = _cardDeck[i].card;
                break;
            }
        }
        if (selectedCard == null)
        {
            Logger.Error("카드 선택 실패");
            return false;
        }

        if (selectedCard.IsSingle)
        {
            _cardDeck.RemoveAll(c => c.card.CardId == selectedCard.CardId);
            CalculateTotalWeight();
            Logger.Log($"덱 삭제 - {selectedCard.CardId}:{selectedCard.CardName} (남은 카드 : {_cardDeck.Count}");
        }
        GameObject go = Instantiate(_cardPrefab, _layout.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(selectedCard, _tooltipClass);
        return true;
    }

    public bool AddCard(int cardId, bool debug = false)
    {
        if (GetHandSize() >= 10)
        {
            Logger.Error("패 가득참");
            return false;
        }
        if (!debug && (_cardDeck.Count == 0 || _cardDeck == null))
        {
            Logger.Error("덱 없음");
            return false;
        }
        if (!debug)
        {
            int temp = _cardDeck.FindIndex(c => c.card.CardId == cardId);
            if (temp == -1)
            {
                Logger.Error("덱에 없는 카드");
                return false;
            }
            if (_cardDeck[temp].card.IsSingle)
            {
                _cardDeck.RemoveAll(c => c.card.CardId == cardId);
                CalculateTotalWeight();
            }
        }

        GameObject go = Instantiate(_cardPrefab, _layout.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(AllCardsDict[cardId], _tooltipClass);
        return true;
    }

    public int AddCards(int count)
    {
        int created = 0;
        for(int i = 0; i < count; i++)
        {
            if (!AddCard()) break;
            created++;
        }
        return created;
    }
}
