using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

[Flags]
public enum ETileState
{
    None        = 0,        // 0 : 상태 없음
    Built       = 1 << 0,   // 1 : 건설됨
    Road        = 1 << 1,   // 2 : 도로가 지어져야하는 위치
    Text        = 1 << 2,   // 4 : 텍스트 있음
    Action      = 1 << 3,   // 8 : 사용효과가 있는 타일
    Upgradable  = 1 << 4,   // 16 : 업그레이드 가능 타일
    Point       = 1 << 5,   // 32 : 승점을 제공하는 타일
    PointUp     = 1 << 6,   // 64 : 승점타일을 강화하는 타일
}

public enum ETileBuilding
{
    Start = 0,
    PostBig = 1,
    End = 2,
}

public class CTile : MonoBehaviour
{
    [Header("타일 정보")]
    [SerializeField] private Renderer _baseRenderer;
    [SerializeField] private AudioSource _selectSound;
    [SerializeField] private Transform _builtTransform;
    [SerializeField] private GameObject _textObject;
    [SerializeField] private TMP_Text _tileText;

    protected string _name;
    protected string _description;
    protected string _tileInfo;
    protected ETileCatalog _tileInCatalog;
    protected ETileState _tileState = ETileState.None;
    protected ETileBuilding _building;
    protected SCost _cost;
    protected Vector3Int _tilePosition;

    protected GameManager _gameManager;
    protected ETileCatalog _upgradeResult;

    protected string _actionName = null;
    protected string _actionDescription = null;
    protected bool _actionUsed = false;
    protected bool _actionEnabled = false;

    protected int _internalPoints = 0;
    protected float _buildingSpeed = 3f;

    public string Name
    {
        get { return _name; }
        protected set { _name = value; }
    }

    public string Description
    {
        get { return _description; }
        protected set { _description = value; }
    }

    public string TileInfo
    {
        get { return _tileInfo; }
        set { _tileInfo = value; }
    }

    public ETileCatalog TileInCatalog
    {
        get { return _tileInCatalog; }
        protected set { _tileInCatalog = value; }
    }
    public SCost Cost
    {
        get { return _cost; }
        protected set { _cost = value; }
    }
    
    public ETileState TileState
    {
        get { return _tileState; }
        protected set { _tileState = value; }
    }
    public string ActionName
    {
        get { return _actionName; }
        protected set { _actionName = value; }
    }

    public string ActionDescription
    {
        get { return _actionDescription; }
        protected set { _actionDescription = value; }
    }

    public bool ActionUsed
    {
        get { return _actionUsed; }
        set { _actionUsed = value; }
    }

    public bool ActionEnabled
    {
        get { return _actionEnabled; }
        set { _actionEnabled = value; }
    }

    public int Points
    {
        get { return _internalPoints; }
        set { _internalPoints = value; }
    }

    public void Highlights(Color color)
    {
        _baseRenderer.material.color = color;
    }

    public Color getColor()
    {
        return _baseRenderer.material.color;
    }

    public void PlaySound()
    {
        if (_selectSound != null)
        {
            _selectSound.Play();
        }
    }

    public virtual int OnCalculatePoint()
    {
        return _internalPoints;
    }
    /// <summary>
    /// 시설을 업그레이드할 수 있다면 업그레이드합니다.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public virtual int Upgrade(CResources resource, GameManager manager, List<TileBase> tileBases, Vector3Int position)
    {
        // 비용에 문제가 있다면 Return하는 함수 필요
        CPrint.V3($"타일 업그레이드", position);
        resource.PayCost(_cost);
        manager.BuildTile(_upgradeResult, position);
        return 0;
    }

    public virtual bool IsUpgradable(GameManager gameManager)
    {
        return false;
    }

    public virtual bool OnAction(GameManager gameManager)
    {
        return false;
    }

    public virtual bool IsActionable(GameManager gameManager)
    {
        return false;
    }

    protected virtual void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            CPrint.Error("게임매니저 못찾음");
        }
        if (_builtTransform != null)
        {
            _building = ETileBuilding.Start;
            _builtTransform.localScale = Vector3.one * 0.05f;
        }
        else
        {
            _building = ETileBuilding.End;
        }
        if ((_tileState & ETileState.Text) == ETileState.None)
        {
            if (_textObject)
            {
                _textObject.SetActive(false);
            }
        }
        else
        {
            if (_textObject)
            {
                _textObject.SetActive(true);
            }
        }
        _tilePosition = _gameManager.GameGrid.WorldToCell(transform.position);
    }

    protected virtual void Update()
    {
        if (_building != ETileBuilding.End)
        {
            float currentScale;
            switch (_building)
            {
                case ETileBuilding.Start:
                    currentScale = _builtTransform.localScale.x;
                    if (currentScale > 1.5f)
                    {
                        _building = ETileBuilding.PostBig;
                    }
                    else
                    {
                        currentScale += _buildingSpeed * Time.deltaTime;
                        _builtTransform.localScale = Vector3.one * currentScale;
                    }
                    break;
                case ETileBuilding.PostBig:
                    currentScale = _builtTransform.localScale.x;
                    if (currentScale <= 1f)
                    {
                        _builtTransform.localScale = Vector3.one;
                        _building = ETileBuilding.End;
                    }
                    else
                    {
                        currentScale -= _buildingSpeed * Time.deltaTime;
                        _builtTransform.localScale = Vector3.one * currentScale;
                    }
                    break;
                case ETileBuilding.End:
                    break;
            }
        }
        if ((_tileState & ETileState.Text) != ETileState.None)
        {
            _tileText.text = _tileInfo;
        }
    }

    public virtual int OnScore()
    {
        return 0;
    }
}