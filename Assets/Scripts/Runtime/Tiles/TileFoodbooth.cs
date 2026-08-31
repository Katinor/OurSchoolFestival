using System.Collections.Generic;

public class TileFoodbooth : CTile
{
    public TileFoodbooth()
    {
        this._name = "간이 음식점";
        this._description = "축제에는 먹거리가 빠질 수 없다고들 하죠!\n인접한 가로수 만큼 축제 점수를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Text | ETileState.Point;
        this.TileInfo = "";
        this._tileInCatalog = ETileCatalog.Foodbooth;
    }
    public override int OnScore()
    {
        int tempCount = 0;
        List<CTile> tiles = _gameManager.FindNeighborTiles(_tilePosition, 1);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileInCatalog == ETileCatalog.Trees) tempCount += 1;
        }
        return tempCount;
    }
}
