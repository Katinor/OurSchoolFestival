using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ETempTileCatalog
{
    Basement,
    RoadBase,
    RoadBuilt,
    Trees
}

public class Test_TilemapSelector : MonoBehaviour
{
    [Header("그리드")]
    [SerializeField] Grid _grid;

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
    [SerializeField] private RectTransform _panelTransform;
    [SerializeField] private TMP_Text _tileName;
    [SerializeField] private TMP_Text _tileDescription;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private float _panelXOn = 640;
    [SerializeField] private float _panelXOff = 1180;
    [SerializeField] private float _panelMove = 1500;

    private Tilemap _tilemap;
    private Color _lastColor;
    private GameObject _lastObject;
    private GameObject _lastSelectedObject;
    private Vector3Int _lastSelectedPosition;
    private LayerMask _hitMask = 0;

    private bool _UIOn = false;

    void Start()
    {
        _hitMask |= LayerMask.GetMask("Tilemap");
        _tilemap = this.GetComponentInChildren<Tilemap>();
        CPrint.Log($"{_hitMask.value} = (Tilemap)");
        _panelTransform.anchoredPosition3D = new Vector3(_panelXOff, 0, 0);
        _upgradeButton.gameObject.SetActive(false);
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(
                () => CallUpgrade() );
        }
        DrawMapTiles();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            CPrint.V3("마우스 입력 감지 : ", ray.GetPoint(0));
            if (EventSystem.current.IsPointerOverGameObject())
            {
                CPrint.Log("UI 클릭함");
                return;
            }
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _hitMask))
            {
                CPrint.V3("Mouse input detected", hit.point);
                Vector3 hitPoint = hit.point;
                if(findObjectByTile(hitPoint, out Vector3Int posInCell, out GameObject go))
                {
                    CPrint.V3($"클릭대상 - {go.name}", posInCell);
                    OnClickTile(go, posInCell);
                }
                else
                {
                    CPrint.V3($"클릭위치", hit.point);
                    OnClickElse();
                }
            }
        }

        if (_UIOn)
        {
            if (_panelTransform.anchoredPosition3D.x > _panelXOn)
            {
                _panelTransform.anchoredPosition3D -= Vector3.right * _panelMove * Time.deltaTime;
                if (_panelTransform.anchoredPosition3D.x <= _panelXOn)
                {
                    _panelTransform.anchoredPosition3D = new Vector3(_panelXOn, 0, 0);
                }
            }
        }
        else
        {
            if (_panelTransform.anchoredPosition3D.x < _panelXOff)
            {
                _panelTransform.anchoredPosition3D += Vector3.right * _panelMove * Time.deltaTime;
                if (_panelTransform.anchoredPosition3D.x >= _panelXOff)
                {
                    _panelTransform.anchoredPosition3D = new Vector3(_panelXOff, 0, 0);
                    _tileName.text = "";
                    _tileDescription.text = "";
                }
            }
        }
    }

    void DrawMapTiles()
    {
        if (_tilemap == null)
        {
            CPrint.Error("타일맵 찾을 수 없음.");
        }
        if (_tileBases[GetIndex(ETempTileCatalog.RoadBase)] == null)
        {
            CPrint.Error("길 찾을 수 없음.");
            return;
        }
        if (_tileBases[GetIndex(ETempTileCatalog.Basement)] == null)
        {
            CPrint.Error("땅 찾을 수 없음.");
            return;
        }
        TileBase baseRoad = _tileBases[GetIndex(ETempTileCatalog.RoadBase)];
        TileBase baseLand = _tileBases[GetIndex(ETempTileCatalog.Basement)];


        _tilemap.ClearAllTiles();
        for (int i = (-1) * _gridSizeYUpper; i <= _gridSizeYUpper; i++)
        {
            for (int j = (-1) * _gridSizeXRight; j <= _gridSizeXRight; j++)
            {
                if (i == 0 && j >= (_gridSizeXRight + 1 - _roadSize))
                {
                    _tilemap.SetTile(new Vector3Int(j, i, 0), baseRoad);
                }
                else
                {
                    _tilemap.SetTile(new Vector3Int(j, i, 0), baseLand);
                }
            }
        }
    }

    bool findObjectByTile(Vector3 hitpoint, out Vector3Int posInCell, out GameObject go)
    {
        posInCell = _tilemap.WorldToCell(hitpoint);
        if (Mathf.Abs(posInCell.x) > _gridSizeXRight || Mathf.Abs(posInCell.y) > _gridSizeYUpper)
        {
            go = null;
            return false;
        }
        go = _tilemap.GetInstantiatedObject(posInCell);
        if (go == null)
        {
            return false;
        }
        return true;
    }

    void OnClickTile(GameObject go, Vector3Int posInCell)
    {
        _UIOn = true;
        CTile tempClass = null;
        if (_lastObject != null)
        {
            if (_lastObject.TryGetComponent<CTile>(out tempClass))
            {
                tempClass.Highlights(_lastColor);
            }
        }
        if (go.TryGetComponent<CTile>(out tempClass))
        {
            _lastColor = tempClass.getColor();
            tempClass.Highlights(Color.yellow);
            _tileName.text = tempClass.Name;
            _tileDescription.text = tempClass.Description;
            CPrint.Log($"{tempClass.TileState}");
            if ((tempClass.TileState & ETileState.Upgradable) != ETileState.None)
            {
                _upgradeButton.gameObject.SetActive(true);
            }
            else
            {
                _upgradeButton.gameObject.SetActive(false);
            }
        }
        else
        {
            _undertext.text = $"({posInCell.x}, {posInCell.y}) : CTile 없는 객체";
        }
        _lastObject = go;
        _lastSelectedObject = go;
        _lastSelectedPosition = posInCell;

        tempClass.PlaySound();
    }

    void OnClickElse()
    {
        _UIOn = false;
        CTile tempClass = null;
        if (_lastObject != null)
        {
            if (_lastObject.TryGetComponent<CTile>(out tempClass))
            {
                tempClass.Highlights(_lastColor);
            }
        }
        _lastObject = null;
        _undertext.text = "경계 밖 클릭";
    }

    public static int GetIndex(ETempTileCatalog tileCatalog)
    {
        return (int) tileCatalog;
    }

    public void CallUpgrade()
    {
        CTile tempClass;
        if (_lastSelectedObject.TryGetComponent<CTile>(out tempClass))
        {
            if ((tempClass.TileState & ETileState.Upgradable) != ETileState.None)
            {
                if (_upgradeSe != null)
                {
                    _upgradeSe.Play();
                }
                tempClass.Upgrade(_tilemap, _tileBases, _lastSelectedPosition);
                OnClickElse();
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }
}
