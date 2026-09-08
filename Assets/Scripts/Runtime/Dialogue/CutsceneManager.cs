using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class CCutsceneData
{
    public string Title;
    public Sprite Mark;
    public Sprite Illust;
    [TextArea(2, 2)] public string Description;
}


public class CutsceneManager : MonoBehaviour
{
    [Header("매니저")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SoundManager _soundManager;

    [Header("요소")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _backPanel;
    [SerializeField] private CanvasGroup _uiPanel;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _illust;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Header("컷씬 목록")]
    [SerializeField] List<CCutsceneData> _cutsceneDataset;

    [Header("버튼들")]
    [SerializeField] private Button _uiHideButton;
    [SerializeField] private Button _nextButton;

    private bool _gotoNext = false;
    private bool _isDialogHide = false;

    void Start()
    {
        if (_nextButton != null)
        {
            _nextButton.onClick.AddListener(
                 () => _gotoNext = true);
        }
        if (_uiHideButton != null)
        {
            _uiHideButton.onClick.AddListener(
                () =>
                {
                    _soundManager.PlaySE(EEffectSound.QuestionChoose);
                    _uiPanel.alpha = 0;
                    _isDialogHide = true;
                });
        }
    }

    private void Update()
    {
        if (_isDialogHide)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _soundManager.PlaySE(EEffectSound.QuestionChoose);
                _isDialogHide = false;
                _uiPanel.alpha = 1;
            }
        }
    }

    public IEnumerator CutscenePlay(int index)
    {
        _soundManager.PlayBGM(EBackgroundSound.CardIllust);
        CCutsceneData data = _cutsceneDataset[index];
        Color color;
        _icon.sprite = data.Mark;
        yield return null;
        _illust.sprite = data.Illust;
        yield return null;
        _title.text = data.Title;
        _description.text = data.Description;
        _backPanel.alpha = 0f;
        _uiPanel.alpha = 0f;
        color = _icon.color;
        color.a = 0f;
        _icon.color = color;
        color = _illust.color;
        color.a = 0f;
        _illust.color = color;
        color = _title.color;
        color.a = 0f;
        _title.color = color;
        _nextButton.gameObject.SetActive(false);
        _uiHideButton.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(true);
        _backPanel.blocksRaycasts = true;
        yield return StartCoroutine(FadeTo_Group(_backPanel, 1f, _fadeDuration * 2, true));
        yield return StartCoroutine(FadeTo_Image(_icon, 1f, _fadeDuration, true));
        yield return StartCoroutine(FadeTo_Text(_title, 1f, _fadeDuration, true));
        yield return new WaitForSecondsRealtime(_fadeDuration * 2);
        StartCoroutine(FadeTo_Image(_icon, 0f, _fadeDuration, true));
        yield return StartCoroutine(FadeTo_Text(_title, 0f, _fadeDuration, true));
        yield return StartCoroutine(FadeTo_Image(_illust, 1f, _fadeDuration, true));
        yield return StartCoroutine(FadeTo_Group(_uiPanel, 1f, _fadeDuration, true));
        yield return new WaitForSecondsRealtime(_fadeDuration * 2);
        _nextButton.gameObject.SetActive(true);
        _uiHideButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => _gotoNext);
        _gotoNext = false;
        _backPanel.alpha = 0f;
        _nextButton.gameObject.SetActive(false);
        _uiHideButton.gameObject.SetActive(false);
        StartCoroutine(FadeTo_Group(_uiPanel, 0f, _fadeDuration, true));
        yield return StartCoroutine(FadeTo_Image(_illust, 0f, _fadeDuration, true));
        _backPanel.blocksRaycasts = false;
        _canvas.gameObject.SetActive(false);
        _gameManager.HideIllust();

    }
    private IEnumerator FadeTo_Group(CanvasGroup target, float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (target == null)
        {
            yield break;
        }
        if (duration < 0f)
        {
            duration = _fadeDuration;
        }

        yield return StartCoroutine(FadeInternal_Group(target, targetAlpha, duration, blockRaycastWhileFading)); ;
    }
    private IEnumerator FadeInternal_Group(CanvasGroup target, float targetAlpha, float duration, bool blockRaycastWhileFading)
    {
        float startAlpha = target.alpha;

        if (duration <= 0f)
        {
            target.alpha = targetAlpha;
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            float lerp = Mathf.Clamp01(t / duration);
            target.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            yield return null;
        }
        target.alpha = targetAlpha;
    }
    private IEnumerator FadeTo_Image(Image target, float targetAlpha, float duration = -1f, bool blockRaycastWhileFading = true)
    {
        if (target == null)
        {
            yield break;
        }
        if (duration < 0f)
        {
            duration = _fadeDuration;
        }

        yield return StartCoroutine(FadeInternal_Image(target, targetAlpha, duration, blockRaycastWhileFading));
    }
    private IEnumerator FadeInternal_Image(Image target, float targetAlpha, float duration, bool blockRaycastWhileFading)
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
