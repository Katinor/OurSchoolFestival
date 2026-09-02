using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class CCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Inspector
    [SerializeField] GameCard _testCard;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private TMP_Text _tagLabel;
    [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private RawImage _illust;
    #endregion

    #region Member Variable - CardData
    private string _cardName;
    private GameCard _gameCard;
    private string _tileAdditionalDescription;
    private string _tooltip;
    private bool _isDeletable;
    private SCost _cost;
    private int _costMoney;
    private bool _canUseMaterials = false;
    private List<TechData> _techData;
    private GameManager _gameManager;
    private OnMouseTooltipCard _onMouseTooltipCard;
    private List<Func<GameManager, int, bool>> _actionFuncList;
    private Func<GameManager, ETileCatalog, Vector3Int, bool> _actionTileFunc;
    private Func<GameManager, SScoreInfo> _actionScoreFunction;
    private ETileCatalog _actionBuilding;
    private List<int> _actionLevelList;
    private bool _hasTileAction = false;
    private bool _hasPointFunction = false;
    private bool _isTileRoad = false;
    private bool _canPayMaterials = false;
    private bool _isSingle = false;
    private int _weight = 100;
    #endregion

    #region Member Variable - UI
    private RectTransform _rectTransform;
    private bool _isLooking = false;
    private readonly float YOnPosition = 180f;      // 176f;
    private readonly float YOffPosition = -240f;    //-184f;
    private readonly float MovementSpeed = 1800f;
    #endregion

    #region Property
    public string CardName
    {
        get { return _cardName; }
        protected set { _cardName = value; }
    }

    public GameCard Card
    {
        get { return _gameCard; }
        protected set { _gameCard = value; }
    }

    public GameCard GameCard
    {
        get { return _gameCard; }
        protected set { _gameCard = value; }
    }

    public int Cost
    {
        get { return _costMoney; }
        protected set { _costMoney = value; }
    }

    public bool IsDeletable
    {
        get { return _isDeletable; }
        protected set { _isDeletable = value; }
    }

    public bool HasTileAction
    {
        get { return _hasTileAction; }
        protected set { _hasTileAction = value; }
    }

    public bool HasPointFunction
    {
        get { return _hasTileAction; }
        protected set { _hasTileAction = value; }
    }

    public bool IsTileRoad
    {
        get { return _isTileRoad; }
        protected set { _isTileRoad = value; }
    }

    public bool CanPayMaterials
    {
        get { return _canPayMaterials; }
        protected set { _canPayMaterials = value; }
    }

    public int Weight
    {
        get { return _weight; }
        set { _weight = value; }
    }

    public bool IsSingle
    {
        get { return _isSingle; }
        set { _isSingle = value; }
    }
    #endregion

    #region Unity Event
    void Awake()
    {
        _actionFuncList = new List<Func<GameManager, int, bool>>();
        _actionLevelList = new List<int>();
        _cost = new SCost();
        if(_testCard != null)
        {
            _gameCard = _testCard;
            Setup(_gameCard);
        }
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (!_onMouseTooltipCard)
        {
            _onMouseTooltipCard = FindObjectOfType<OnMouseTooltipCard>();
        }
        if (_useButton != null)
        {
            _useButton.onClick.AddListener(
                () => _gameManager.CallCard(this, false));
        }
        if (_deleteButton != null)
        {
            _deleteButton.onClick.AddListener(
                () => _gameManager.DeleteCard(this));
        }
    }

    void Update()
    {
        if (_isLooking)
        {
            if (_gameManager.GameState == EGameState.Idle || _gameManager.GameState == EGameState.TileInspect)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    _useButton.onClick.Invoke();
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    _deleteButton.onClick.Invoke();
                }
            }
            if (_rectTransform.anchoredPosition3D.y <= YOnPosition)
            {
                _rectTransform.anchoredPosition3D += Vector3.up * MovementSpeed * Time.deltaTime;
                if(_rectTransform.anchoredPosition3D.y >= YOnPosition)
                {
                    Vector3 tempPos = _rectTransform.anchoredPosition3D;
                    tempPos.y = YOnPosition;
                    _rectTransform.anchoredPosition3D = tempPos;
                }
            }
        }
        else
        {
            if (_rectTransform.anchoredPosition3D.y >= YOffPosition)
            {
                _rectTransform.anchoredPosition3D -= Vector3.up * MovementSpeed * Time.deltaTime;
                if (_rectTransform.anchoredPosition3D.y <= YOffPosition)
                {
                    Vector3 tempPos = _rectTransform.anchoredPosition3D;
                    tempPos.y = YOffPosition;
                    _rectTransform.anchoredPosition3D = tempPos;
                }
            }
        }
    }
    #endregion

    public void Setup(GameCard targetCard, OnMouseTooltipCard tooltipClass = null)
    {
        _gameCard = targetCard;
        _nameLabel.text = targetCard.CardName;
        _cardName = targetCard.CardName;
        _isDeletable = targetCard.IsDeletable;
        _costLabel.text = targetCard.CostInfo.moneyCurrent.ToString();
        _techData = targetCard.TagList;
        _tagLabel.text = "";
        _canPayMaterials = false;
        for (int i = 0; i < _techData.Count; i++)
        {
            if (i > 0) _tagLabel.text += ", ";
            _tagLabel.text += TagTranslator(_techData[i]);
            if (_techData[i].tag == ETech.Structure)
            {
                _canPayMaterials = true;
            }
        }
        _cost = new SCost(targetCard.CostInfo, _canUseMaterials);
        _costMoney = _cost.moneyCurrent;
        _weight = targetCard.Weight;
        _isSingle = targetCard.IsSingle;

        _descriptionLabel.text = $"{targetCard.Description}\n<i><size=75%>{targetCard.FlavorText}</size><i>";
        _tooltip = targetCard.Tooltip;
        _onMouseTooltipCard = tooltipClass;
        _illust.texture = targetCard.Illust;

        for(int i = 0; i < targetCard.ActionList.Count; i++)
        {
            switch (targetCard.ActionList[i].action)
            {
                case EAction.moneyCurrent:
                    _actionFuncList.Add(CCardStatic.CardMoneyCurrent);
                    break;
                case EAction.moneyIncrease:
                    _actionFuncList.Add(CCardStatic.CardMoneyIncrease);
                    break;
                case EAction.materialsCurrent:
                    _actionFuncList.Add(CCardStatic.CardMaterialsCurrent);
                    break;
                case EAction.materialsIncrease:
                    _actionFuncList.Add(CCardStatic.CardMaterialsIncrease);
                    break;
                case EAction.menpowerCurrent:
                    _actionFuncList.Add(CCardStatic.CardMenpowerCurrent);
                    break;
                case EAction.menpowerIncrease:
                    _actionFuncList.Add(CCardStatic.CardMenpowerIncrease);
                    break;
                case EAction.Success:
                    _actionFuncList.Add(CCardStatic.CardSuccess);
                    break;
                case EAction.Interest:
                    _actionFuncList.Add(CCardStatic.CardInterest);
                    break;
                case EAction.Road:
                    _actionFuncList.Add(CCardStatic.CardRoad);
                    break;
                case EAction.Science:
                    _actionFuncList.Add(CCardStatic.CardScience);
                    break;
                case EAction.Music:
                    _actionFuncList.Add(CCardStatic.CardMusic);
                    break;
                case EAction.Art:
                    _actionFuncList.Add(CCardStatic.CardArt);
                    break;
                case EAction.Exercise:
                    _actionFuncList.Add(CCardStatic.CardExercise);
                    break;
                case EAction.Cult:
                    _actionFuncList.Add(CCardStatic.CardCult);
                    break;
                case EAction.Tile:
                    _actionFuncList.Add(CCardStatic.CardEmpty);
                    _actionTileFunc = CCardStatic.CardTile;
                    if (targetCard.ActionList[i].level < 0) _isTileRoad = true;
                    _actionBuilding = (ETileCatalog) Mathf.Abs(targetCard.ActionList[i].level);
                    _hasTileAction = true;
                    break;
                case EAction.CustomScript:
                    _actionFuncList.Add(CCardStatic.CardCustom);
                    break;
                case EAction.ScoreFunction:
                    _actionFuncList.Add(CCardStatic.CardEmpty);
                    _actionScoreFunction = CCardStatic.FindPointFunction(targetCard.ActionList[i].level);
                    _hasPointFunction = true;
                    break;
            }
            _actionLevelList.Add(targetCard.ActionList[i].level);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isLooking = true;
        if (!string.IsNullOrWhiteSpace(_tooltip))
        {
            _onMouseTooltipCard.Enable(_tooltip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isLooking = false;
        _onMouseTooltipCard.Disable();
    }

    private string TagTranslator(TechData data)
    {
        switch (data.tag)
        {
            case ETech.Structure:
                _canUseMaterials = true;
                return "자재 결제가능";
            case ETech.Success:
                if (data.level > 0) return $"완성도 {data.level} 필요";
                else return $"완성도 {-data.level} 이하";
            case ETech.Interest:
                if (data.level > 0) return $"관심도 {data.level} 필요";
                else return $"관심도 {-data.level} 이하";
            case ETech.Road:
                if (data.level > 0) return $"안정도 {data.level} 필요";
                else return $"안정도 {-data.level} 이하";
            case ETech.Science:
                return $"과학 {data.level} 필요";
            case ETech.Music:
                return $"음악 {data.level} 필요";
            case ETech.Art:
                return $"미술 {data.level} 필요";
            case ETech.Exercise:
                return $"활동 {data.level} 필요";
            case ETech.Cult:
                return $"사교 {data.level} 필요";
                    }
        return "";
    }

    public void UseCard(Vector3Int pos, int MaterialCount, bool skipTile = false)
    {
        if (CanPayMaterials)
        {
            SCost newCost = new SCost(_cost, MaterialCount);
            _gameManager.Resources.PayCost(newCost);
        }
        else
        {
            _gameManager.Resources.PayCost(_cost);
        }   
        bool isDone = true;
        for(int i = 0; i < _actionFuncList.Count; i++)
        {
            if(!_actionFuncList[i](_gameManager, _actionLevelList[i])) isDone = false;
        }
        if (_hasTileAction && !skipTile)
        {
            _gameManager.BuildTile(_actionBuilding, pos);
        }
        if (_hasPointFunction)
        {
            _gameManager.AddScoreAction(_actionScoreFunction);
        }
        if (isDone)
        {
            CPrint.Success($"{_cardName} 발동 성공!");
        }
        else
        {
            CPrint.Error($"{_cardName} 발동 실패");
        }
        transform.SetParent(null);
        Destroy(gameObject);
    }

    public void DeleteCard()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }

    public bool AvailableToUse()
    {
        if (!_gameManager.CheckResource(_cost)) return false;
        for(int i = 0; i < _techData.Count; i++)
        {
            TechData tempData = _techData[i];
            if (tempData.tag == ETech.Structure)
            {
                continue;
            }
            else if(tempData.tag == ETech.Success)
            {
                if(tempData.level > 0)
                {
                    if (_gameManager.Resources.festivalSuccess < tempData.level) return false;
                }
                else
                {
                    if (_gameManager.Resources.festivalSuccess > -tempData.level) return false;
                }
            }
            else if(tempData.tag == ETech.Interest)
            {
                if (tempData.level > 0)
                {
                    if (_gameManager.Resources.festivalInterest < tempData.level) return false;
                }
                else
                {
                    if (_gameManager.Resources.festivalInterest > -tempData.level) return false;
                }
            }
            else if (tempData.tag == ETech.Road)
            {
                if (tempData.level > 0)
                {
                    if (_gameManager.Resources.festivalRoad < tempData.level) return false;
                }
                else
                {
                    if (_gameManager.Resources.festivalRoad > -tempData.level) return false;
                }
            }
            else if (_gameManager.CurrentTech.ContainsKey(tempData.tag))
            {
                if (_gameManager.CurrentTech[tempData.tag] < tempData.level) return false;
            }
            else return false;
        }
        return true;
    }

    public int GetRadius()
    {
        if (_hasTileAction)
        {
            TileBase _hexRuleTile = _gameManager.GetTileBase(_actionBuilding);
            if (_hexRuleTile is RuleTile ruleTile)
            {
                CTile tempTile = ruleTile.m_DefaultGameObject.GetComponent<CTile>();
                return tempTile.Radius;
            }
            else return -1;
        }
        else return -1;
    }
}
