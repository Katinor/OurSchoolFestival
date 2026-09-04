using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ETitleState
{
    Title = 0,
    Save = 1,
    Question = 2,
    Exit = 3,
}

public class TItleManager : MonoBehaviour
{
    [Header("사운드")]
    [SerializeField] private SoundManager _soundManager;

    [Header("첫 타이틀 버튼")]
    [SerializeField] private Button _gameStart;
    [SerializeField] private Button _gameExit;

    [Header("타이틀 캔버스")]
    [SerializeField] private Canvas _titleCanvas;

    [Header("세이브 슬롯")]
    [SerializeField] private Canvas _saveCanvas;
    [SerializeField] private Button _titleButton;
    [SerializeField] private List<SaveSlotUI> _saveSlots;

    [Header("물음메뉴")]
    [SerializeField] private RectTransform _questionPanel;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private Button _questionYes;
    [SerializeField] private Button _questionNo;

    [Header("사운드 패널")]
    [SerializeField] private RectTransform _soundPanelTransform;
    [SerializeField] private Button _soundButtonToggle;
    [SerializeField] private float _soundPanelXOn = -790;
    [SerializeField] private float _soundPanelXOff = -1110;
    [SerializeField] private float _panelMove = 1500;

    private SceneFlowManager _sceneManager;
    private ETitleState _titleState;
    private int _targetSlot = -1;
    private bool _useSaveData = false;
    private Action _questionAction;
    private int _questionValue = 0;
    private bool _soundUIOn = false;

    void Start()
    {
        _sceneManager = SceneFlowManager.Instance;
        _sceneManager.LoadingScreenOn(1f, false);
        _titleCanvas.gameObject.SetActive(true);
        _saveCanvas.gameObject.SetActive(false);
        if (_saveSlots.Count != SaveManager.MaxSaveSlot)
        {
            Logger.Error("세이브 슬롯 수 안맞음");
            enabled = false;
            return;
        }
        SaveManager.RefreshAllData();
        for(int i = 0; i < _saveSlots.Count; i++)
        {
            if (SaveManager.Available(i)) _saveSlots[i].LoadSavedata(SaveManager.LoadData(i));
            else _saveSlots[i].ResetSavedata();
        }

        #region Button Listener setting
        if (_gameStart != null)
        {
            _gameStart.onClick.AddListener(
                () => CallGameStart());
        }
        if (_gameExit != null)
        {
            _gameExit.onClick.AddListener(
                () => CallGameExit());
        }
        if (_titleButton != null)
        {
            _titleButton.onClick.AddListener(
                () => CallGotoTitle());
        }
        if (_soundButtonToggle != null)
        {
            _soundButtonToggle.onClick.AddListener(
                () => CallSoundToggle());
        }
        if (_questionYes != null)
        {
            _questionYes.onClick.AddListener(
                () => _questionValue = 1);
        }
        if (_questionNo != null)
        {
            _questionNo.onClick.AddListener(
                () => _questionValue = -1);
        }
        #endregion

        (float bgmLevel, float seLevel) = _sceneManager.KeepVolume;
        _soundManager.SetVolumeForce(bgmLevel, seLevel);
        _soundManager.PlayBGM(EBackgroundSound.Title);
        StartCoroutine(_sceneManager.LoadingScreenOff());
        _titleState = ETitleState.Title;
    }

    void Update()
    {
        SoundPanelMove();
        switch (_titleState)
        {
            case ETitleState.Title:
                return;
            case ETitleState.Save:
                for (int i = 0; i < _saveSlots.Count; i++)
                {
                    if (_saveSlots[i].Input == 1)
                    {
                        ResetInput();
                        _targetSlot = i;
                        _saveSlots[i].Input = 0;
                        if (SaveManager.Available(i))
                        {
                            _useSaveData = true;
                            ShowQuestion($"{i + 1}번 데이터를 불러옵니까?", CallGameScene);
                        }
                        else
                        {
                            _useSaveData = false;
                            ShowQuestion($"{i + 1}번 데이터에 새 게임을 시작합니까?", CallGameScene);
                        }
                    }
                    else if (_saveSlots[i].Input == -1)
                    {
                        ResetInput();
                        _targetSlot = i;
                        _useSaveData = true;
                        ShowQuestion($"{i + 1}번 데이터를 삭제합니까?", CallSlotDelete);
                    }
                }
                return;
            case ETitleState.Question:
            case ETitleState.Exit:
                if (Input.GetMouseButtonDown(1))
                {
                    _questionNo.onClick.Invoke();
                }
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                {
                    _questionYes.onClick.Invoke();
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    _questionNo.onClick.Invoke();
                }
                if (_questionValue == 0)
                {
                    return;
                }
                else if (_questionValue == 1)
                {
                    _questionAction();
                }
                else
                {
                    _soundManager.PlaySE(EEffectSound.Beep);
                }
                HideQuestion();
                return;
        }
    }

    private void CallGameExit()
    {
        ShowQuestion
            (
                "게임을 종료하시겠습니까?",
                ActionGameExit
            );
    }
    private void ActionGameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void CallGameStart()
    {
        _soundManager.PlaySE(EEffectSound.QuestionAppear);
        _saveCanvas.gameObject.SetActive(true);
        _titleCanvas.gameObject.SetActive(false);
        _titleState = ETitleState.Save;
    }

    private void CallGameScene()
    {
        _sceneManager.TargetSaveData = _targetSlot;
        _sceneManager.LoadSavedData = _useSaveData;
        (float bgmLevel, float seLevel) = _soundManager.GetVolume();
        _sceneManager.KeepVolume = (bgmLevel, seLevel);
        _titleState = ETitleState.Title;
        _sceneManager.LoadScene(ESceneId.Game);
    }

    private void CallGotoTitle()
    {
        _soundManager.PlaySE(EEffectSound.QuestionAppear);
        _titleCanvas.gameObject.SetActive(true);
        _saveCanvas.gameObject.SetActive(false);
        _titleState = ETitleState.Title;
    }

    private void ShowQuestion(string shownText, Action action)
    {
        if (_titleState == ETitleState.Question || _titleState == ETitleState.Exit)
        {
            HideQuestion();
        }
        _soundManager.PlaySE(EEffectSound.QuestionAppear);
        _questionPanel.gameObject.SetActive(true);
        _questionText.text = shownText;
        _questionAction = action;
        if (_titleState == ETitleState.Title)
        {
            _titleState = ETitleState.Exit;
        }
        else if (_titleState == ETitleState.Save)
        {
            _titleState = ETitleState.Question;
        }
    }

    private void HideQuestion()
    {
        _questionPanel.gameObject.SetActive(false);
        _questionAction = null;
        _questionText.text = "";
        _questionValue = 0;
        if (_titleState == ETitleState.Exit)
        {
            _titleState = ETitleState.Title;
        }
        else if (_titleState == ETitleState.Question)
        {
            _titleState = ETitleState.Save;
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
    private void CallSoundToggle()
    {
        _soundUIOn = !_soundUIOn;
    }
    private void CallSlotDelete()
    {
        SaveManager.DeleteData(_targetSlot);
        _saveSlots[_targetSlot].ResetSavedata();
        SaveManager.RefreshAllData();

    }

    private void ResetInput()
    {
        for (int i = 0; i < _saveSlots.Count; i++)
        {
            _saveSlots[i].Input = 0;
        }
    }
}
