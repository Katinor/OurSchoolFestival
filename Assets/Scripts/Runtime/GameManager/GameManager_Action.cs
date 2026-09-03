using UnityEngine;
using UnityEngine.Rendering;

public partial class GameManager
{
    private void SetListener()
    {
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(
                () => CallUpgrade());
        }
        if (_tileActionButton != null)
        {
            _tileActionButton.onClick.AddListener(
                () => CallTileAction());
        }
        if (_leftButtonToggle != null)
        {
            _leftButtonToggle.onClick.AddListener(
                () => CallLeftToggle());
        }
        if (_soundButtonToggle != null)
        {
            _soundButtonToggle.onClick.AddListener(
                () => CallSoundToggle());
        }
        if (_nextDayButton != null)
        {
            _nextDayButton.onClick.AddListener(
                () => CallNextDay());
        }
        if (_titleButton != null)
        {
            _titleButton.onClick.AddListener(
                () => CallGotoTitle());
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
        if (_leftButton5 != null)
        {
            _leftButton5.onClick.AddListener(
                () => CallCommon05());
        }
        if (_cardAddButton != null)
        {
            _cardAddButton.onClick.AddListener(
                () => CallCardAdd());
        }
        if (_materialsYes != null)
        {
            _materialsYes.onClick.AddListener(
                () => CallMatAccept());
        }
        if (_materialsNo != null)
        {
            _materialsNo.onClick.AddListener(
                () => CallMatDecline());
        }
        if (_materialsUp != null)
        {
            _materialsUp.onClick.AddListener(
                () => CallMatIncrease());
        }
        if (_materialsDown != null)
        {
            _materialsDown.onClick.AddListener(
                () => CallMatDecrease());
        }

    }

    private void CatchCommonKeyaction()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _leftButton1.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _leftButton2.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _leftButton3.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _leftButton4.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _leftButton5.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _leftButtonToggle.onClick.Invoke();
        }
    }

    private void CallUpgrade()
    {
        if (findTileByPosition(_lastSelectedPosition, out CTile LastObject))
        {
            if ((LastObject.TileState & ETileState.Upgradable) != ETileState.None)
            {
                if (CheckResource(LastObject.UpgradeCost))
                {
                    _soundManager.PlaySE(EEffectSound.Success);
                    if (!LastObject.Upgrade(this))
                    {
                        Logger.Error("문제 발생함");
                    }
                    OnClickElse();
                    _gameState = EGameState.Idle;
                    return;
                }
                else
                {
                    CreateError("자원 부족함", true);
                    return;
                }

            }
            else
            {
                return;
            }
        }
    }
    private void CallTileAction()
    {
        if (findTileByPosition(_lastSelectedPosition, out CTile LastObject))
        {
            if ((LastObject.TileState & ETileState.Action) != ETileState.None)
            {
                if (CheckResource(LastObject.ActionCost))
                {
                    _soundManager.PlaySE(EEffectSound.Success);
                    if (!LastObject.OnAction(this))
                    {
                        Logger.Error("문제 발생함");
                    }
                    if (LastObject.ActionUsed) _tileActionMessage.text = "(이미 사용함)";
                    else if (!LastObject.ActionEnabled) _tileActionMessage.text = "(사용조건 불만족)";
                    else if (!CheckResource(LastObject.ActionCost)) _tileActionMessage.text = "(액션 비용 부족)";
                    else _tileActionMessage.text = "";

                    if (!LastObject.ActionUsed && LastObject.ActionEnabled && CheckResource(LastObject.ActionCost))
                    {
                        _tileActionButton.interactable = true;
                    }
                    else
                    {
                        _tileActionButton.interactable = false;
                    }
                    return;
                }
                else
                {
                    CreateError("자원 부족함", true);
                    return;
                }

            }
            else
            {
                return;
            }
        }
    }

    public void CallCard(CCard card, bool alreadyPaiedMaterials)
    {
        int cardAvailable = card.AvailableToUse();
        if (cardAvailable == 0)
        {
            if (card.CanPayMaterials && !alreadyPaiedMaterials)
            {
                ShowMaterialChecker(card);
            }
            else
            {
                if (card.HasTileAction)
                {
                    if (card.IsTileRoad)
                    {
                        if (_resources.festivalRoad >= 8)
                        {
                            ShowQuestion
                            (
                                $"{card.CardName}을 사용합니까?\n(도로가 꽉 차, 타일 건설은 생략합니다.)",
                                ActionCardUse,
                                card,
                                true
                            );
                        }
                        else
                        {
                            ShowTilechecker
                            (
                                $"{card.CardName}을 사용합니까?",
                                ActionCardUse,
                                ETileState.Road,
                                ETileState.Built,
                                card
                            );
                        }
                    }
                    else
                    {
                        ShowTilechecker
                        (
                            $"{card.CardName}을 사용합니까?",
                            ActionCardUse,
                            ETileState.None,
                            ETileState.Built | ETileState.Road,
                            card
                        );
                    }
                }
                else
                {
                    ShowQuestion
                    (
                        $"{card.CardName}을 사용합니까?",
                        ActionCardUse,
                        card
                    );
                }
            }
        }
        else
        {
            switch (cardAvailable)
            {
                case 1:
                    CreateError("자원 부족함", true);
                    break;
                case 2:
                    CreateError("축제지표 불만족", true);
                    break;
                case 3:
                    CreateError("기술 불만족", true);
                    break;
                default:
                    CreateError("알 수 없는 이유", true);
                    break;
            }
        }
    }
    public void DeleteCard(CCard card)
    {
        Logger.Log($"삭제부르기 -> {card.IsDeletable}");
        if (card.IsDeletable)
        {
            ShowQuestion
                (
                    $"{card.CardName}을 삭제합니까?",
                    ActionCardDelete,
                    card
                );
        }
        else
        {
            CreateError("삭제 불가능!", true);
        }
    }

    private void ActionCardUse(CCard card, Vector3Int position)
    {
        card.UseCard(position, _usingMatCount, _questionIsTileSkiped);
        _soundManager.PlaySE(EEffectSound.Success);
    }

    private void ActionCardDelete(CCard card, Vector3Int position)
    {
        card.DeleteCard();
        _soundManager.PlaySE(EEffectSound.Success);
    }

    private void CallLeftToggle()
    {
        if (_gameState == EGameState.TileInspect)
        {
            _gameState = EGameState.Idle;
            _rightUIOn = false;
            _leftUIOn = !_leftUIOn;
        }
        else if (_gameState == EGameState.Idle)
        {
            _leftUIOn = !_leftUIOn;
        }
    }

    private void CallSoundToggle()
    {
        _soundUIOn = !_soundUIOn;
    }

    private void CallAccept()
    {
        _questionValue = 1;
    }

    private void CallDecline()
    {
        _questionValue = -1;
    }
    private void CallMatAccept()
    {
        _questionValue = 1;
    }

    private void CallMatDecline()
    {
        _questionValue = -1;
    }

    private void CallMatIncrease()
    {
        _soundManager.PlaySE(EEffectSound.QuestionChoose);

        _usingMatCount += 1;
        if (_usingMatCount > _resources.materialsCurrent)
        {
            _usingMatCount = _resources.materialsCurrent;
        }
    }

    private void CallMatDecrease()
    {
        _soundManager.PlaySE(EEffectSound.QuestionChoose);
        _usingMatCount -= 1;
        if (_usingMatCount < 0)
        {
            _usingMatCount = 0;
        }
    }

    private void CallCardAdd()
    {
        if (_cardHand.AddCard())
        {
            Logger.Success("카드 추가됨");
        }
        else
        {
            CreateError("손패 꽉 참");
        }
    }

    private void CallCommon01()
    {
        if(_gameState == EGameState.Idle || _gameState == EGameState.TileInspect)
        {
            if (CheckResource(11, 0, 0, 0, 0, 0))
            {
                ShowQuestion
                    (
                        "비용을 써서 학생회 인원을 충원합니다.\n진행합니까?",
                        ActionCommon01
                    );
            }
            else
            {
                CreateError("자원 부족함", true);
            }
        }
        else
        {
            CreateError("현재 상태에선 불가능합니다.", true);
        }
    }
    private void ActionCommon01(GameObject go, Vector3Int position)
    {
        _resources.PayCost(11, 0, 0, 0, 0, 0);
        _resources.menpowerIncrease += 1;
        _soundManager.PlaySE(EEffectSound.Success);
    }
    private void CallCommon02()
    {
        if (_gameState == EGameState.Idle || _gameState == EGameState.TileInspect)
        {
            if (CheckResource(14, 0, 0, 0, 0, 0))
            {
                ShowQuestion
                    (
                        "비용을 써서 대규모 광고를 합니다.\n진행합니까?",
                        ActionCommon02
                    );
            }
            else
            {
                CreateError("자원 부족함", true);
            }
        }
        else
        {
            CreateError("현재 상태에선 불가능합니다.", true);
        }
    }
    private void ActionCommon02(GameObject go, Vector3Int position)
    {
        _resources.PayCost(14, 0, 0, 0, 0, 0);
        _resources.festivalInterest += 1;
        _soundManager.PlaySE(EEffectSound.Success);
    }
    private void CallCommon03()
    {
        if (_gameState == EGameState.Idle || _gameState == EGameState.TileInspect)
        {
            if (CheckResource(23, 0, 0, 0, 0, 0))
            {
                ShowTilechecker
                    (
                        "해당 위치에 나무를 심습니다.\n진행합니까?",
                        ActionCommon03,
                        ETileState.None,
                        ETileState.Built | ETileState.Road
                    );
            }
            else
            {
                CreateError("자원 부족함", true);
            }
            }
        else
        {
            CreateError("현재 상태에선 불가능합니다.", true);
        }
    }
    private void ActionCommon03(GameObject go, Vector3Int position)
    {
        _resources.PayCost(23, 0, 0, 0, 0, 0);
        Logger.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.Trees)]);
        _soundManager.PlaySE(EEffectSound.Success);
    }
    private void CallCommon04()
    {
        if (_gameState == EGameState.Idle || _gameState == EGameState.TileInspect)
        {
            if (CheckResource(18, 0, 0, 0, 0, 0))
            {
                ShowTilechecker
                    (
                        "해당 위치에 도로를 설치합니다.\n진행합니까?",
                        ActionCommon04,
                        ETileState.Road,
                        ETileState.Built
                    );
            }
            else
            {
                CreateError("자원 부족함", true);
            }
        }
        else
        {
            CreateError("현재 상태에선 불가능합니다.", true);
        }
    }
    private void ActionCommon04(GameObject go, Vector3Int position)
    {
        _resources.PayCost(18, 0, 0, 0, 0, 0);
        Logger.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.RoadBuilt)]);
        _soundManager.PlaySE(EEffectSound.Success);
    }
    private void CallCommon05()
    {
        if (_gameState == EGameState.Idle || _gameState == EGameState.TileInspect)
        {
            if (CheckResource(25, 0, 0, 0, 0, 0))
            {
                ShowTilechecker
                    (
                        "해당 위치에 간이 음식점을 건설합니다.\n진행합니까?",
                        ActionCommon05,
                        ETileState.None,
                        ETileState.Built | ETileState.Road,
                        1
                    );
            }
            else
            {
                CreateError("자원 부족함", true);
            }
        }
        else
        {
            CreateError("현재 상태에선 불가능합니다.", true);
        }
    }
    private void ActionCommon05(GameObject go, Vector3Int position)
    {
        _resources.PayCost(25, 0, 0, 0, 0, 0);
        _resources.moneyIncrease += 1;
        Logger.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.Foodbooth)]);
        findTileByPosition(position, out CTile tile);
        tile.TileInfo = "<sprite=1> 1";
        _soundManager.PlaySE(EEffectSound.Success);
    }

    private void CallGotoTitle()
    {
        ShowQuestion
            (
                "오늘 한 내용은 저장되지 않습니다.\n타이틀로 돌아갑니까?",
                ActionGotoTitle
            );
    }

    private void ActionGotoTitle(GameObject go, Vector3Int position)
    {
        _sceneManager.LoadScene(ESceneId.Title);
    }
}