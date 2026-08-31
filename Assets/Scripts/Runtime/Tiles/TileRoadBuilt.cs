public class TileRoadBuilt : CTile
{
    public TileRoadBuilt()
    {
        this._name = "도로";
        this._description = "축제의 중심으로 뻗은 도로는 축제의 안정도를 크게 높여줍니다.";
        this._tileState = ETileState.Road | ETileState.Built | ETileState.Text;
        this._tileInfo = "<sprite=8> 1";
        this._tileInCatalog = ETileCatalog.RoadBuilt;
    }

    protected override void Start()
    {
        base.Start();
        _gameManager.Resources.festivalRoad += 1;
    }
}
