using System.Collections.Generic;

public class TileArtMuseum : CTile
{
    public TileArtMuseum()
    {
        this._name = "전시회 본부";
        this._description = "파견나온 미술동아리 부원들의 본부입니다.\n반경 2 이내의 작품만큼 축제 점수를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._radius = 2;
        this._tileInCatalog = ETileCatalog.ArtMuseum;
    }
    public override SScoreInfo OnScore()
    {
        int tempCount = 0;
        List<CTile> tiles;
        if (_gameManager.GetTech(ETech.Art) >= 5)
        {
            this._description = "파견나온 미술동아리 부원들의 본부입니다.\n반경 2 이내의 작품만큼 축제 점수를 얻습니다.\n반경 1 이내의 작품만큼 추가로 축제 점수를 얻습니다.";
            tiles = _gameManager.FindNeighborTiles(_tilePosition, 1);
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].TileInCatalog == ETileCatalog.ArtistMasterpiece) tempCount += 1;
            }
        }
        tiles = _gameManager.FindNeighborTiles(_tilePosition, 2);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileInCatalog == ETileCatalog.ArtistMasterpiece) tempCount += 1;
        }
        return new SScoreInfo(tempCount, "미술");
    }
    public override void OnSelected()
    {
        SScoreInfo score = OnScore();
        _tileInfo = $"<sprite=9> {score.Score}";
        _additionalDescription = $"현재 산출 점수는 <b>{score.Score}</b>점 입니다.";
        base.OnSelected();
    }
}
