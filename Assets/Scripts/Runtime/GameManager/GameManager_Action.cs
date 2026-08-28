using UnityEngine;

public partial class GameManager
{
    private void SetListener()
    {
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(
                () => CallUpgrade());
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
    }

    private void CallUpgrade()
    {
        CTile tempClass;
        if (_lastSelectedObject.TryGetComponent<CTile>(out tempClass))
        {
            if ((tempClass.TileState & ETileState.Upgradable) != ETileState.None)
            {
                if (CheckResource(tempClass.Cost) >= 0)
                {
                    if (_upgradeSe != null)
                    {
                        _upgradeSe.Play();
                    }
                    tempClass.Upgrade(_resources, this, _tileBases, _lastSelectedPosition);
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
        else
        {
            return;
        }
    }

    public void CallCard(CCard card)
    {
        if (card.AvailableToUse())
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
        else
        {
            CreateError("자원 부족함", true);
        }
    }
    public void DeleteCard(CCard card)
    {
        CPrint.Log($"삭제부르기 -> {card.IsDeletable}");
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
        card.UseCard(position, _questionIsTileSkiped);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }

    private void ActionCardDelete(CCard card, Vector3Int position)
    {
        card.DeleteCard();
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
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

    private void CallNextDay()
    {
        _resources.moneyCurrent += _resources.moneyIncrease;
        _resources.materialsCurrent += _resources.materialsIncrease;
        _resources.menpowerRemain += _resources.menpowerCurrent;
        while(_resources.menpowerRemain >= 8)
        {
            _resources.menpowerRemain -= 8;
            _resources.festivalInterest += 1;
        }
        _menpowerRamainsSlider.value = _resources.menpowerRemain / 8f;
        _resources.menpowerCurrent = _resources.menpowerIncrease;
    }

    private void CallAccept()
    {
        _questionValue = 1;
    }

    private void CallDecline()
    {
        _questionValue = -1;
    }

    private void CallCardAdd()
    {
        if (_cardHand.AddCard())
        {
            CPrint.Success("카드 추가됨");
        }
        else
        {
            CreateError("손패 꽉 참");
        }
    }

    private void CallCommon01()
    {
        if (CheckResource(11, 0, 0, 0, 0, 0) == 0)
        {
            ShowQuestion
                (
                    "비용을 써서 학생회 인원을 확충합니다.\n진행합니까?",
                    ActionCommon01
                );
        }
        else
        {
            CreateError("자원 부족함", true);
        }
    }

    private void ActionCommon01(GameObject go, Vector3Int position)
    {
        _resources.PayCost(11, 0, 0, 0, 0, 0);
        _resources.menpowerIncrease += 1;
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
    private void CallCommon02()
    {
        if (CheckResource(14, 0, 0, 0, 0, 0) == 0)
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

    private void ActionCommon02(GameObject go, Vector3Int position)
    {
        _resources.PayCost(14, 0, 0, 0, 0, 0);
        _resources.festivalInterest += 1;
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
    private void CallCommon03()
    {
        if (CheckResource(23, 0, 0, 0, 0, 0) == 0)
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

    private void ActionCommon03(GameObject go, Vector3Int position)
    {
        _resources.PayCost(23, 0, 0, 0, 0, 0);
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.Trees)]);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }

    private void CallCommon04()
    {
        if (CheckResource(18, 0, 0, 0, 0, 0) == 0)
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

    private void ActionCommon04(GameObject go, Vector3Int position)
    {
        _resources.PayCost(18, 0, 0, 0, 0, 0);
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.RoadBuilt)]);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
    private void CallCommon05()
    {
        if (CheckResource(25, 0, 0, 0, 0, 0) == 0)
        {
            ShowTilechecker
                (
                    "해당 위치에 부스를 건설합니다.\n진행합니까?",
                    ActionCommon05,
                    ETileState.None,
                    ETileState.Built | ETileState.Road
                );
        }
        else
        {
            CreateError("자원 부족함", true);
        }
    }

    private void ActionCommon05(GameObject go, Vector3Int position)
    {
        _resources.PayCost(25, 0, 0, 0, 0, 0);
        _resources.moneyIncrease += 1;
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETileCatalog.Booth)]);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
}