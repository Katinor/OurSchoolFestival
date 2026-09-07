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
    [SerializeField] private bool _inStartDeck = true;
    [SerializeField] private List<TechData> _tagList = new List<TechData>();
    [SerializeField] private List<ActionData> _actionList = new List<ActionData>();
    [SerializeField] private List<int> _addCardOnHand = new List<int>();
    [SerializeField] private List<int> _addCardOnDeck = new List<int>();
    [SerializeField][TextArea(2, 10)] private string _comments;

    public int CardId
    {
        get { return _cardId; }
    }
    public string CardName
    {
        get { return _cardName; }
    }

    public string Description
    {
        get { return _description; }
    }
    public string FlavorText
    {
        get { return _flavorText; }
    }

    public Texture Illust
    {
        get { return _illust; }
    }

    public SCostInfo CostInfo
    {
        get { return _costInfo; }
    }

    public bool IsDeletable
    {
        get { return _isDeletable; }
    }

    public bool IsSingle
    {
        get { return _isSingle; }
    }
    public bool InStartDeck
    {
        get { return _inStartDeck; }
    }

    public List<TechData> TagList
    {
        get { return _tagList; }
    }

    public List<ActionData> ActionList
    {
        get { return _actionList; }
    }

    public List<int> AddCardOnHand
    {
        get { return _addCardOnHand; }
    }
    public List<int> AddCardOnDeck
    {
        get { return _addCardOnDeck; }
    }

    public string Tooltip
    {
        get { return _tooltip; }
        set { _tooltip = value; }
    }

}
