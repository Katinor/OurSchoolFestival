public class TileRoadUnconstruct : CTile
{
    public TileRoadUnconstruct()
    {
        this._name = "도로터";
        this._description = "도로를 짓기 좋은 땅입니다.";
        this._tileState = ETileState.Road | ETileState.Upgradable;
        this._upgradeResult = ETileCatalog.RoadBuilt;
        this._upgradeCost = new SCost(18, 0, 0, 0, 0, 0);
        this._baseColor = new UnityEngine.Color(0f, 0f, 1f);
        this._tileInCatalog = ETileCatalog.RoadBase;
    }
}
