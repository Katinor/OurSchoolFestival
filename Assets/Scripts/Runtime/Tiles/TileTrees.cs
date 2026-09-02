public class TileTrees : CTile
{
    public TileTrees()
    {
        this._name = "가로수";
        this._description = "때로는 주변의 자연이 축제를 완성시키기도 합니다.\n축제 점수를 1 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._internalPoints = 1;
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInfo = "<sprite=9> 1";
        this._tileInCatalog = ETileCatalog.Trees;
    }

    protected override void Start()
    {
        base.Start();
        _gameManager.Resources.festivalSuccess += 1;
    }

    public override SScoreInfo OnScore()
    {
        return new SScoreInfo(1, _name);
    }
}
