using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TechData
{
    public ETech tag;
    public int level;
}

[Serializable]
public struct ActionData
{
    public EAction action;
    public int level;
}

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class GameCard : ScriptableObject
{
    [SerializeField] private string _cardName;
    [SerializeField][TextArea(2, 10)] private string _desctiption;
    [SerializeField] private Texture _illust;
    [SerializeField] private SCostInfo _costInfo;
    [SerializeField] private List<TechData> _tagList = new List<TechData>();
    [SerializeField] private List<ActionData> _actionList = new List<ActionData>();


    public string CardName
    {
        get { return _cardName; }
        protected set { _cardName = value; }
    }

    public string Description
    {
        get { return _desctiption; }
        protected set { _desctiption = value; }
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

}
