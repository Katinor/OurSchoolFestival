using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum ETileCatalog
{
    Basement,
    RoadBase,
    RoadBuilt,
    Trees,
    Booth,
    Foodbooth,
    FestivalHQ,
    CoffeeBooth,
    LaboBooth,
    InfoLab,
    ArtistMasterpiece,
    ArtMuseum,
    CultBooth,
    BuskingBooth,
    SoundRelay,
    InformationCenter,
    SportsHQ,
    SportsAttraction,
    ElectronicSystem
}

public enum EGameState
{
    Idle,
    TileInspect,
    MaterialCount,
    TileSelect,
    Question,
    NextDay,
    LastDayIdle,
    LastDayTileInspect,
    DailyEvent,
    NoInput,
}
[Flags]
public enum EGameAchievement
{
    None = 0,
    GameClear = 1 << 0,
    FoodMaster = 1 << 1,
    NatureMaster = 1 << 2,
    BrainMaster = 1 << 3,
    ScienceMaster = 1 << 4,
    MusicMaster = 1 << 5,
    ArtMaster = 1 << 6,
    SportsMaster = 1 << 7
}

public partial class GameManager : MonoBehaviour
{
    #region Inspector
    [Header("그리드")]
    [SerializeField] Grid _grid;

    [Header("캔버스")]
    [SerializeField] Canvas _canvas;

    [Header("에러 캔버스")]
    [SerializeField] Canvas _errorCanvas;

    [Header("카드핸드")]
    [SerializeField] CHand _cardHand;

    [Header("사운드 매니저")]
    [SerializeField] SoundManager _soundManager;

    [Header("카메라")]
    [SerializeField] CameraManager _camera;

    [Header("일일 이벤트 매니저")]
    [SerializeField] DailyEventManager _dailyManager;

    [Header("컷씬 매니저")]
    [SerializeField] CutsceneManager _cutsceneManager;

    [Header("핸드추가 (디버그용)")]
    [SerializeField] Button _cardAddButton;

    [Header("에러 프리팹")]
    [SerializeField] GameObject _errorPrefab;

    [Header("카메라")]
    [SerializeField] Camera _mainCamera;

    [Header("기본 타일")]
    [SerializeField] private List<TileBase> _tileBases;    

    [Header("좌표계")]
    [SerializeField] private int _gridSizeXRight = 4;
    [SerializeField] private int _gridSizeYUpper = 3;
    [SerializeField] private int _roadSize = 8;

    [Header("하단 텍스트")]
    [SerializeField] private TMP_Text _undertext;

    [Header("우측메뉴")]
    [SerializeField] private RectTransform _rightPanelTransform;
    [SerializeField] private TMP_Text _tileName;
    [SerializeField] private TMP_Text _tileDescription;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TMP_Text _tileUpgradeCost;
    [SerializeField] private Button _tileActionButton;
    [SerializeField] private TMP_Text _tileActionCost;
    [SerializeField] private TMP_Text _tileActionMessage;
    [SerializeField] private float _rightPanelXOn = 640;
    [SerializeField] private float _rightPanelXOff = 1180;
    [SerializeField] private float _panelMove = 1500;

    [Header("좌측메뉴")]
    [SerializeField] RectTransform _leftPanelTransform;
    [SerializeField] private Button _leftButtonToggle;
    [SerializeField] private Button _leftButton1;
    [SerializeField] private Button _leftButton2;
    [SerializeField] private Button _leftButton3;
    [SerializeField] private Button _leftButton4;
    [SerializeField] private Button _leftButton5;
    [SerializeField] private float _leftPanelXOn = -870;
    [SerializeField] private float _leftPanelXOff = -1030;

    [Header("상단메뉴")]
    [SerializeField] private TMP_Text _moneyCurrentText;
    [SerializeField] private TMP_Text _moneyIncreaseText;
    [SerializeField] private TMP_Text _moneyIncreaseText2;
    [SerializeField] private TMP_Text _materialsCurrentText;
    [SerializeField] private TMP_Text _materialsIncreaseText;
    [SerializeField] private TMP_Text _menpowerCurrentText;
    [SerializeField] private TMP_Text _menpowerIncreaseText;
    [SerializeField] private Slider _menpowerRamainsSlider;
    [SerializeField] private TMP_Text _techText;
    [SerializeField] private Button _nextDayButton;
    [SerializeField] private TMP_Text _nextDayText;
    [SerializeField] private Button _titleButton;
    [SerializeField] private Button _undoButton;
    [SerializeField] private TMP_Text _undoText;
    [SerializeField] private OnMouseTooltip _undoTooltip;

    [Header("지표메뉴")]
    [SerializeField] private TMP_Text _successText;
    [SerializeField] private TMP_Text _interestText;
    [SerializeField] private TMP_Text _roadText;
    [SerializeField] private TMP_Text _handText;

    [Header("물음메뉴")]
    [SerializeField] private RectTransform _questionPanel;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private Button _questionYes;
    [SerializeField] private Button _questionNo;

    [Header("자재물음메뉴")]
    [SerializeField] private RectTransform _materialsPanel;
    [SerializeField] private TMP_Text _materialsCount;
    [SerializeField] private TMP_Text _materialsText;
    [SerializeField] private Button _materialsYes;
    [SerializeField] private Button _materialsNo;
    [SerializeField] private Button _materialsUp;
    [SerializeField] private Button _materialsDown;

    [Header("사운드 패널")]
    [SerializeField] private RectTransform _soundPanelTransform;
    [SerializeField] private Button _soundButtonToggle;
    [SerializeField] private float _soundPanelXOn = -790;
    [SerializeField] private float _soundPanelXOff = -1110;

    [Header("턴 화면")]
    [SerializeField] private DayResultManager _DayManager;
    [SerializeField] private TMP_Text _dailyOpeningText;
    [SerializeField] private TMP_Text _dailyTitleText;
    #endregion

    #region Member Variable
    private SceneFlowManager _sceneManager;
    private int _version = SaveManager.Version;
    private int _saveSlot = 0;
    private int _randomSeed;
    private Tilemap _tilemap;
    private int _currentDay = 0;
    private Color _lastColor;
    private List<Vector3Int> _lastNearObject;
    private Vector3Int? _lastSelectedPosition;
    private LayerMask _hitMask = 0;
    private EGameState _gameState = EGameState.NoInput;

    private CResources _resources;
    private Dictionary<ETech, int> _currentTech;
    private List<Func<GameManager, SScoreInfo>> _cardScores;
    private List<int> _cardScoresList;
    private SScoreSet _scoreSet;
    private EGameAchievement _achievement = EGameAchievement.None;
    private int _scoreTotal;
    private Stack<CUndoData> _undoDataList;

    private int _questionValue = 0;
    private Action<GameObject, Vector3Int> _questionAction = null;
    private string _questionString = null;
    private GameObject _questionArgGO = null;
    private Vector3Int _questionArgVector = Vector3Int.zero;
    private ETileState _questionMask;
    private ETileState _questionMaskReverse;

    private Action<CCard, Vector3Int> _questionCard = null;
    private CCard _questionArgCard = null;
    private bool _questionIsCard = false;
    private bool _questionIsTileSkiped = false;
    private int _questionTileRadius = 0;
    private int _usingMatCount = 0;

    private bool _rightUIOn = false;
    private bool _leftUIOn = false;
    private bool _soundUIOn = false;

    private int _dailyEventArg = -1;
    private bool _epilogueFlag = false;
    private float _fadeDuration = 1f;

    public readonly int SuccessMax = 18;
    public readonly int InterestMax = 19;
    public readonly int RoadMax = 8;
    #endregion

    public Grid GameGrid
    {
        get { return _grid; }
        protected set { _grid = value; }
    }
    public CResources Resources
    {
        get { return _resources; }
        set { _resources = value; }
    }

    public Dictionary<ETech, int> CurrentTech
    {
        get { return _currentTech; }
        set { _currentTech = value; }
    }
    
    public EGameState GameState
    {
        get { return _gameState; }
        protected set { _gameState = value; }
    }
    public int CurrentDay
    {
        get { return _currentDay; }
        protected set { _currentDay = value; }
    }

    public int DailyEventArg
    {
        get { return _dailyEventArg; }
        set { _dailyEventArg = value; }
    }

    public bool EpilogueFlag
    {
        get { return _epilogueFlag; }
        set { _epilogueFlag = value; }
    }

    void Start()
    {
        _tilemap = this.GetComponentInChildren<Tilemap>();
        _gameState = EGameState.NoInput;
        StartCoroutine(StateShow());
        if (SceneFlowManager.Instance != null)
        {
            _sceneManager = SceneFlowManager.Instance;
            _saveSlot = _sceneManager.TargetSaveData;
            (float bgmLevel, float seLevel) = _sceneManager.KeepVolume;
            _soundManager.SetVolumeForce(bgmLevel, seLevel);
        }
        if (_tileBases == null)
        {
            Logger.Error("타일 목록이 설정되지 않음");
            enabled = false;
        }
        _hitMask |= LayerMask.GetMask("Tilemap");
        _rightPanelTransform.anchoredPosition3D = new Vector3(_rightPanelXOff, 0, 0);
        _currentTech = new Dictionary<ETech, int>();
        _cardScores = new List<Func<GameManager, SScoreInfo>>();
        _cardScoresList = new List<int>();
        _scoreSet = new SScoreSet();
        _resources = new CResources(200, 50, 1, 1, 1, 1);
        _undoDataList = new Stack<CUndoData>();
        _undoButton.interactable = false;
        _undoText.gameObject.SetActive(false);
        _undoText.text = "";
        _undoTooltip.SetText("");
        _dailyOpeningText.gameObject.SetActive(false);
        _dailyTitleText.gameObject.SetActive(false);
        SetListener();
        DrawMapTiles();
        if (_sceneManager != null && _sceneManager.LoadSavedData)
        {
            _saveSlot = _sceneManager.TargetSaveData;
            if (!LoadData())
            {
                Logger.Error("로드 실패. 타이틀로 돌아감!");
                _sceneManager.LoadScene(ESceneId.Title);
                return;
            }
        }
        else
        {
            UnityEngine.Random.InitState(System.Guid.NewGuid().GetHashCode());
            _randomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(_randomSeed);
            _currentDay = 1;
            SetDayButton(_currentDay);
            _cardHand.CardPositionReset();
        }
        if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
#if UNITY_EDITOR
        StartCoroutine(StartManager());
#endif
    }

    void Update()
    {
        switch (_gameState)
        {
            case EGameState.Idle:
            case EGameState.TileInspect:
                UpdateIdle();
                break;
            case EGameState.LastDayIdle:
            case EGameState.LastDayTileInspect:
                UpdateLastIdle();
                break;
            case EGameState.MaterialCount:
                UpdateMaterialCheck();
                break;
            case EGameState.TileSelect:
                UpdateTileSelect();
                break;
            case EGameState.Question:
                UpdateQuestionSelect();
                break;
            case EGameState.DailyEvent:
                // if (DailyEventArg != -1) StartCoroutine(EndDailyEvent());
                break;
        }

        LeftPanelMove();
        RightPanelMove();
        SoundPanelMove();
        ResourceSync();
    }

    private IEnumerator StartManager()
    {
        if (_sceneManager != null) yield return StartCoroutine(_sceneManager.LoadingScreenOff(2f, 1f));
        if (_currentDay >= 16)
        {
            _gameState = EGameState.LastDayIdle;
        }
        else
        {
            _gameState = EGameState.DailyEvent;
            StartCoroutine(StartDailyEvent());
        }
    }
    /// <summary>
    /// 현재 사용가능한지를 확인해줍니다.
    /// </summary>
    public bool CheckResource(int moneyCurrent, int moneyIncrease,
        int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease, bool canUseMaterials = false)
    {
        bool canUse = true;
        string log = "";
        if (_resources.moneyCurrent < moneyCurrent)
        {
            //구현 후 사용
            if (!canUseMaterials)
            {
                Logger.Log("마테리얼을 못쓰는데 쌈");
                canUse = false;
                log += "자본, ";
            }
            else if ((_resources.moneyCurrent + 2 * (_resources.materialsCurrent) < moneyCurrent))
            {
                Logger.Log($"{_resources.moneyCurrent + 2 * (_resources.materialsCurrent)} < {moneyCurrent} : 자재써도 불가 ");
                canUse = false;
                log += "자본, ";
            }   
        }
        if (_resources.moneyIncrease < moneyIncrease)
        {
            canUse = false;
            log += "자본+, ";
        }
        if (_resources.materialsCurrent < materialsCurrent)
        {
            canUse = false;
            log += "자재, ";
        }
        if (_resources.materialsIncrease <  materialsIncrease)
        {
            canUse = false;
            log += "자재+, ";
        }
        if (_resources.menpowerCurrent < menpowerCurrent)
        {
            canUse = false;
            log += "인력, ";
        }
        if (_resources.menpowerIncrease < menpowerIncrease)
        {
            canUse = false;
            log += "인력+, ";
        }
        if (canUse)
        {
            return true;
        }
        else
        {
            Logger.Warn($"자원 부족 : {log}");
            return false;
        }
    }
    /// <summary>
    /// 현재 사용가능한지를 확인해줍니다.
    /// </summary>
    public bool CheckResource(SCost cost)
    {
        return CheckResource(cost.moneyCurrent, cost.moneyIncrease, cost.materialsCurrent,
            cost.materialsIncrease, cost.menpowerCurrent, cost.menpowerIncrease, cost.canUseMaterials);
    }
    /// <summary>
    /// 현재 기술상태를 다시 가져옵니다.
    /// </summary>
    public void ReloadTech()
    {
        _techText.text = "";
        foreach(KeyValuePair<ETech, int> target in _currentTech)
        {
            for (int i = 0; i < target.Value; i++)
            {
                _techText.text += TechParserForReload(target.Key);
            }
        }
    }

    private string TechParserForReload(ETech tech)
    {
        switch (tech)
        {
            case ETech.Science:
                return "<sprite=10>";
            case ETech.Music:
                return "<sprite=11>";
            case ETech.Art:
                return "<sprite=12>";
            case ETech.Sports:
                return "<sprite=13>";
            case ETech.Cult:
                return "<sprite=14>";
            default:
                return "";
        }
    }
    /// <summary>
    /// 카드에 있는 점수 대리자를 추가합니다.
    /// </summary>
    public void AddScoreAction(int level)
    {
        _cardScores.Add(CCardStatic.FindPointFunction(level));
        _cardScoresList.Add(level);
    }
    /// <summary>
    /// 비용을 지불합니다.
    /// </summary>
    public void PayCost(SCost cost)
    {
        _resources.PayCost(cost);
    }
    /// <summary>
    /// 카드를 추가하는데에 사용합니다.
    /// create를 키면 덱에서 가져오지 않고, 생성합니다.
    /// isTop을 키면 왼손으로 들고옵니다.
    /// </summary>
    public void GetCard(int cardId, bool create = true, bool isTop = true)
    {
        _cardHand.AddCard(cardId, create, false, isTop);
        _soundManager.PlaySE(EEffectSound.CardDraw);
    }
    /// <summary>
    /// 덱에 카드를 추가합니다.
    /// </summary>
    public void AddDeck(List<int> deckIds)
    {
        _cardHand.AddCardInDeck(deckIds);
    }
    /// <summary>
    /// 일반 덱에서 카드를 뽑습니다.
    /// </summary>
    public void DrawCards(int cardCount)
    {
        _cardHand.AddCards(cardCount);
    }
    /// <summary>
    /// 현재 특정 수만큼 카드를 뽑을 수 있는지 확인합니다.
    /// </summary>
    public bool CanDraw(int cardCount)
    {
        return _cardHand.CanDraw(cardCount);
    }
    /// <summary>
    /// 일러스트를 보여줍니다.
    /// </summary>
    public void ShowIllust(int index)
    {
        OnClickElse();
        _gameState = EGameState.NoInput;
        StartCoroutine(_cutsceneManager.CutscenePlay(index));
    }
    /// <summary>
    /// 일러스트를 숨깁니다.
    /// </summary>
    public void HideIllust()
    {
        if (_currentDay < 6) _soundManager.PlayBGM(EBackgroundSound.Part1);
        else if (_currentDay < 11) _soundManager.PlayBGM(EBackgroundSound.Part2);
        else if (_currentDay == 16) _soundManager.PlayBGM(EBackgroundSound.Result);
        else _soundManager.PlayBGM(EBackgroundSound.Part3);
        _gameState = EGameState.Idle;
    }
    /// <summary>
    /// 특정 기술의 레벨을 반환합니다.
    /// </summary>
    public int GetTech(ETech target)
    {
        if (_currentTech.ContainsKey(target)) return _currentTech[target];
        else return 0;
    }
    /// <summary>
    /// 피노 덱에서 카드를 뽑습니다.
    /// </summary>
    public void GetMysteryCard()
    {
        _cardHand.AddMysteryCard();
    }
    /// <summary>
    /// 피노 덱에 카드가 남아있는지 보여줍니다.
    /// </summary>
    public bool MysteryAvailable()
    {
        return _cardHand.MysteryAvailable();
    }
}
