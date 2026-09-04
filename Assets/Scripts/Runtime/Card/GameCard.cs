using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class GameCard : ScriptableObject
{
    [SerializeField] private int _cardId;
    [SerializeField] private string _cardName;
    [SerializeField][TextArea(2, 10)] private string _description;
    [SerializeField][TextArea(2, 10)] private string _flavorText;
    [SerializeField][TextArea(2, 10)] private string _tooltip;
    [SerializeField] private Texture _illust;
    [SerializeField] private SCostInfo _costInfo;
    [SerializeField] private bool _isDeletable = true;
    [SerializeField] private bool _isSingle = false;
    [SerializeField] private int _weight = 100;
    [SerializeField] private List<TechData> _tagList = new List<TechData>();
    [SerializeField] private List<ActionData> _actionList = new List<ActionData>();
    [SerializeField][TextArea(2, 10)] private string _tileAdditionalDesc;
    [SerializeField][TextArea(2, 10)] private string _comments;

    public int CardId
    {
        get { return _cardId; }
        protected set { _cardId = value; }
    }
    public string CardName
    {
        get { return _cardName; }
        protected set { _cardName = value; }
    }

    public string Description
    {
        get { return _description; }
        protected set { _description = value; }
    }
    public string FlavorText
    {
        get { return _flavorText; }
        protected set { _flavorText = value; }
    }

    public Texture Illust
    {
        get { return _illust; }
        protected set { _illust = value; }
    }

    public SCostInfo CostInfo
    {
        get { return _costInfo; }
        protected set { _costInfo = value; }
    }

    public bool IsDeletable
    {
        get { return _isDeletable; }
        protected set { _isDeletable = value;}
    }

    public bool IsSingle
    {
        get { return _isSingle; }
        protected set { _isSingle = value; }
    }

    public int Weight
    {
        get { return _weight; }
        protected set { _weight = value; }
    }
    public List<TechData> TagList
    {
        get { return _tagList; }
        protected set { _tagList = value; }
    }

    public List<ActionData> ActionList
    {
        get { return _actionList; }
        protected set { _actionList = value; }
    }

    public string Tooltip
    {
        get { return _tooltip; }
        set { _tooltip = value; }
    }

    public string AdditionalDescription
    {
        get { return _tileAdditionalDesc; }
        set { _tileAdditionalDesc = value; }
    }

}
