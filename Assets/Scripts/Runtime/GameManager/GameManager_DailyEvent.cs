using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    public IEnumerator StartDailyEvent()
    {
        if (_sceneManager.LoadSavedData)
        {
            Logger.Log($"불러온 데이터 : 게임 속행");
            if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
            else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
            else if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
            else _soundManager.PlayBGM(EBackgroundSound.Part3);
            _gameState = EGameState.Idle;
            yield break;
        }
        _dailyEventArg = -1;
        int page = -1;
        switch (_currentDay)
        {
            case 1:
                page = 0;
                _soundManager.PlayBGM(EBackgroundSound.DailyEvent);
                break;
            default:
                break;
        }
        if (page == -1)
        {
            Logger.Log($"{_currentDay}일차 일일이벤트 없음 : 게임 속행");
            yield return StartCoroutine(_cardHand.AddCardCoroutine(4, 0.5f));
            if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
            else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
            else if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
            else _soundManager.PlayBGM(EBackgroundSound.Part3);
            SaveData();
            _gameState = EGameState.Idle;
            yield break;
        }
        else
        {
            if (_currentDay == 1) yield return StartCoroutine(_cardHand.AddCardCoroutine(6, 0.5f));
            else yield return StartCoroutine(_cardHand.AddCardCoroutine(4, 0.5f));
        }
        Logger.Log($"{_currentDay}일차 일일이벤트 시작 : {page} 페이지 호출");
        _nextDayButton.interactable = false;
        _titleButton.interactable = false;
        _dailyManager.StartEvent(page, 0);
    }
    public IEnumerator EndDailyEvent()
    {
        switch (_currentDay)
        {
            case 1:
                GetCard(400, true, true);
                break;
        }
        SaveData();
        if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
        else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
        else if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
        else _soundManager.PlayBGM(EBackgroundSound.Part3);
        _nextDayButton.interactable = true;
        _titleButton.interactable = true;
        _gameState = EGameState.Idle;
        yield break;
    }
}
