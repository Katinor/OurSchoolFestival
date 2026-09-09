public class TileSoundRelay : CTile
{
    public TileSoundRelay()
    {
        this._name = "음향 중계기";
        this._description = "버스킹 부스의 소리를 전달합니다!\n축제 점수를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._internalPoints = 1;
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInfo = "<sprite=9> 1";
        this._tileInCatalog = ETileCatalog.SoundRelay;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(1, _name);
    }
}
