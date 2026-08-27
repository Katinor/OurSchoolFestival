using UnityEngine;

public partial class Test_TilemapSelector
{
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
                    tempClass.Upgrade(_resources, _tilemap, _tileBases, _lastSelectedPosition);
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
            ShowQuestion
                (
                    $"{card.CardName}을 사용합니까?",
                    ActionCard,
                    card
                );
        }
        else
        {
            CreateError("자원 부족함", true);
        }
    }

    private void ActionCard(CCard card)
    {
        card.UseCard();
        Destroy(card.gameObject);
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
        _resources.moneyCurrent -= 11;
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
        _resources.moneyCurrent -= 14;
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
        _resources.moneyCurrent -= 23;
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETempTileCatalog.Trees)]);
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
        _resources.moneyCurrent -= 18;
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETempTileCatalog.RoadBuilt)]);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
}