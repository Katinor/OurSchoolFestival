using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CDialogData
{
    [SerializeField] private string _speakerName;
    [SerializeField][TextArea(2, 10)] private string _dialogueText;

    [SerializeField] private bool _illustHide;
    [SerializeField] private bool _illustChange;
    [SerializeField] private int _illustIndex;
    [SerializeField] private EDialogBGM _bgmSwitch;
    [SerializeField] private EBackgroundSound _backgroundSound;
    [SerializeField] private bool _seChange;
    [SerializeField] private EEffectSound _effectSound;

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
    public EDialogBGM BackgroundEffect
    {
        get { return _bgmSwitch; }
    }
    public EBackgroundSound BackgroundSound
    {
        get { return _backgroundSound; }
    }
    public bool EffectSwitch
    {
        get { return _seChange; }
    }
    public EEffectSound EffectSound
    {
        get { return _effectSound; }
    }
}
public class PrologueSet : MonoBehaviour
{
    [Header("대사 목록")]
    [SerializeField] List<CDialogData> _dialogData;

    public CDialogData GetDialogueData(int index)
    {
        return _dialogData[index];
    }

    public bool IsLast(int index)
    {
        return (index == _dialogData.Count -1);
    }
}
