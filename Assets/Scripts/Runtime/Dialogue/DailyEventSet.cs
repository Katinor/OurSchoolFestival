using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CQuestionData
{
    public string _question;
    public int _pageIndex;
    public int _dialogueIndex;
}

[Serializable]
public class CDailyEventData
{
    [SerializeField] private string _speakerName;
    [SerializeField][TextArea(2, 10)] private string _dialogueText;

    [SerializeField] private bool _illustHide;
    [SerializeField] private bool _illustChange;
    [SerializeField] private int _illustIndex;

    [SerializeField] private List<CQuestionData> _questions;

    public string SpeakerName
    {
        get { return _speakerName; }
    }
    public string Dialogue
    {
        get { return _dialogueText; }
    }
    public bool IllustHide
    {
        get { return _illustHide; }
    }
    public bool IllustChange
    {
        get { return _illustChange; }
    }
    public int IllustIndex
    {
        get { return _illustIndex; }
    }
    public List<CQuestionData> Questions
    {
        get { return _questions; }
    }
}
public class DailyEventSet : MonoBehaviour
{
    [Header("대사 목록")]
    [SerializeField] List<CDailyEventData> _dialogData;

    public CDailyEventData GetDialogueData(int index)
    {
        return _dialogData[index];
    }

    public bool IsLast(int index)
    {
        return (index == _dialogData.Count -1);
    }
}
