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
                if (_upgradeSe != null)
                {
                    _upgradeSe.Play();
                }
                tempClass.Upgrade(_tilemap, _tileBases, _lastSelectedPosition);
                OnClickElse();
                _gameState = EGameState.Idle;
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
        _moneyCurrent += _moneyIncrease;
        _materialsCurrent += _materialsIncrease;
        _menpowerRemains += _menpowerCurrent;
        while(_menpowerRemains >= 8)
        {
            _menpowerRemains -= 8;
            _festivalInterest += 1;
        }
        _menpowerRamainsSlider.value = _menpowerRemains / 8f;
        _menpowerCurrent = _menpowerIncrease;
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
            CreateError("비용 부족함!");
        }
    }

    private void ActionCommon01(GameObject go, Vector3Int position)
    {
        _moneyCurrent -= 11;
        _menpowerIncrease += 1;
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
            CreateError("비용 부족함!");
        }
    }

    private void ActionCommon02(GameObject go, Vector3Int position)
    {
        _moneyCurrent -= 14;
        _festivalInterest += 1;
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
            CreateError("비용 부족함!");
        }
    }

    private void ActionCommon03(GameObject go, Vector3Int position)
    {
        _moneyCurrent -= 23;
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
            CreateError("비용 부족함!");
        }
    }

    private void ActionCommon04(GameObject go, Vector3Int position)
    {
        _moneyCurrent -= 18;
        CPrint.V3("타일 지을 곳", position);
        _tilemap.SetTile(position, _tileBases[GetIndex(ETempTileCatalog.RoadBuilt)]);
        if (_upgradeSe != null)
        {
            _upgradeSe.Play();
        }
    }
}