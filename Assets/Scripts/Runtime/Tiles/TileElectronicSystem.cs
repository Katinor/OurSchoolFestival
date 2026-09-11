public class TileElectronicSystem : CTile
{
    public TileElectronicSystem()
    {
        this._name = "체육 지원서버";
        this._description = "체육 본부를 강화합니다.\n추가로 체육 본부의 사용 코스트가 1 감소합니다.\n축제 점수를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._internalPoints = 1;
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInfo = "<sprite=9> 1";
        this._tileInCatalog = ETileCatalog.ElectronicSystem;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(1, "체육");
    }
}
