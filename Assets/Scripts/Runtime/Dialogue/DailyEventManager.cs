using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyEventManager : MonoBehaviour
{
    [Header("게임 매니저")]
    [SerializeField] private GameManager _gameManager;
    [Header("사운드")]
    [SerializeField] private SoundManager _soundManager;
    [Header("일일 이벤트 캔버스")]
    [SerializeField] private Canvas _canvas;

    [Header("버튼들")]
    [SerializeField] private Button _uiHideButton;
    [SerializeField] private Button _nextButton;

    [Header("UI 패널")]
    [SerializeField] CanvasGroup _uiPanel;
    [Header("일러스트")]
    [SerializeField] Image _illustPanel;
    [Header("대사창")]
    [SerializeField] RectTransform _dialoguePanel;
    [SerializeField] TMP_Text _dialogueText;
    [Header("화자창")]
    [SerializeField] RectTransform _speakerPanel;
    [SerializeField] TMP_Text _speakerText;

    //[Header("대답버튼")]
    //[SerializeField] Button _answer01;
    //[SerializeField] OnMouseTooltip _tooltip01;
    //[SerializeField] Button _answer02;
    //[SerializeField] OnMouseTooltip _tooltip02;
    //[SerializeField] Button _answer03;
    //[SerializeField] OnMouseTooltip _tooltip03;

    [Header("일러스트 목록")]
    [SerializeField] List<Sprite> _sprites;

    [Header("대사 목록")]
    [SerializeField] List<DailyEventSet> _dialogueDatas;

    private SceneFlowManager _sceneManager;
    private bool _isEnable = false;
    private bool _isDialogHide = false;
    private bool _gotoNext = false;
    private int _pageIndex = 0;
    private int _dialogueIndex = 0;
    private bool _isQuestion;
    private int _arg = 0;
    private int _questionResult;

    void Start()
    {
        #region Button Listener setting
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
        #endregion
    }

    void Update()
    {
        if (!_isEnable) return;
        if (_gotoNext == true)
        {
            _gotoNext = false;
            if (!_isQuestion)
            {
                CallNextDialogue();
            }
            else
            {
                _gameManager.CreateError("선택지 상태!", true);
            }
        }
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

    private void CallNextDialogue()
    {
        if (_dialogueDatas[_pageIndex].IsLast(_dialogueIndex))
        {
            GoToGame(_arg);
            return;
        }
        else
        {
            _dialogueIndex += 1;
        }
        RefreshDialogue();
    }

    private void RefreshDialogue()
    {
        Logger.Log($"대화 불러오기 : {_pageIndex} - {_dialogueIndex}");
        CDailyEventData targetData = _dialogueDatas[_pageIndex].GetDialogueData(_dialogueIndex);
        if (string.IsNullOrEmpty(targetData.SpeakerName))
        {
            _speakerPanel.gameObject.SetActive(false);
        }
        else
        {
            _speakerText.text = targetData.SpeakerName;
            _speakerPanel.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(targetData.Dialogue))
        {
            _dialogueText.text = targetData.Dialogue;
        }
        if (targetData.IllustHide)
        {
            _illustPanel.gameObject.SetActive(false);
        }
        else if (targetData.IllustChange)
        {
            _illustPanel.sprite = _sprites[targetData.IllustIndex];
            _illustPanel.gameObject.SetActive(true);
        }
        _soundManager.PlaySE(EEffectSound.QuestionChoose);
    }

    private void GoToDialogue(int page, int dialogue)
    {
        _pageIndex = page;
        _dialogueIndex = dialogue;
        RefreshDialogue();
    }

    private void GoToGame(int arg)
    {
        _gameManager.DailyEventArg = arg;
        _isEnable = false;
        _speakerPanel.gameObject.SetActive(false);
        _illustPanel.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
        StartCoroutine(_gameManager.EndDailyEvent());
    }

    public void StartEvent(int pageIndex, int dialogueIndex)
    {
        _isEnable = true;
        _pageIndex = pageIndex;
        _dialogueIndex = dialogueIndex;
        _canvas.gameObject.SetActive(true);
        RefreshDialogue();
    }
}
