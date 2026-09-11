public class TileArtistMasterpiece : CTile
{
    public TileArtistMasterpiece()
    {
        this._name = "미술 작품";
        this._description = "미술 동아리의 혼신의 역작입니다!\n축제 점수를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._internalPoints = 1;
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInfo = "<sprite=9> 1";
        this._tileInCatalog = ETileCatalog.ArtistMasterpiece;
    }

    protected override void Start()
    {
        base.Start();
        if (!_isLoaded) _gameManager.Resources.festivalSuccess += 1;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(1, "미술");
    }
}
