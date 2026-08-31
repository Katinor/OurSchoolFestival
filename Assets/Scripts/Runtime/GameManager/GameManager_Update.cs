using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameManager
{
    private void UpdateIdle()
    {
        CatchCommonKeyaction();
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
                return;
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
    }

    private void UpdateMaterialCheck()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CreateError("취소함", true);
            _gameState = EGameState.Idle;
            HideMaterialChecker();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _materialsYes.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _materialsNo.onClick.Invoke();
        }

        if (_questionValue == 0)
        {
            RefreshMatPanel(_questionArgCard);
            return;
        }
        else if (_questionValue == 1)
        {
            if (_usingMatCount > _resources.materialsCurrent)
            {
                CreateError("자재가 부족합니다.", true);
                _questionValue = 0;
                return;
            }
            else if ( _questionArgCard.Cost - (_usingMatCount * 2) > _resources.moneyCurrent )
            {
                CreateError("남은 비용을 충당할 자본이 없습니다.", true);
                _questionValue = 0;
                return;
            }
            CallCard(_questionArgCard, true);
        }
        else
        {
            CreateError("취소함", true);
            _gameState = EGameState.Idle;
        }
        HideMaterialChecker();
    }

    private void UpdateTileSelect()
    {
        ChangeBottomText("타일을 선택하세요.");
        if (Input.GetMouseButtonDown(1))
        {
            CreateError("취소함", true);
            ChangeBottomText("");
            _gameState = EGameState.Idle;
            return;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CreateError("취소함", true);
            ChangeBottomText("");
            _gameState = EGameState.Idle;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                CPrint.Log("UI 클릭함");
                return;
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
                            OpenQuestionAfterTile(posInCell);
                            return;
                        }
                        else
                        {
                            if (_questionMask == ETileState.None)
                            {
                                if ((tempClass.TileState & _questionMaskReverse) == ETileState.None)
                                {
                                    ChangeBottomText("");
                                    OpenQuestionAfterTile(posInCell);
                                    return;
                                }
                            }
                            else if (_questionMaskReverse == ETileState.None)
                            {
                                if ((tempClass.TileState & _questionMask) == _questionMask)
                                {
                                    ChangeBottomText("");
                                    OpenQuestionAfterTile(posInCell);
                                    return;
                                }
                            }
                            else
                            {
                                if ((tempClass.TileState & _questionMaskReverse) == ETileState.None)
                                {
                                    if ((tempClass.TileState & _questionMask) == _questionMask)
                                    {
                                        ChangeBottomText("");
                                        OpenQuestionAfterTile(posInCell);
                                        return;
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
    }

    private void UpdateQuestionSelect()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CreateError("취소함", true);
            HideQuestion();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _questionYes.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _questionNo.onClick.Invoke();
        }
        if (_questionValue == 0)
        {
            return;
        }
        else if (_questionValue == 1)
        {
            if (_questionIsCard) _questionCard(_questionArgCard, _questionArgVector);
            else _questionAction(_questionArgGO, _questionArgVector);
        }
        HideQuestion();
    }
    private void OpenTileAfterMat()
    {
        if (_questionIsCard)
        {
            ShowQuestion(
                _questionString,
                _questionCard,
                _questionArgCard,
                Vector3Int.zero
            );
        }
        else
        {
            ShowQuestion(
               _questionString,
               _questionAction,
               _questionArgGO,
               Vector3Int.zero
            );
        }
    }

    private void OpenQuestionAfterMat()
    {
        if (_questionIsCard)
        {
            ShowQuestion(
                _questionString,
                _questionCard,
                _questionArgCard,
                Vector3Int.zero
            );
        }
        else
        {
            ShowQuestion(
               _questionString,
               _questionAction,
               _questionArgGO,
               Vector3Int.zero
            );
        }
    }

    private void OpenQuestionAfterTile(Vector3Int posInCell)
    {
        if (_questionIsCard)
        {
            ShowQuestion(
                _questionString,
                _questionCard,
                _questionArgCard,
                posInCell
            );
        }
        else
        {
            ShowQuestion(
               _questionString,
               _questionAction,
               _questionArgGO,
               posInCell
            );
        }
    }
}
