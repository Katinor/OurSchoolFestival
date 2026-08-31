using System;
using System.Collections;
using TMPro;
using UnityEngine;

public partial class GameManager
{
    private void LeftPanelMove()
    {
        if (_leftUIOn)
        {
            if (_leftPanelTransform.anchoredPosition3D.x < _leftPanelXOn)
            {
                _leftPanelTransform.anchoredPosition3D += Vector3.right * _panelMove * Time.deltaTime;
                if (_leftPanelTransform.anchoredPosition3D.x >= _leftPanelXOn)
                {
                    _leftPanelTransform.anchoredPosition3D = new Vector3(_leftPanelXOn, 0, 0);
                }
            }
        }
        else
        {
            if (_leftPanelTransform.anchoredPosition3D.x > _leftPanelXOff)
            {
                _leftPanelTransform.anchoredPosition3D -= Vector3.right * _panelMove * Time.deltaTime;
                if (_leftPanelTransform.anchoredPosition3D.x <= _leftPanelXOff)
                {
                    _leftPanelTransform.anchoredPosition3D = new Vector3(_leftPanelXOff, 0, 0);
                    _tileName.text = "";
                    _tileDescription.text = "";
                }
            }
        }
    }

    private void RightPanelMove()
    {
        if (_rightUIOn)
        {
            if (_rightPanelTransform.anchoredPosition3D.x > _rightPanelXOn)
            {
                _rightPanelTransform.anchoredPosition3D -= Vector3.right * _panelMove * Time.deltaTime;
                if (_rightPanelTransform.anchoredPosition3D.x <= _rightPanelXOn)
                {
                    _rightPanelTransform.anchoredPosition3D = new Vector3(_rightPanelXOn, 0, 0);
                }
            }
        }
        else
        {
            if (_rightPanelTransform.anchoredPosition3D.x < _rightPanelXOff)
            {
                _rightPanelTransform.anchoredPosition3D += Vector3.right * _panelMove * Time.deltaTime;
                if (_rightPanelTransform.anchoredPosition3D.x >= _rightPanelXOff)
                {
                    _rightPanelTransform.anchoredPosition3D = new Vector3(_rightPanelXOff, 0, 0);
                    _tileName.text = "";
                    _tileDescription.text = "";
                }
            }
        }
    }

    private void ResourceSync()
    {
        _moneyCurrentText.text = _resources.moneyCurrent.ToString();
        _moneyIncreaseText.text = "+" + _resources.moneyIncrease.ToString();
        _materialsCurrentText.text = _resources.materialsCurrent.ToString();
        _materialsIncreaseText.text = "+" + _resources.materialsIncrease.ToString();
        _menpowerCurrentText.text = _resources.menpowerCurrent.ToString();
        _menpowerIncreaseText.text = "+" + _resources.menpowerIncrease.ToString();
        _successText.text = Clamp(_resources.festivalSuccess, 0, 14).ToString();
        _interestText.text = Clamp(_resources.festivalInterest, 0, 19).ToString();
        _roadText.text = Clamp(_resources.festivalRoad, 0, 8).ToString();
    }

    private int Clamp(int target, int min, int max)
    {
        if (target < min) return min;
        else if (target > max) return max;
        else return target;
    }

    private void ChangeBottomText(string text)
    {
        _undertext.text = text;
        if (string.IsNullOrEmpty(text))
        {
            _undertext.gameObject.SetActive(false);
        }
        else
        {
            _undertext.gameObject.SetActive(true);
        }
    }

    private void RefreshMatPanel(CCard card)
    {
        _materialsCount.text = _usingMatCount.ToString();
        int remainMoney = card.Cost;
        remainMoney -= _usingMatCount * 2;
        if (remainMoney < 0) remainMoney = 0;
        _materialsText.text = $"자재를 {_usingMatCount} 만큼 사용하고\r\n남은 {remainMoney} 은/는 자본으로 지불합니다. ";
    }

    public void CreateError(string text, bool trackMouse = false)
    {
        GameObject go = Instantiate(_errorPrefab, _errorCanvas.transform);
        if (go.TryGetComponent<TMP_Text>(out TMP_Text goText))
        {
            StartCoroutine(CreateErrorCoroutine(goText, text, trackMouse));
        }
        else
        {
            CPrint.Error("에러 프리팹 잘못됨");
        }
    }

    private IEnumerator CreateErrorCoroutine(TMP_Text go, string text, bool trackMouse)
    {
        go.text = text;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        Vector2 basePoint;
        
        if(trackMouse)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle
                (
                    _errorCanvas.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    null,
                    out Vector2 localPoint)
                )
            {
                rectTransform.anchoredPosition = localPoint;
                basePoint = localPoint;
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
                basePoint = Vector2.zero;
            }
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.zero;
            basePoint = Vector2.zero;
        }
        go.alpha = 0f;
        rectTransform.localScale = Vector3.one * 1.5f;
        float timer = 0f;
        while (timer < 0.125f)
        {
            timer += Time.deltaTime;
            rectTransform.localScale = Vector3.one * (1.5f - timer * 4f);
            go.alpha = timer * 8f;
            yield return null;
        }
        go.alpha = 1f;
        rectTransform.localScale = Vector3.one;
        timer = 0f;
        yield return new WaitForSeconds(0.5f);
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            rectTransform.anchoredPosition = basePoint + Vector2.up * timer * 2 * (rectTransform.rect.height * 0.1f);
            go.alpha = 1 - timer * 2;
            yield return null;
        }
        Destroy(go.gameObject);
    }

    private void ShowMaterialChecker(CCard card)
    {
        if (_gameState == EGameState.MaterialCount)
        {
            HideMaterialChecker();
        }
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _usingMatCount = 0;
        _questionValue = 0;
        _questionArgCard = card;
        _materialsPanel.gameObject.SetActive(true);
        _gameState = EGameState.MaterialCount;
    }

    private void HideMaterialChecker()
    {
        _materialsPanel.gameObject.SetActive(false);
    }

    private void ShowQuestion(string text, Action<CCard, Vector3Int> action, CCard card, bool tileSkiped = true)
    {
        if (_gameState == EGameState.Question)
        {
            HideQuestion();
        }
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionPanel.gameObject.SetActive(true);
        _questionText.text = text;
        _questionValue = 0;
        _questionIsCard = true;
        _questionCard = action;
        _questionArgCard = card;
        _questionArgVector = Vector3Int.zero;
        _questionIsTileSkiped = tileSkiped;
        _gameState = EGameState.Question;
    }
    private void ShowQuestion(string text, Action<CCard, Vector3Int> action, CCard card, Vector3Int actionArgVector)
    {
        if (_gameState == EGameState.Question)
        {
            HideQuestion();
        }
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionPanel.gameObject.SetActive(true);
        _questionText.text = text;
        _questionValue = 0;
        _questionIsCard = true;
        _questionCard = action;
        _questionArgCard = card;
        _questionArgVector = actionArgVector;
        _questionIsTileSkiped = false;
        _gameState = EGameState.Question;
    }
    private void ShowQuestion(string text, Action<GameObject, Vector3Int> action = null, GameObject actionArgGO = null)
    {
        if (_gameState == EGameState.Question)
        {
            HideQuestion();
        }
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionPanel.gameObject.SetActive(true);
        _questionText.text = text;
        _questionValue = 0;
        _questionIsCard = false;
        _questionAction = action;
        _questionArgGO = actionArgGO;
        _questionArgVector = Vector3Int.zero;
        _gameState = EGameState.Question;
    }

    private void ShowQuestion(string text, Action<GameObject, Vector3Int> action, GameObject actionArgGO, Vector3Int actionArgVector)
    {
        if (_gameState == EGameState.Question)
        {
            HideQuestion();
        }
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionPanel.gameObject.SetActive(true);
        _questionText.text = text;
        _questionValue = 0;
        _questionIsCard = false;
        _questionAction = action;
        _questionArgGO = actionArgGO;
        _questionArgVector = actionArgVector;
        _gameState = EGameState.Question;
    }

    private void HideQuestion()
    {
        _questionPanel.gameObject.SetActive(false);
        _questionValue = 0;
        _questionAction = null;
        _questionCard = null;
        _gameState = EGameState.Idle;
    }

    private void ShowTilechecker(string text, Action<CCard, Vector3Int> action, ETileState mask, ETileState maskReversed, CCard card)
    {
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionString = text;
        _questionIsCard = true;
        _questionCard = action;
        _questionArgCard = card;
        _questionMask = mask;
        _questionMaskReverse = maskReversed;
        _gameState = EGameState.TileSelect;
    }

    private void ShowTilechecker(string text, Action<GameObject, Vector3Int> action, ETileState mask, ETileState maskReversed, GameObject actionArgGO = null)
    {
        if (_onQuestionSe != null)
        {
            _onQuestionSe.Play();
        }
        _questionString = text;
        _questionIsCard = false;
        _questionAction = action;
        _questionArgGO = actionArgGO;
        _questionMask = mask;
        _questionMaskReverse = maskReversed;
        _gameState = EGameState.TileSelect;
    }
}