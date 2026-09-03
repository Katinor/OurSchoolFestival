using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Vector3 _basePosition = new Vector3(0.5f, 5f, -3.5f);
    [SerializeField] private Vector3 _focusOffset = new Vector3(0.25f, 0f, 0.5f);
    [SerializeField] private float _minXBound = 0f;
    [SerializeField] private float _minZBound = 0f;
    [SerializeField] private float _maxXBound = 5f;
    [SerializeField] private float _maxZBound = 3f;

    [SerializeField] private float _cameraSpeed = 5f;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _scrollSpeed = 100f;
    [SerializeField] private float _maxZoom = 20f;

    [SerializeField] private float _focusDuration = 0.4f;

    private Camera _cam;
    private float _minZoom = 60f;
    private float _targetZoom = 60f;
    private Vector3 _dragOrigin;

    private Coroutine _focusCoroutine;
    private bool _isFocusing = false;
    private bool _lockZoom = false;
    private EGameState _lastGameState;


    public Camera cam
    {
        get { return _cam; }
        set { _cam = value; }
    }

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null)
        {
            Logger.Error("카메라 없음!");
            enabled = false;
        }
        if (_gameManager == null)
        {
            Logger.Error("게임매니저 없음!");
            enabled = false;
        }
        if (_tilemap == null)
        {
            Logger.Error("타일맵 없음!");
            enabled = false;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        ZoomSystem();
        CameraMovement();
    }

    private void ZoomSystem()
    {
        if (_lockZoom) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            _targetZoom = Mathf.Clamp
                (
                    _cam.fieldOfView - scroll * _scrollSpeed,
                    _maxZoom,
                    _minZoom
                );

        }
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetZoom, _zoomSpeed * Time.deltaTime);
        if (!_isFocusing) ClampCamera();
    }

    private void CameraMovement()
    {
        if (!_isFocusing)
        {
            if (Input.GetMouseButtonDown(2))
            {
                _dragOrigin = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                Vector3 mouseMove = Input.mousePosition - _dragOrigin;
                Vector3 targetMove = new Vector3
                    (
                        (-1) * mouseMove.x * _cameraSpeed * 0.001f,
                        0,
                        (-1) * mouseMove.y * _cameraSpeed * 0.001f
                    );
                _cam.transform.position += targetMove;
                _dragOrigin = Input.mousePosition;
                ClampCamera();
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            _dragOrigin = _basePosition;
        }
    }

    private void ClampCamera()
    {
        if (_isFocusing) return;
        float ratio = Mathf.InverseLerp(_minZoom, _maxZoom, _cam.fieldOfView);
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp
            (
                pos.x,
                _basePosition.x - _minXBound - ((_maxXBound - _minXBound) * ratio),
                _basePosition.x + _minXBound + ((_maxXBound - _minXBound) * ratio)
            );
        pos.z = Mathf.Clamp
            (
                pos.z,
                _basePosition.z - _minZBound - ((_maxZBound - _minZBound) * ratio),
                _basePosition.z + _minZBound + ((_maxZBound - _minZBound) * ratio)
            );
        transform.position = pos;
    }

    public void StartFocusing(Vector3Int targetPosition)
    {
        if (_focusCoroutine != null)
        {
            StopCoroutine(_focusCoroutine);
            _focusCoroutine = null;
        }
        Vector3 worldCenterPos = _tilemap.GetCellCenterWorld(targetPosition);
        Logger.Log("포커스!");
        _focusCoroutine = StartCoroutine(FocusingToCoroutine(worldCenterPos));
    }

    private IEnumerator FocusingToCoroutine(Vector3 targetPosition)
    {
        _isFocusing = true;
        _lockZoom = true;
        _targetZoom = _maxZoom + (_minZoom - _maxZoom) * 0.5f;
        Vector3 startPos = transform.position;
        float startZoom = _cam.fieldOfView;
        Vector3 targetPos = _basePosition + targetPosition + _focusOffset;
        float timer = 0f;
        while (timer < _focusDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / _focusDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            _cam.fieldOfView = Mathf.Lerp(startZoom, _targetZoom, t);
            yield return null;
        }
        transform.position = targetPos;
        _cam.fieldOfView = _targetZoom;
        _lockZoom = false;
        yield break;
    }

    private IEnumerator FocusingBackCoroutine()
    {
        _isFocusing = true;
        _lockZoom = true;
        _targetZoom = _minZoom;
        Vector3 startPos = transform.position;
        float startZoom = _cam.fieldOfView;
        Vector3 targetPos = _basePosition;
        float timer = 0f;
        while (timer < _focusDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / _focusDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            _cam.fieldOfView = Mathf.Lerp(startZoom, _targetZoom, t);
            yield return null;
        }
        transform.position = targetPos;
        _cam.fieldOfView = _targetZoom;
        _isFocusing = false;
        _lockZoom = false;
        yield break;
    }

    public void StopFocusing(bool onlyFocus = false)
    {
        if (onlyFocus && !_isFocusing) return;
        if (_focusCoroutine != null)
        {
            StopCoroutine(_focusCoroutine);
            _focusCoroutine = null;
        }
        _focusCoroutine = StartCoroutine(FocusingBackCoroutine());
    }
}
