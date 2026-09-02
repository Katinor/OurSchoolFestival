using System.Collections.Generic;

public class TileFoodbooth : CTile
{
    public TileFoodbooth()
    {
        this._name = "간이 음식점";
        this._description = "축제에는 먹거리가 빠질 수 없다고들 하죠!\n인접한 가로수 만큼 축제 점수를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._radius = 1;
        this._tileInCatalog = ETileCatalog.Foodbooth;
    }
    public override SScoreInfo OnScore()
    {
        int tempCount = 0;
        List<CTile> tiles = _gameManager.FindNeighborTiles(_tilePosition, 1);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileInCatalog == ETileCatalog.Trees) tempCount += 1;
        }
        return new SScoreInfo(tempCount, _name);
    }
    public override void OnSelected()
    {
        SScoreInfo score = OnScore();
        _tileInfo = $"<sprite=9> {score.Score}";
        base.OnSelected();
        _additionalDescription = $"현재 인접한 가로수는 <b>{score.Score}</b>개 입니다.";
    }
}
