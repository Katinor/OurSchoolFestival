using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
