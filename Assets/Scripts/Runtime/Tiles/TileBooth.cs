public class TileBooth : CTile
{
    public TileBooth()
    {
        this._name = "부스";
        this._description = "다양한 활동이 이루어지고 있는 부스입니다.";
        this._tileState = ETileState.Built | ETileState.Text;
        this.TileInfo = "";
        this._tileInCatalog = ETileCatalog.Booth;
    }
}
