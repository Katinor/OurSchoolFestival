using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager
{
    public IEnumerator StartDailyEvent()
    {
        ClearUndo();
        _dailyEventArg = -1;
        int page = -1;
        switch (_currentDay)
        {
            case 1:
                page = 0;
                break;
            case 2:
                page = 1;
                break;
            case 3:
                page = 2;
                break;
            case 4:
                page = 3;
                break;
            case 5:
                page = 4;
                break;
            default:
                break;
        }
        if (_currentDay == 16)
        {
            _soundManager.PlayBGM(EBackgroundSound.Result);
            _sceneManager.LoadSavedData = false;
            _gameState = EGameState.LastDayIdle;
            yield break;
        }
        else if (page == -1)
        {
            Logger.Log($"{_currentDay}일차 일일이벤트 없음 : 게임 속행");
            if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
            else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
            else _soundManager.PlayBGM(EBackgroundSound.Part3);
            yield return StartCoroutine(DailyOpening());
            if (_sceneManager != null && !_sceneManager.LoadSavedData)
            {
                _randomSeed = Random.Range(int.MinValue, int.MaxValue);
                Random.InitState(_randomSeed);
                SaveData();
            }
            if (_sceneManager != null ) _sceneManager.LoadSavedData = false;
            yield return StartCoroutine(_cardHand.AddCardCoroutine(4, 1f));
            _gameState = EGameState.Idle;
            yield break;
        }
        _soundManager.PlayBGM(EBackgroundSound.DailyEvent);
        yield return StartCoroutine(DailyOpening());
        if (_sceneManager != null && !_sceneManager.LoadSavedData)
        {
            _randomSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(_randomSeed);
            SaveData();
        }
        if (_sceneManager != null) _sceneManager.LoadSavedData = false;
        if (_currentDay == 1) yield return StartCoroutine(_cardHand.AddCardCoroutine(6, 0.5f));
        else yield return StartCoroutine(_cardHand.AddCardCoroutine(4, 1f));
        Logger.Log($"{_currentDay}일차 일일이벤트 시작 : {page} 페이지 호출");
        _nextDayButton.interactable = false;
        _titleButton.interactable = false;
        _dailyManager.StartEvent(page, 0);
        yield break;
    }
    public IEnumerator EndDailyEvent()
    {
        switch (_currentDay)
        {
            case 1:
                GetCard(400, true, true);
                break;
            case 2:
                GetCard(500, true, true);
                break;
            case 3:
                GetCard(600, true, true);
                break;
            case 4:
                GetCard(700, true, true);
                break;
            case 5:
                GetCard(800, true, true);
                break;
        }
        if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
        else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
        else if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
        else _soundManager.PlayBGM(EBackgroundSound.Part3);
        _nextDayButton.interactable = true;
        _titleButton.interactable = true;
        _gameState = EGameState.Idle;
        yield break;
    }

    private IEnumerator DailyOpening()
    {
        yield return null;
        Color color;
        if (_currentDay >= 15) yield break;
        _dailyOpeningText.text = $"{_currentDay}일차\n";
        if (_currentDay < 6) _dailyOpeningText.text += "<size=75%>준비와 개막</size>";
        else if (_currentDay < 11) _dailyOpeningText.text += "<size=75%>축제 본행사</size>";
        else _dailyOpeningText.text += "<size=75%>축제 하이라이트</size>";
        color = _dailyOpeningText.color;
        color.a = 0f;
        _dailyOpeningText.color = color;
        _dailyOpeningText.gameObject.SetActive(true);
        yield return null;
        yield return StartCoroutine(FadeTo_Text(_dailyOpeningText, 1f, _fadeDuration * 0.5f, true));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeTo_Text(_dailyOpeningText, 0f, _fadeDuration * 0.5f, true));
        _dailyOpeningText.gameObject.SetActive(false);
        if ((_currentDay - 1) % 5 == 0)
        {
            color = _dailyTitleText.color;
            color.a = 0f;
            _dailyTitleText.color = color;
            if (_currentDay < 6) _dailyTitleText.text = "준비와 개막\n<size=60%>축제를 만들어갈\n15일이 시작됩니다.</size>";
            else if (_currentDay < 11) _dailyTitleText.text = "축제 본행사\n<size=60%>모든 동아리가 모였습니다.\n이제 본격적인 축제를 시작하세요.</size>";
            else _dailyTitleText.text = "축제 하이라이트\n<size=60%>남은 기간은 단 5일.\n최고의 축제를 완성하세요.</size>";
            _dailyTitleText.gameObject.SetActive(true);
            yield return null;
            yield return StartCoroutine(FadeTo_Text(_dailyTitleText, 1f, _fadeDuration * 0.5f, true));
            yield return new WaitForSecondsRealtime(2f);
            yield return StartCoroutine(FadeTo_Text(_dailyTitleText, 0f, _fadeDuration * 0.5f, true));
            _dailyTitleText.gameObject.SetActive(false);
            yield break;
        }
        else yield break;
    }

    private IEnumerator FadeTo_Text(TMP_Text target, float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (target == null)
        {
            yield break;
        }
        if (duration < 0f)
        {
            duration = _fadeDuration;
        }

        yield return StartCoroutine(FadeInternal_Text(target, targetAlpha, duration, blockRaycastWhileFading));
    }

    private IEnumerator FadeInternal_Text(TMP_Text target, float targetAlpha, float duration, bool blockRaycastWhileFading)
    {
        Color color;
        float startAlpha = target.color.a;

        if (duration <= 0f)
        {
            color = target.color;
            color.a = targetAlpha;
            target.color = color;
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            float lerp = Mathf.Clamp01(t / duration);
            color = target.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            target.color = color;
            yield return null;
        }
        color = target.color;
        color.a = targetAlpha;
        target.color = color;
    }
}
