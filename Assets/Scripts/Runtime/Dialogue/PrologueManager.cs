using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EDialogBGM
{
    None,
    Stop,
    Change,
}

public class PrologueManager : MonoBehaviour
{
    [Header("프롤로그 / 에필로그 선택")]
    [SerializeField] private bool _isEpilogue = false;

    [Header("사운드")]
    [SerializeField] private SoundManager _soundManager;

    [Header("사운드 패널")]
    [SerializeField] private RectTransform _soundPanelTransform;
    [SerializeField] private Button _soundButtonToggle;
    [SerializeField] private float _soundPanelXOn = -790;
    [SerializeField] private float _soundPanelXOff = -1110;
    [SerializeField] private float _panelMove = 1500;

    [Header("버튼들")]
    [SerializeField] private Button _uiHideButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _sceneSkipButton;
    [SerializeField] private Button _skipButton;

    [Header("UI 패널")]
    [SerializeField] CanvasGroup _uiPanel;
    [Header("사운드 패널")]
    [SerializeField] CanvasGroup _soundPanel;
    [Header("일러스트")]
    [SerializeField] Image _illustPanel;
    [Header("대사창")]
    [SerializeField] RectTransform _dialoguePanel;
    [SerializeField] TMP_Text _dialogueText;
    [Header("화자창")]
    [SerializeField] RectTransform _speakerPanel;
    [SerializeField] TMP_Text _speakerText;

    [Header("일러스트 목록")]
    [SerializeField] List<Sprite> _sprites;

    [Header("대사 목록")]
    [SerializeField] List<PrologueSet> _dialogueDatas;

    private SceneFlowManager _sceneManager;
    private bool _isDialogHide = false;
    private bool _soundUIOn = false;
    private bool _gotoNext = false;
    private int _pageIndex = 0;
    private int _dialogueIndex = 0;

    void Start()
    {
        _sceneManager = SceneFlowManager.Instance;
        #region Button Listener setting
        if (_nextButton != null)
        {
            _nextButton.onClick.AddListener(
                 () => _gotoNext = true);
        }
        if (_skipButton != null)
        {
            _skipButton.onClick.AddListener(
                 () => CallNextScene());
        }
        if (_sceneSkipButton != null)
        {
            _sceneSkipButton.onClick.AddListener(
                 () =>
                 {
                     _pageIndex += 1;
                     if (_pageIndex + 1 == _dialogueDatas.Count)
                     {
                         _sceneSkipButton.interactable = false;
                     }
                     _dialogueIndex = 0;
                     RefreshDialogue();
                 });
        }
        if (_uiHideButton != null)
        {
            _uiHideButton.onClick.AddListener(
                () =>
                {
                    _soundManager.PlaySE(EEffectSound.QuestionChoose);
                    _uiPanel.alpha = 0;
                    _soundPanel.alpha = 0;
                    _isDialogHide = true;
                });
        }
        if (_soundButtonToggle != null)
        {
            _soundButtonToggle.onClick.AddListener(
                () => _soundUIOn = !_soundUIOn);
        }
        #endregion
        RefreshDialogue();
        _uiPanel.alpha = 1;
        if (_sceneManager != null) StartCoroutine(_sceneManager.LoadingScreenOff(1f));
    }

    void Update()
    {
        SoundPanelMove();
        if (_gotoNext == true)
        {
            _gotoNext = false;
            CallNextDialogue();
        }
        if (_isDialogHide)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _soundManager.PlaySE(EEffectSound.QuestionChoose);
                _isDialogHide = false;
                _soundPanel.alpha = 1;
                _uiPanel.alpha = 1;
            }
        }
    }

    private void CallNextDialogue()
    {
        if (_dialogueDatas[_pageIndex].IsLast(_dialogueIndex))
        {
            _pageIndex += 1;
            if(_pageIndex + 1 == _dialogueDatas.Count){
                _sceneSkipButton.interactable = false;
            }
            else if (_pageIndex >= _dialogueDatas.Count)
            {
                CallNextScene();
                return;
            }
            _dialogueIndex = 0;
        }
        else
        {
            _dialogueIndex += 1;
        }
        RefreshDialogue();
    }

    private void CallNextScene()
    {
        if (_sceneManager == null)
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
            return;
        }
        else
        {
            if (_isEpilogue) _sceneManager.LoadScene(ESceneId.Title);
            else _sceneManager.LoadScene(ESceneId.Game);
        }
    }

    private void RefreshDialogue()
    {
        CDialogData targetData = _dialogueDatas[_pageIndex].GetDialogueData(_dialogueIndex);
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
        switch (targetData.BackgroundEffect)
        {
            case EDialogBGM.None:
                break;
            case EDialogBGM.Stop:
                _soundManager.StopBGM();
                break;
            case EDialogBGM.Change:
                _soundManager.PlayBGM(targetData.BackgroundSound);
                break;
        }
        if (targetData.EffectSwitch)
        {
            _soundManager.PlaySE(targetData.EffectSound);
        }
        else
        {
            _soundManager.PlaySE(EEffectSound.QuestionChoose);
        }
    }

    private void SoundPanelMove()
    {
        if (_soundUIOn)
        {
            if (_soundPanelTransform.anchoredPosition3D.x < _soundPanelXOn)
            {
                _soundPanelTransform.anchoredPosition3D += Vector3.right * _panelMove * 2 * Time.deltaTime;
                if (_soundPanelTransform.anchoredPosition3D.x >= _soundPanelXOn)
                {
                    _soundPanelTransform.anchoredPosition3D = new Vector3(_soundPanelXOn, 486, 0);
                }
            }
        }
        else
        {
            if (_soundPanelTransform.anchoredPosition3D.x > _soundPanelXOff)
            {
                _soundPanelTransform.anchoredPosition3D -= Vector3.right * _panelMove * 2 * Time.deltaTime;
                if (_soundPanelTransform.anchoredPosition3D.x <= _soundPanelXOff)
                {
                    _soundPanelTransform.anchoredPosition3D = new Vector3(_soundPanelXOff, 486, 0);
                }
            }
        }
    }
}
