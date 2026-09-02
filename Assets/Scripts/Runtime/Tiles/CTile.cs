using System;
using System.Collections;
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
    [SerializeField] private List<ParticleSystem> _particleSystem;
    [SerializeField] private bool _isParticleScaled = true;

    protected string _name = "";
    protected string _description = "";
    protected string _additionalDescription = "";
    protected int _radius = 0;
    protected bool _isFirst = false;
    protected Color _baseColor;
    protected string _tileInfo = "";
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
    protected float _buildingSpeed = 5f;
    protected Coroutine _particleCoroutine;

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

    public string AdditionalDescription
    {
        get { return _additionalDescription; }
        protected set { _additionalDescription = value; }
    }

    public string TileInfo
    {
        get { return _tileInfo; }
        set { _tileInfo = value; }
    }

    public int Radius
    {
        get { return _radius; }
        protected set { _radius = value; }
    }

    public bool IsFirst
    {
        get { return _isFirst; }
        set { _isFirst = value; }
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

    public string GetDescription()
    {
        return _description + "\n\n" + _additionalDescription;
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
    public virtual bool Upgrade(CResources resource, GameManager manager, List<TileBase> tileBases, Vector3Int? position)
    {
        // 비용에 문제가 있다면 Return하는 함수 필요
        if (position.HasValue)
        {
            CPrint.V3($"타일 업그레이드", position.Value);
            resource.PayCost(_cost);
            manager.BuildTile(_upgradeResult, position.Value);
            return true;
        }
        return false;
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
        if (_baseColor == null)
        {
            _baseColor = _baseRenderer.material.color;
        }
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
        if (!_isFirst) _particleCoroutine = StartCoroutine(OnParticle());
        if (_radius > 0)
        {
            List<CTile> tempList = _gameManager.FindNeighborTiles(_tilePosition, _radius);
            for(int i = 0; i < tempList.Count; i++)
            {
                tempList[i].ShowParticle(0, 0, 0.5f);
            }
        }
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
                    if (currentScale > 1.75f)
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

    public virtual SScoreInfo OnScore()
    {
        return new SScoreInfo(0, "");
    }
    public virtual void OnPoint(int radius = 0)
    {
        Highlights(Color.green);
        ShowAllParticle(radius, true);
    }

    public virtual void OnPointByOthers()
    {
        Highlights(Color.yellow);
    }

    public virtual void OnSelected()
    {
        Highlights(Color.cyan);
        _tileText.gameObject.SetActive(true);
    }

    public virtual void OnSelectedByOthers()
    {
        Highlights(new Color(0.4f, 1f, 0f));
    }

    public virtual void OnResetShown()
    {
        Highlights(_baseColor);
        _tileText.gameObject.SetActive(false);
        StopAllParticle();
    }

    public virtual IEnumerator OnParticle()
    {
        if (_particleSystem == null || _particleSystem.Count == 0)
        {
            yield break;
        }
        _particleSystem[0].transform.localScale = Vector3.one * 2;
        for(int i = 0; i < _particleSystem.Count; i++)
        {
            if (_particleSystem[i] != null)
            {
                if (_isParticleScaled && (i != 0)) _particleSystem[i].transform.localScale = Vector3.one * (1 + _radius);
                _particleSystem[i].Play(true);
            }
            else
            {
                CPrint.Error($"{this._name} : 파티클이 잘못됨");
            }
        }
        yield return new WaitForSeconds(0.75f);
        _particleSystem[0].Stop();
        yield return new WaitForSeconds(0.75f);
        if (_particleSystem.Count > 1)
        {
            for (int i = 1; i < _particleSystem.Count; i++)
            {
                _particleSystem[i].Stop();
            }
        }
    }

    public virtual void ShowAllParticle(int radius = 0, bool isFirstSkip = true)
    {
        int firstIndex = 0;
        if (isFirstSkip) firstIndex = 1;
        else _particleSystem[0].transform.localScale = Vector3.one;
        if (_particleSystem == null || _particleSystem.Count <= firstIndex)
        {
            return;
        }

        for (int i = firstIndex; i < _particleSystem.Count; i++)
        {
            if (_particleSystem[i] != null)
            {
                if (_isParticleScaled && (i != 0)) _particleSystem[i].transform.localScale = Vector3.one * (1 + radius);
                _particleSystem[i].Play(true);
            }
            else
            {
                CPrint.Error($"{this._name} : 파티클이 잘못됨");
            }
        }
    }
    public virtual void ShowParticle(int radius = 0, int index = 0, float duration = 0.5f)
    {
        if (_particleCoroutine != null)
        {
            StopCoroutine(_particleCoroutine);
            StopAllParticle();
        }
        if (_particleSystem == null || index >= _particleSystem.Count)
        {
            return;
        }
        _particleCoroutine = StartCoroutine(OnParticleOnce(radius, index, duration));
    }
    public virtual IEnumerator OnParticleOnce(int radius, int index, float duration)
    {
        if (_particleSystem == null || index >= _particleSystem.Count)
        {
            yield break;
        }
        
        if (_particleSystem[index] != null)
        {
            _particleSystem[index].transform.localScale = Vector3.one * (1 + _radius);
            _particleSystem[index].Play(true);
        }
        else
        {
            CPrint.Error($"{this._name} : 파티클이 잘못됨");
            yield break;
        }

        yield return new WaitForSeconds(duration);
        _particleSystem[index].Stop();
        yield break;
    }

    public virtual void StopAllParticle()
    {
        for (int i = 0; i < _particleSystem.Count; i++)
        {
            _particleSystem[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _particleSystem[i].transform.localScale = Vector3.one;
        }
    }
}