using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class CCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameCard _testCard;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private TMP_Text _tagLabel;
    [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private Button _useButton;
    [SerializeField] private RawImage _illust;

    private string _cardName;
    private GameCard _gameCard;
    private SCost _cost;
    private bool _canUseMaterials = false;
    private List<TechData> _techData;
    private Test_TilemapSelector _tilemapSelector;
    private List<Func<Test_TilemapSelector, int, bool>> _actionFuncList;
    private List<int> _actionLevelList;
    private bool _hasTileAction = false;

    private RectTransform _rectTransform;
    private bool _isLooking = false;
    private readonly float YOnPosition = 176f;
    private readonly float YOffPosition = -184f;
    private readonly float MovementSpeed = 1800f;

    public string CardName
    {
        get { return _cardName; }
        protected set { _cardName = value; }
    }

    void Awake()
    {
        _actionFuncList = new List<Func<Test_TilemapSelector, int, bool>>();
        _actionLevelList = new List<int>();
        if(_testCard != null)
        {
            _gameCard = _testCard;
            Setup(_gameCard);
        }
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        _tilemapSelector = FindObjectOfType<Test_TilemapSelector>();
        if (_useButton != null)
        {
            _useButton.onClick.AddListener(
                () => _tilemapSelector.CallCard(this));
        }
    }

    void Update()
    {
        if (_isLooking)
        {
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

    public void Setup(GameCard targetCard)
    {
        _nameLabel.text = targetCard.CardName;
        _cardName = targetCard.CardName;
        _costLabel.text = targetCard.CostInfo.moneyCurrent.ToString();
        _techData = targetCard.TagList;
        _tagLabel.text = "";
        for(int i = 0; i < _techData.Count; i++)
        {
            if (i > 0) _tagLabel.text += ", ";
            _tagLabel.text += TagTranslator(_techData[i]);
        }
        _cost = new SCost(targetCard.CostInfo, _canUseMaterials);

        _descriptionLabel.text = targetCard.Description;
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
                    _actionFuncList.Add(CCardStatic.CardTile);
                    _hasTileAction = true;
                    break;
                case EAction.CustomScript:
                    _actionFuncList.Add(CCardStatic.CardCustom);
                    break;
            }
            _actionLevelList.Add(targetCard.ActionList[i].level);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isLooking = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isLooking = false;
    }

    private string TagTranslator(TechData data)
    {
        switch (data.tag)
        {
            case ETech.Structure:
                _canUseMaterials = true;
                return "자재 결제가능";
            case ETech.Success:
                return $"완성도 {data.level} 필요";
            case ETech.Interest:
                return $"관심도 {data.level} 필요";
            case ETech.Road:
                return $"안정도 {data.level} 필요";
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

    public void UseCard()
    {
        _tilemapSelector.Resources.PayCost(_cost);
        bool isDone = true;
        for(int i = 0; i < _actionFuncList.Count; i++)
        {
            if(!_actionFuncList[i](_tilemapSelector, _actionLevelList[i])) isDone = false;
        }
        if (isDone)
        {
            CPrint.Success($"{_cardName} 발동 성공!");
        }
        else
        {
            CPrint.Error($"{_cardName} 발동 실패");
        }
        Destroy(gameObject);
    }

    public bool AvailableToUse()
    {
        if (_tilemapSelector.CheckResource(_cost) < 0) return false;
        for(int i = 0; i < _techData.Count; i++)
        {
            TechData tempData = _techData[i];
            if (_tilemapSelector.CurrentTech.ContainsKey(tempData.tag))
            {
                if (_tilemapSelector.CurrentTech[tempData.tag] < tempData.level) return false;
            }
            else return false;
        }
        return true;
    }

}
