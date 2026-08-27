using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum ETempTileCatalog
{
    Basement,
    RoadBase,
    RoadBuilt,
    Trees
}

public enum EGameState
{
    Idle,
    TileInspect,
    TileSelect,
    Question,
}

public partial class Test_TilemapSelector : MonoBehaviour
{
    #region Inspector
    [Header("그리드")]
    [SerializeField] Grid _grid;

    [Header("캔버스")]
    [SerializeField] Canvas _canvas;

    [Header("에러 캔버스")]
    [SerializeField] Canvas _errorCanvas;

    [Header("카드캔버스")]
    [SerializeField] Canvas _cardCanvas;

    [Header("에러 프리팹")]
    [SerializeField] GameObject _errorPrefab;

    [Header("카메라")]
    [SerializeField] Camera _mainCamera;

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

    private Action<CCard> _questionCard = null;
    private CCard _questionArgCard = null;
    private bool _questionIsCard = false;

    private bool _rightUIOn = false;
    private bool _leftUIOn = false;
    #endregion

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

    void Start()
    {
        _resources = new CResources(50, 20, 0, 1, 0, 1);
        _hitMask |= LayerMask.GetMask("Tilemap");
        _tilemap = this.GetComponentInChildren<Tilemap>();
        CPrint.Log($"{_hitMask.value} = (Tilemap)");
        _rightPanelTransform.anchoredPosition3D = new Vector3(_rightPanelXOff, 0, 0);
        _upgradeButton.gameObject.SetActive(false);
        _currentTech = new Dictionary<ETech, int>();
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(
                () => CallUpgrade() );
        }
        if (_leftButtonToggle != null)
        {
            _leftButtonToggle.onClick.AddListener(
                () => CallLeftToggle());
        }
        if (_debugNextDay != null)
        {
            _debugNextDay.onClick.AddListener(
                () => CallNextDay());
        }
        if (_questionYes != null)
        {
            _questionYes.onClick.AddListener(
                () => CallAccept());
        }
        if (_questionNo != null)
        {
            _questionNo.onClick.AddListener(
                () => CallDecline());
        }
        if (_leftButton1 != null)
        {
            _leftButton1.onClick.AddListener(
                () => CallCommon01());
        }
        if (_leftButton2 != null)
        {
            _leftButton2.onClick.AddListener(
                () => CallCommon02());
        }
        if (_leftButton3 != null)
        {
            _leftButton3.onClick.AddListener(
                () => CallCommon03());
        }
        if (_leftButton4 != null)
        {
            _leftButton4.onClick.AddListener(
                () => CallCommon04());
        }
        DrawMapTiles();
    }

    void Update()
    {
        switch (_gameState)
        {
            case EGameState.Idle:
            case EGameState.TileInspect:
                if (Input.GetMouseButtonDown(1))
                {
                    OnClickElse();
                    _gameState = EGameState.Idle;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        CPrint.Log("UI 클릭함");
                        break;
                    }
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _hitMask))
                    {
                        Vector3 hitPoint = hit.point;
                        if (findObjectByTile(hitPoint, out Vector3Int posInCell, out GameObject go))
                        {
                            CPrint.V3($"클릭대상 - {go.name}", posInCell);
                            OnClickTile(go, posInCell);
                            _gameState = EGameState.TileInspect;
                        }
                        else
                        {
                            OnClickElse();
                            _gameState = EGameState.Idle;
                        }
                    }
                }
                break;
            case EGameState.TileSelect:
                ChangeBottomText("타일을 선택하세요.");
                if (Input.GetMouseButtonDown(1))
                {
                    CreateError("취소함", true);
                    ChangeBottomText("");
                    _gameState = EGameState.Idle;
                    break;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        CPrint.Log("UI 클릭함");
                        break;
                    }
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _hitMask))
                    {
                        Vector3 hitPoint = hit.point;
                        if (findObjectByTile(hitPoint, out Vector3Int posInCell, out GameObject go))
                        {
                            CPrint.V3($"클릭대상 - {go.name}", posInCell);
                            if (go.TryGetComponent<CTile>(out CTile tempClass))
                            {
                                if (_questionMask == ETileState.None && _questionMaskReverse == ETileState.None)
                                {
                                    ChangeBottomText("");
                                    ShowQuestion(
                                            _questionString,
                                            _questionAction,
                                            _questionArgGO,
                                            posInCell
                                        );
                                    break;
                                }
                                else
                                {
                                    if (_questionMask == ETileState.None)
                                    {
                                        if ((tempClass.TileState & _questionMaskReverse) == ETileState.None)
                                        {
                                            ChangeBottomText("");
                                            ShowQuestion(
                                                    _questionString,
                                                    _questionAction,
                                                    _questionArgGO,
                                                    posInCell
                                                );
                                            break;
                                        }
                                    }
                                    else if (_questionMaskReverse == ETileState.None)
                                    {
                                        if ((tempClass.TileState & _questionMask) == _questionMask)
                                        {
                                            ChangeBottomText("");
                                            ShowQuestion(
                                                    _questionString,
                                                    _questionAction,
                                                    _questionArgGO,
                                                    posInCell
                                                );
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if ((tempClass.TileState & _questionMaskReverse) == ETileState.None)
                                        {
                                            if ((tempClass.TileState & _questionMask) == _questionMask)
                                            {
                                                ChangeBottomText("");
                                                ShowQuestion(
                                                        _questionString,
                                                        _questionAction,
                                                        _questionArgGO,
                                                        posInCell
                                                    );
                                                break;
                                            }
                                        }

                                    }
                                }
                                CreateError("유효하지 않은 타일입니다.", true);
                                ChangeBottomText("");
                                _gameState = EGameState.Idle;
                            }
                        }
                        else
                        {
                            CreateError("취소함", true);
                            ChangeBottomText("");
                            _gameState = EGameState.Idle;
                        }
                    }
                }
                break;
            case EGameState.Question:
                if (Input.GetMouseButtonDown(1))
                {
                    CreateError("취소함", true);
                    HideQuestion();
                }

                if (_questionValue == 0)
                {
                    break;
                }
                else if (_questionValue == 1)
                {
                    if (_questionIsCard) _questionCard(_questionArgCard);
                    else _questionAction(_questionArgGO, _questionArgVector);
                }
                HideQuestion();
                break;
        }

        LeftPanelMove();
        RightPanelMove();
        ResourceSync();
    }

    public int CheckResource(int moneyCurrent, int moneyIncrease,
        int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease, bool canUseMaterials = false)
    {
        if (_resources.moneyCurrent < moneyCurrent)
        {
            if (!canUseMaterials) return -1;
            else if ((_resources.moneyCurrent + 2 * (_resources.materialsCurrent) < moneyCurrent)) return -1;
            else return 1;
        }
        if (_resources.moneyIncrease < moneyIncrease) return -1;
        if (_resources.materialsCurrent < materialsCurrent) return -1;
        if (_resources.materialsIncrease <  materialsIncrease) return -1;
        if (_resources.menpowerCurrent < menpowerCurrent) return -1;
        if (_resources.menpowerIncrease < menpowerIncrease) return -1;
        return 0;
    }

    public int CheckResource(SCost cost)
    {
        if (_resources.moneyCurrent < cost.moneyCurrent)
        {
            if (!cost.canUseMaterials) return -1;
            else if ((_resources.moneyCurrent + 2 * (_resources.materialsCurrent) < cost.moneyCurrent)) return -1;
            else return 1;
        }
        if (_resources.moneyIncrease < cost.moneyIncrease) return -1;
        if (_resources.materialsCurrent < cost.materialsCurrent) return -1;
        if (_resources.materialsIncrease < cost.materialsIncrease) return -1;
        if (_resources.menpowerCurrent < cost.menpowerCurrent) return -1;
        if (_resources.menpowerIncrease < cost.menpowerIncrease) return -1;
        return 0;
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
