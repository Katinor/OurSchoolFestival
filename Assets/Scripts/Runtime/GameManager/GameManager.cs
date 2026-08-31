using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum ETileCatalog
{
    Basement,
    RoadBase,
    RoadBuilt,
    Trees,
    Booth,
    Foodbooth,
    FestivalHQ
}

public enum EGameState
{
    Idle,
    TileInspect,
    MaterialCount,
    TileSelect,
    Question,
    NextDay
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

    [Header("핸드추가 (디버그용)")]
    [SerializeField] Button _cardAddButton;

    [Header("에러 프리팹")]
    [SerializeField] GameObject _errorPrefab;

    [Header("카메라")]
    [SerializeField] Camera _mainCamera;

    [Header("선택지 등장")]
    [SerializeField] AudioSource _onQuestionSe;

    [Header("일반 사운드")]
    [SerializeField] AudioSource _onChooseSe;

    [Header("업그레이드")]
    [SerializeField] AudioSource _upgradeSe;

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
    [SerializeField] private TMP_Text _materialsCurrentText;
    [SerializeField] private TMP_Text _materialsIncreaseText;
    [SerializeField] private TMP_Text _menpowerCurrentText;
    [SerializeField] private TMP_Text _menpowerIncreaseText;
    [SerializeField] private Slider _menpowerRamainsSlider;
    [SerializeField] private TMP_Text _techText;
    [SerializeField] private Button _debugNextDay;

    [Header("지표메뉴")]
    [SerializeField] private TMP_Text _successText;
    [SerializeField] private TMP_Text _interestText;
    [SerializeField] private TMP_Text _roadText;

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
    #endregion

    #region Member Variable
    private Tilemap _tilemap;
    private Color _lastColor;
    private GameObject _lastObject;
    private GameObject _lastSelectedObject;
    private Vector3Int _lastSelectedPosition;
    private LayerMask _hitMask = 0;
    private EGameState _gameState = EGameState.Idle;

    private CResources _resources;
    private Dictionary<ETech, int> _currentTech;

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
    private int _usingMatCount = 0;

    private bool _rightUIOn = false;
    private bool _leftUIOn = false;
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

    void Start()
    {
        _resources = new CResources(50, 20, 0, 1, 0, 1);
        _hitMask |= LayerMask.GetMask("Tilemap");
        _tilemap = this.GetComponentInChildren<Tilemap>();
        CPrint.Log($"{_hitMask.value} = (Tilemap)");
        _rightPanelTransform.anchoredPosition3D = new Vector3(_rightPanelXOff, 0, 0);
        _upgradeButton.gameObject.SetActive(false);
        _currentTech = new Dictionary<ETech, int>();
        SetListener();
        DrawMapTiles();
    }

    void Update()
    {
        switch (_gameState)
        {
            case EGameState.Idle:
            case EGameState.TileInspect:
                UpdateIdle();
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
        }

        LeftPanelMove();
        RightPanelMove();
        ResourceSync();
    }
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
                CPrint.Log("마테리얼을 못쓰는데 쌈");
                canUse = false;
                log += "자본, ";
            }
            else if ((_resources.moneyCurrent + 2 * (_resources.materialsCurrent) < moneyCurrent))
            {
                CPrint.Log($"{_resources.moneyCurrent + 2 * (_resources.materialsCurrent)} < {moneyCurrent} : 자재써도 불가 ");
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
            CPrint.Warn($"자원 부족 : {log}");
            return false;
        }
    }

    public bool CheckResource(SCost cost)
    {
        return CheckResource(cost.moneyCurrent, cost.moneyIncrease, cost.materialsCurrent,
            cost.materialsIncrease, cost.menpowerCurrent, cost.menpowerIncrease, cost.canUseMaterials);
    }

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
                return "<sprite=9>";
            case ETech.Music:
                return "<sprite=10>";
            case ETech.Art:
                return "<sprite=11>";
            case ETech.Exercise:
                return "<sprite=12>";
            case ETech.Cult:
                return "<sprite=13>";
            default:
                return "";
        }
    }
}
