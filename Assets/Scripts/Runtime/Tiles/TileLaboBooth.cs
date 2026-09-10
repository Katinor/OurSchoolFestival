public class TileLaboBooth : CTile
{
    public TileLaboBooth()
    {
        this._name = "실험 부스";
        this._description = "카티노르의 지도 아래, 과학 동아리의 부원들이 열심히 아이디어를 모으고 있습니다.";
        this._additionalDescription = "<size=150%>액션 : 브레인스토밍</size>\n<sprite=4> 1 사용\n카드를 한 장 뽑습니다.";
        this._tileState = ETileState.Built | ETileState.Action;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.LaboBooth;
        this._actionName = "브레인스토밍";
        this._actionCost = new SCost(0, 0, 0, 0, 1, 0);
        this._actionEnabled = true;
        this._actionUsed = false;
    }
    public override void OnSelected()
    {
        _actionEnabled = _gameManager.CanDraw(1);
        base.OnSelected();
    }
    public override bool OnAction(GameManager gameManager)
    {
        Logger.V3($"{_name} : 액션 발동", _tilePosition);
        gameManager.PayCost(_actionCost);
        gameManager.DrawCards(1);
        ShowParticle(0, 0, 0.5f);
        _actionUsed = true;
        return true;
    }
}
