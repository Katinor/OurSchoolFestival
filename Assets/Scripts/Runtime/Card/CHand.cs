using JetBrains.Annotations;
using System.Collections;
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
        CPrint.Log($"카드 불러옴 : {AllCards.Count}, {AllCardsDict.Count}");
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

    private void LoadAllCards()
    {
        GameCard[] _gameCards = Resources.LoadAll<GameCard>("CardData");
        AllCards = new List<GameCard>(_gameCards);
        foreach (GameCard card in _gameCards)
        {
            if (!AllCardsDict.ContainsKey(card.CardId))
            {
                AllCardsDict[card.CardId] = card;
            }
            else
            {
                CPrint.Error("이미 존재하는 카드");
            }
        }
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

    private void CalculateTotalWeight()
    {
        _totalWeight = 0;
        foreach (CardWeightData data in _cardDeck)
        {
            _totalWeight += data.weight;
        }
    }

    public bool AddCard()
    {
        if (GetHandSize() >= 10)
        {
            CPrint.Error("패 가득참");
            return false;
        }
        if (_cardDeck.Count == 0 || _cardDeck == null)
        {
            CPrint.Error("덱 없음");
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
            CPrint.Error("카드 선택 실패");
            return false;
        }

        if (selectedCard.IsSingle)
        {
            _cardDeck.RemoveAll(c => c.card.CardId == selectedCard.CardId);
            CalculateTotalWeight();
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
            CPrint.Error("패 가득참");
            return false;
        }
        if (!debug && (_cardDeck.Count == 0 || _cardDeck == null))
        {
            CPrint.Error("덱 없음");
            return false;
        }
        if (!debug)
        {
            int temp = _cardDeck.FindIndex(c => c.card.CardId == cardId);
            if (temp == -1)
            {
                CPrint.Error("덱에 없는 카드");
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
