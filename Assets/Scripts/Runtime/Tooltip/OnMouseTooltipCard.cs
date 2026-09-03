using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseTooltipCard : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] private TMP_Text _tooltip;

    private bool _isOn = false;
    private Vector2 _Offset = new Vector2(10f, 10f);

    void Start()
    {
        if (_tooltip == null)
        {
            Logger.Error("툴팁 할당이 안되어있습니다.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (_isOn)
        {
            FindNewPosition();
        }
    }

    private void FindNewPosition()
    {
        int flag = 0;
        Vector2 localPoint;
        Vector2 pivot = new Vector2(1, 1);
        RectTransform rectTransform = _tooltip.rectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle
                (
                    _canvas.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    null,
                    out localPoint)
                )
        {
            if (localPoint.x < 0) flag += 1;
            if (localPoint.y < 0) flag += 2;

            switch (flag)
            {
                case 0:
                    pivot.x = 1;
                    pivot.y = 1;
                    break;
                case 1:
                    pivot.x = 0;
                    pivot.y = 1;
                    break;
                case 2:
                    pivot.x = 1;
                    pivot.y = 0;
                    break;
                case 3:
                    pivot.x = 0;
                    pivot.y = 0;
                    break;
            }
            
        }
        else
        {
            localPoint = Vector2.zero;
        }
        rectTransform.anchoredPosition = localPoint;
        rectTransform.pivot = pivot;
    }

    public void Enable(string tooltip)
    {
        _isOn = true;
        _tooltip.gameObject.SetActive(true);
        _tooltip.text = tooltip;
    }

    public void Disable()
    {
        _isOn = false;
        _tooltip.gameObject.SetActive(false);
        // StartCoroutine(DisableCheckCoroutine());
    }

    private IEnumerator DisableCheckCoroutine()
    {
        yield return new WaitForEndOfFrame();
        if (!_isOn)
        {
            _tooltip.gameObject.SetActive(false);
        }
    }
}
