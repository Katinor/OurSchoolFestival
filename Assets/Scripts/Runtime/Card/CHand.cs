using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CHand : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _layout;
    [SerializeField] private OnMouseTooltipCard _tooltipClass;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private float _canvasWidth;

    private List<GameCard> AllCards;
    // private List<GameCard> _cardDeck;
    private Dictionary<int, GameCard> AllCardsDict;

    private void Awake()
    {
        AllCards = new List<GameCard>();
        AllCardsDict = new Dictionary<int, GameCard>();
        LoadAllCards();
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

    private void LoadAllCards()
    {
        GameCard[] _gameCards = Resources.LoadAll<GameCard>("CardData");
        AllCards = new List<GameCard>(_gameCards);
        foreach (GameCard card in AllCards)
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

    public bool AddCard()
    {
        if (GetHandSize() >= 10) return false;
        int index = UnityEngine.Random.Range(0, AllCards.Count);
        GameObject go = Instantiate(_cardPrefab, _layout.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(AllCards[index], _tooltipClass);
        return true;
    }
    public bool AddCard(int cardId)
    {
        if (GetHandSize() >= 10) return false;
        GameObject go = Instantiate(_cardPrefab, _layout.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(AllCardsDict[cardId], _tooltipClass);
        return true;
    }

    public bool AddCard(GameCard gameCard)
    {
        if (GetHandSize() >= 10) return false;
        GameObject go = Instantiate(_cardPrefab, _layout.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(gameCard, _tooltipClass);
        return true;
    }
}
