public class TileRoadBuilt : CTile
{
    public TileRoadBuilt()
    {
        this._name = "도로";
        this._description = "축제의 중심으로 뻗은 도로는 축제의 안정도를 크게 높여줍니다.";
        this._tileState = ETileState.Road | ETileState.Built;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(0f, 0f, 1f);
        this._tileInCatalog = ETileCatalog.RoadBuilt;
    }

    protected override void Start()
    {
        base.Start();
        _gameManager.Resources.festivalRoad += 1;
    }
}
