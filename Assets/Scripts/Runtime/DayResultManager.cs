using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayResultManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private float _defaultFadeDuration = 0.25f;
    [SerializeField] private float _waitTime = 1f;

    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private TMP_Text _resultTitle;
    [SerializeField] private TMP_Text _titleFestival;
    [SerializeField] private TMP_Text _scoreFestival;
    [SerializeField] private TMP_Text _descFestival;
    [SerializeField] private TMP_Text _titleTile;
    [SerializeField] private TMP_Text _scoreTile;
    [SerializeField] private TMP_Text _descTile;
    [SerializeField] private TMP_Text _titleCard;
    [SerializeField] private TMP_Text _scoreCard;
    [SerializeField] private TMP_Text _descCard;
    [SerializeField] private TMP_Text _titleAchievement;
    [SerializeField] private TMP_Text _scoreAchievement;
    [SerializeField] private TMP_Text _descAchievement;
    [SerializeField] private TMP_Text _totalScore;
    [SerializeField] private Button _nextDay;
    [SerializeField] private TMP_Text _nextDayText;

    private bool _isPressed = false;

    private Coroutine _fadeRoutine;

    private void Start()
    {
        if (_fadeGroup == null)
        {
            Logger.Error("페이드 그룹이 비어있음");
            enabled = false;
        }
        if (_nextDay == null)
        {
            Logger.Error("다음 날 버튼이 비어있음");
            enabled = false;
        }
        else
        {
            _nextDay.onClick.AddListener(() =>
            {
                _isPressed = true;
            });
        }
        _fadeGroup.alpha = 0f;
        TurnOffAll();
    }
    public IEnumerator LoadingScreenOn(float ratio = 1f)
    {
        _loadingText.gameObject.SetActive(true);
        yield return StartCoroutine(Co_FadeTo(1f, _defaultFadeDuration * ratio, true));
    }

    public IEnumerator LoadingScreenOff(float ratio = 1f)
    {
        _loadingText.gameObject.SetActive(false);
        yield return StartCoroutine(Co_FadeTo(0f, _defaultFadeDuration * ratio, true));
    }
    public IEnumerator StartDayResult(GameManager gameManager, SoundManager soundManager)
    {
        SScoreInfo tempInfo;
        _isPressed = false;
        TurnOffAll();
        gameManager.CalculateScore();
        _loadingText.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.QuestionAppear);
        if (gameManager.CurrentDay >= 16) _resultTitle.text = $"게임 결과";
        else _resultTitle.text = $"{gameManager.CurrentDay}일차 결과";
        _resultTitle.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime * 2);
        soundManager.PlaySE(EEffectSound.QuestionAppear);
        _titleFestival.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.Success);
        _scoreFestival.text = gameManager.GetFestivalScore().ToString();
        _descFestival.text = gameManager.GetFestivalDesc();
        _scoreFestival.gameObject.SetActive(true);
        _descFestival.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.QuestionAppear);
        _titleTile.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.Success);
        tempInfo = gameManager.GetTileScore();
        _scoreTile.text = tempInfo.Score.ToString();
        _descTile.text = tempInfo.Description;
        _scoreTile.gameObject.SetActive(true);
        _descTile.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.QuestionAppear);
        _titleCard.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.Success);
        tempInfo = gameManager.GetCardScore();
        _scoreCard.text = tempInfo.Score.ToString();
        _descCard.text = tempInfo.Description;
        _scoreCard.gameObject.SetActive(true);
        _descCard.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.QuestionAppear);
        _titleAchievement.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime);
        soundManager.PlaySE(EEffectSound.Success);
        tempInfo = gameManager.GetAchievementScore();
        _scoreAchievement.text = tempInfo.Score.ToString();
        _descAchievement.text = tempInfo.Description;
        _scoreAchievement.gameObject.SetActive(true);
        _descAchievement.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime * 2);
        soundManager.PlaySE(EEffectSound.Success);
        _totalScore.text = "총점 : " + gameManager.GetTotalScore().ToString();
        _totalScore.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(_waitTime * 2);
        soundManager.PlaySE(EEffectSound.Success);
        if (gameManager.CurrentDay >= 15) _nextDayText.text = $"타이틀로";
        else _nextDayText.text = $"다음날\n<size=75%>{gameManager.CurrentDay + 1}일차로</size>";
        _nextDay.gameObject.SetActive(true);
        _fadeGroup.interactable = true;
        yield return new WaitUntil(() => _isPressed);
        if (gameManager.CurrentDay >= 16) gameManager.CallGotoTitleResult();
        else TurnOffAll();
    }

    private void TurnOffAll()
    {
        _resultTitle.gameObject.SetActive(false);
        _titleFestival.gameObject.SetActive(false);
        _scoreFestival.gameObject.SetActive(false);
        _descFestival.gameObject.SetActive(false);
        _titleTile.gameObject.SetActive(false);
        _scoreTile.gameObject.SetActive(false);
        _descTile.gameObject.SetActive(false);
        _titleCard.gameObject.SetActive(false);
        _scoreCard.gameObject.SetActive(false);
        _descCard.gameObject.SetActive(false);
        _titleAchievement.gameObject.SetActive(false);
        _scoreAchievement.gameObject.SetActive(false);
        _descAchievement.gameObject.SetActive(false);
        _totalScore.gameObject.SetActive(false);
        _nextDay.gameObject.SetActive(false);
    }

    // 페이드 코루틴
    private IEnumerator Co_FadeTo(float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (duration < 0f)
        {
            // 기본 페이드 시간 적용
            duration = _defaultFadeDuration;
        }

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        _fadeRoutine = StartCoroutine(Co_Fade_Internal(targetAlpha, duration, blockRaycastWhileFading));
        yield return _fadeRoutine;
        _fadeRoutine = null;
    }

    private IEnumerator Co_Fade_Internal(float targetAlpha, float duration, bool blockRaycastWhileFading)
    {
        float startAlpha = _fadeGroup.alpha;
        _fadeGroup.blocksRaycasts = blockRaycastWhileFading;
        _fadeGroup.interactable = false;

        if (duration <= 0f)
        {
            _fadeGroup.alpha = targetAlpha;
            _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            _fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            yield return null;

        }
        _fadeGroup.alpha = targetAlpha;
        _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
    }

}
