using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHand : MonoBehaviour
{
    #region Inspector
    [SerializeField] private OnMouseTooltipCard _tooltipClass;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private int _cardMax = 20;
    #endregion

    private List<GameCard> AllCards;
    private List<GameCard> _cardDeck;
    private List<GameCard> _cardPinoDeck;
    private Dictionary<int, GameCard> AllCardsDict;
    private int _lastHandCount;

    private void Awake()
    {
        AllCards = new List<GameCard>();
        AllCardsDict = new Dictionary<int, GameCard>();
        LoadAllCards();
        LoadStartDeck();
        Logger.Log($"카드 불러옴 : {AllCards.Count}, {AllCardsDict.Count}");
    }

    private void Update()
    {
        if (_lastHandCount != GetHandSize())
        {
            _lastHandCount = GetHandSize();
            CardPositionReset();
        }
    }

    public int GetHandSize()
    {
        return this.transform.childCount;
    }

    public bool CanDraw(int cardCount)
    {
        if ((GetHandSize() + cardCount) > _cardMax) return false;
        else return true;
    }
    public List<GameCard> GetAllHand()
    {
        List<GameObject> handObjects = new List<GameObject>();
        List<GameCard> handCards = new List<GameCard>();
        for (int i = 0; i < transform.childCount; i++)
        {
            handObjects.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < handObjects.Count; i++)
        {
            CCard cardComponent = handObjects[i].GetComponent<CCard>();
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
        for (int i = 0; i < transform.childCount; i++)
        {
            handObjects.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < handObjects.Count; i++)
        {
            CCard cardComponent = handObjects[i].GetComponent<CCard>();
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
        for (int i = 0; i < transform.childCount; i++)
        {
            handObjects.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < handObjects.Count; i++)
        {
            CCard cardComponent = handObjects[i].GetComponent<CCard>();
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
        for (int i=0; i<gameCards.Length; i++)
        {
            GameCard card = gameCards[i];
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
        for (int i = 0; i < _cardDeck.Count; i++)
        {
            deckCards.Add(_cardDeck[i].CardId);
        }
        return deckCards;
    }
    public List<int> GetPinoDeckByInt()
    {
        List<int> deckCards = new List<int>();
        for (int i = 0; i < _cardPinoDeck.Count; i++)
        {
            deckCards.Add(_cardPinoDeck[i].CardId);
        }
        return deckCards;
    }

    private void LoadStartDeck()
    {
        GameCard[] gameCards = Resources.LoadAll<GameCard>("CardData");
        _cardDeck = new List<GameCard>();
        _cardPinoDeck = new List<GameCard>();
        for (int i = 0; i < gameCards.Length; i++)
        {
            GameCard card = gameCards[i];
            if (card.InStartDeck)
            {
                _cardDeck.Add(gameCards[i]);
            }
            if (card.CardId >= 801 && card.CardId <= 899)
            {
                _cardPinoDeck.Add(gameCards[i]);
            }
        }
    }
    public void CardPositionReset()
    {
        int handSize = GetHandSize();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            RectTransform rect = child.GetComponent<RectTransform>();
            Vector3 pos = rect.anchoredPosition3D;
            if (handSize < 5) pos.x = -540 + 320 * i;
            else pos.x = -540 + i * (1080f / (handSize - 1));
            rect.anchoredPosition = pos;
        }
    }

    public void LoadSavedDeck(List<int> cardId, List<int> cardPinoId)
    {
        _cardDeck = new List<GameCard>();
        for(int i = 0; i < cardId.Count; i++)
        {
            GameCard card = AllCardsDict[cardId[i]];
            _cardDeck.Add(card);
        }
        _cardPinoDeck = new List<GameCard>();
        for (int i = 0; i < cardPinoId.Count; i++)
        {
            GameCard card = AllCardsDict[cardPinoId[i]];
            _cardPinoDeck.Add(card);
        }
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
            AddCardUndo(cardId[i]);
        }
        CardPositionReset();
    }

    public bool AddCard()
    {
        if (GetHandSize() >= _cardMax)
        {
            Logger.Error("패 가득참");
            return false;
        }
        if (_cardDeck == null || _cardDeck.Count == 0)
        {
            Logger.Error("덱 없음");
            return false;
        }
        int index = UnityEngine.Random.Range(0, _cardDeck.Count);
        GameCard selectedCard = _cardDeck[index];
        if (selectedCard == null)
        {
            Logger.Error("카드 선택 실패");
            return false;
        }

        if (selectedCard.IsSingle)
        {
            _cardDeck.RemoveAll(c => c.CardId == selectedCard.CardId);
            Logger.Log($"덱 삭제 - {selectedCard.CardId}:{selectedCard.CardName} (남은 카드 : {_cardDeck.Count}");
        }
        GameObject go = Instantiate(_cardPrefab, this.transform);
        go.transform.position += Vector3.right * 2160;
        CCard card = go.GetComponent<CCard>();
        card.Setup(selectedCard, _tooltipClass);
        return true;
    }

    public bool AddCard(int cardId, bool create = false, bool noAnimation = false, bool isTop = false)
    {
        if (!isTop && GetHandSize() >= _cardMax)
        {
            Logger.Error("패 가득참");
            return false;
        }
        if (!create && (_cardDeck == null || _cardDeck.Count == 0))
        {
            Logger.Error("덱 없음");
            return false;
        }
        if (!create)
        {
            int temp = _cardDeck.FindIndex(c => c.CardId == cardId);
            if (temp == -1)
            {
                Logger.Error("덱에 없는 카드");
                return false;
            }
            if (_cardDeck[temp].IsSingle)
            {
                _cardDeck.RemoveAll(c => c.CardId == cardId);
            }
        }

        GameObject go = Instantiate(_cardPrefab, this.transform);
        if (isTop) go.transform.SetAsFirstSibling();
        CCard card = go.GetComponent<CCard>();
        card.Setup(AllCardsDict[cardId], _tooltipClass);
        if (noAnimation)
        {
            card.IsLoaded = true;
        }
        CardPositionReset();
        return true;
    }

    public void AddCardUndo(int cardId)
    {
        GameObject go = Instantiate(_cardPrefab, this.transform);
        CCard card = go.GetComponent<CCard>();
        card.Setup(AllCardsDict[cardId], _tooltipClass);
        card.IsLoaded = true;

        CardPositionReset();
    }

    public void AddMysteryCard()
    {
        if (_cardPinoDeck.Count <= 0)
        {
            Logger.Error("덱 없음");
            return;
        }
        int index = UnityEngine.Random.Range(0, _cardPinoDeck.Count);
        GameCard selectedCard = _cardPinoDeck[index];
        if (selectedCard == null)
        {
            Logger.Error("카드 선택 실패");
            return;
        }

        if (selectedCard.IsSingle)
        {
            _cardPinoDeck.RemoveAll(c => c.CardId == selectedCard.CardId);
            Logger.Log($"덱 삭제 - {selectedCard.CardId}:{selectedCard.CardName} (남은 카드 : {_cardPinoDeck.Count}");
        }
        GameObject go = Instantiate(_cardPrefab, this.transform);
        go.transform.SetAsFirstSibling();
        go.transform.position += Vector3.right * 2160;
        CCard card = go.GetComponent<CCard>();
        card.Setup(selectedCard, _tooltipClass);
        CardPositionReset();
        return;
    }

    public bool MysteryAvailable()
    {
        return (_cardPinoDeck.Count > 0);
    }

    public void AddCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!AddCard()) break;
        }
    }
    
    public IEnumerator AddCardCoroutine(int count, float ratio = 1f)
    {
        for (int i = 0; i < count; i++)
        {
            if (!AddCard()) yield break;
            _soundManager.PlaySE(EEffectSound.CardDraw);
            yield return new WaitForSecondsRealtime(0.25f * ratio);
        }
    }
    
    public void AddCardInDeck(List<int> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            GameCard card = AllCardsDict[target[i]];
            if (card == null)
            {
                Logger.Error("카드 정보에 등록되지 않은 카드");
            }
            else
            {
                if (_cardDeck.Contains(card))
                {
                    Logger.Error("이미 존재하는 카드");
                }
                else
                {
                    _cardDeck.Add(card);
                }
            }
        }
        Logger.Success($"{target} 추가 => 현재 덱 {_cardDeck.Count} 장");
    }

    // 나중에 쓸려고 미리 만들어놓기
    // 제발 나중에 이걸 볼 것
    // 제발
    public bool ValidateCardIds(List<int> cardIds)
    {
        if (cardIds == null)
        {
            Logger.Error("카드 목록이 없습니다.");
            return false;
        }

        for (int i = 0; i < cardIds.Count; i++)
        {
            if (!AllCardsDict.ContainsKey(cardIds[i]))
            {
                Logger.Error($"존재하지 않는 카드 : {cardIds[i]}");
                return false;
            }
        }

        return true;
    }
}
