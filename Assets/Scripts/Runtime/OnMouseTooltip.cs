using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Canvas _canvas;
    [SerializeField] private TMP_Text _tooltip;
    [SerializeField][TextArea(2, 10)] private string _description;

    private bool _isOn = false;
    private Vector2 _Offset = new Vector2(10f, 10f);

    void Start()
    {
        if (_tooltip == null)
        {
            CPrint.Error("툴팁 할당이 안되어있습니다.");
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isOn = true;
        _tooltip.text = _description;
        _tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isOn = false;
        _tooltip.gameObject.SetActive(false);
    }
}
