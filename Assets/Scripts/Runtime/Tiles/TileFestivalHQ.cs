using System.Collections.Generic;

public class TileFestivalHQ : CTile
{
    public TileFestivalHQ()
    {
        this._name = "축제 본부";
        this._description = "축제의 심장입니다!\n인접한 모든 타일만큼 축제 점수를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Text | ETileState.Point;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._radius = 1;
        this._tileInCatalog = ETileCatalog.FestivalHQ;
    }
    public override SScoreInfo OnScore()
    {
        int tempCount = 0;
        List<CTile> tiles = _gameManager.FindNeighborTiles(_tilePosition, 1);
        for (int i = 0; i < tiles.Count; i++)
        {
            if ((tiles[i].TileState & ETileState.Built) != ETileState.None) tempCount += 1;
        }
        return new SScoreInfo(tempCount, _name);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        SScoreInfo score = OnScore();
        _tileInfo = $"<sprite=9> {score.Score}";
        _additionalDescription = $"현재 인접한 타일은 <b>{score.Score}</b>개 입니다.";
    }
}
