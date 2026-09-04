using System;
using System.Collections.Generic;

public class CUndoData
{
    private string _name;
    private CResources _resources;
    private Dictionary<ETech, int> _currentTech;
    private List<Func<GameManager, SScoreInfo>> _cardScoresList;
    private List<int> _cardsOnHand;
    private List<int> _cardsOnDeck;
    private List<int> _tileInt;
    private List<int> _tilePoint;
    private List<bool> _tileUsed;

    public string Name
    {
        get { return _name; }
        protected set { _name = value; }
    }
    public CResources Resources
    {
        get { return _resources; }
        protected set { _resources = value; }
    }
    public Dictionary<ETech, int> CurrentTech
    {
        get { return _currentTech; }
        protected set { _currentTech = value; }
    }
    public List<Func<GameManager, SScoreInfo>> CardScoresList
    {
        get { return _cardScoresList; }
        private set { _cardScoresList = value; }
    }
    public List<int> CardsOnHand
    {
        get { return _cardsOnHand; }
        private set { _cardsOnHand = value; }
    }
    public List<int> CardsOnDeck
    {
        get { return _cardsOnDeck; }
        private set { _cardsOnDeck = value; }
    }
    public List<int> TileInt
    {
        get { return _tileInt; }
        private set { _tileInt = value; }
    }
    public List<int> TilePoint
    {
        get { return _tilePoint; }
        private set { _tilePoint = value; }
    }

    public List<bool> TileUsed
    {
        get { return _tileUsed; }
        private set { _tileUsed = value; }
    }

    public CUndoData
        (
            string name,
            CResources resources,
            Dictionary<ETech, int> currentTech,
            List<Func<GameManager, SScoreInfo>> cardScoresList,
            List<int> cardsOnHand,
            List<int> cardsOnDeck,
            List<int> tileInt,
            List<int> tilePoint,
            List<bool> tileUsed
        )
    {
        _name = name;
        _resources = new CResources(resources);
        _currentTech = new Dictionary<ETech, int>(currentTech);
        _cardScoresList = new List<Func<GameManager, SScoreInfo>>(cardScoresList);
        Logger.Log($"카드점수 저장 - {_cardScoresList.Count}");
        _cardsOnHand = new List<int>(cardsOnHand);
        _cardsOnDeck = new List<int>(cardsOnDeck);
        _tileInt = new List<int>(tileInt);
        _tilePoint = new List<int>(tilePoint);
        _tileUsed = new List<bool>(tileUsed);
    }
}

public partial class GameManager
{
    public void PushUndo(string name)
    {
        (List<int> tileIdList, List<int> tilePointList, List<bool> tileUsedList) = GetAllTilesForUndo();
        _undoDataList.Push(
            new CUndoData(
                name,
                _resources,
                _currentTech,
                _cardScores,
                _cardHand.GetAllHandByInt(),
                _cardHand.GetCardDeckByInt(),
                tileIdList,
                tilePointList,
                tileUsedList
                )
            );
        _undoButton.interactable = true;
        _undoText.gameObject.SetActive(true);
        _undoText.text = _undoDataList.Count.ToString();
        _undoTooltip.SetText($"[{name}]을\n되돌립니다.");
    }

    public void PopUndo()
    {
        if (_gameState == EGameState.NoInput)
        {
            CreateError("되돌리기 불가능");
            return;
        }
        _soundManager.PlaySE(EEffectSound.QuestionChoose);
        CUndoData undoData = _undoDataList.Pop();
        _resources = undoData.Resources;
        _currentTech = undoData.CurrentTech;
        ReloadTech();
        _cardScores = undoData.CardScoresList;
        Logger.Log($"카드점수 - {undoData.CardScoresList.Count} => {_cardScoresList.Count} => {_cardScores.Count}");
        _cardHand.LoadSavedHand(undoData.CardsOnHand);
        _cardHand.LoadSavedDeck(undoData.CardsOnDeck);
        LoadTilesFromUndo(undoData.TileInt, undoData.TilePoint, undoData.TileUsed);

        if (_undoDataList.Count == 0)
        {
            _undoButton.interactable = false;
            _undoText.gameObject.SetActive(false);
            _undoText.text = "";
            _undoTooltip.SetText("");
        }
        else
        {
            _undoTooltip.SetText($"{_undoDataList.Peek().Name}을\n되돌립니다.");
            _undoText.text = _undoDataList.Count.ToString();
        }
    }

    public void ClearUndo()
    {
        _undoDataList.Clear();
        _undoButton.interactable = false;
        _undoText.gameObject.SetActive(false);
        _undoText.text = "";
        _undoTooltip.SetText("");
    }
}
