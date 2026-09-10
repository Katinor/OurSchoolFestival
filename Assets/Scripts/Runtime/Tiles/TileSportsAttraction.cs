public class TileSportsAttraction : CTile
{
    public TileSportsAttraction()
    {
        this._name = "체육 어트랙션";
        this._description = "체육 본부를 강화합니다.\n축제 점수를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._internalPoints = 1;
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInfo = "<sprite=9> 1";
        this._tileInCatalog = ETileCatalog.SportsAttraction;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(1, _name);
    }
}
