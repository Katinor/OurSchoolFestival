using System.Collections.Generic;

public class TileSportsHQ : CTile
{
    private bool _withElectronicSystem = false;
    private int _attractionCount = 0;
    private int _currentCost = 0;
    private int _currentScore = 0;

    public TileSportsHQ()
    {
        this._name = "체육 본부";
        this._description = "체육부원들이 어트랙션을 관리하고 있습니다. 어트랙션 2개 마다 액션의 코스트와 점수가 1 증가합니다.";
        this._additionalDescription = "<size=150%>액션 : 어트랙션 관리</size>\n<sprite=4> 2 사용\n<sprite=9>를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Action | ETileState.Point;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.SportsHQ;
        this._actionName = "어트랙션 관리";
        this._actionCost = new SCost(0, 0, 0, 0, 1, 0);
        this._internalPoints = 0;
        this._actionEnabled = true;
        this._actionUsed = false;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(_internalPoints, _name);
    }

    public override void OnSelected()
    {
        _tileInfo = $"<sprite=9> {_internalPoints}";
        _attractionCount = 0;
        _withElectronicSystem = false;
        List<CTile> tiles = _gameManager.GetAllTiles();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileInCatalog == ETileCatalog.SportsAttraction)
            {
                _attractionCount++;
            }
            else if (tiles[i].TileInCatalog == ETileCatalog.ElectronicSystem)
            {
                _attractionCount++;
                _withElectronicSystem = true;
            }
        }

        _currentScore = 1 + (_attractionCount / 2) + (_gameManager.GetTech(ETech.Sports) >= 5 ? 1 : 0);
        _currentCost = 2 + (_attractionCount / 2) - (_withElectronicSystem ? 1 : 0);

        _actionCost = new SCost(0, 0, 0, 0, _currentCost, 0);
        _additionalDescription = $"<size=150%>액션 : 어트랙션 관리</size>\n<sprite=4> {_currentCost} 사용\n<sprite=9>를 {_currentScore} 얻습니다.";
        base.OnSelected();
    }
    public override bool OnAction(GameManager gameManager)
    {
        Logger.V3($"{_name} : 액션 발동", _tilePosition);
        gameManager.PayCost(_actionCost);
        _internalPoints += _currentScore;
        _tileInfo = $"<sprite=9> {_internalPoints}";
        ShowParticle(0, 0, 0.5f);
        _actionUsed = true;
        return true;
    }
}
