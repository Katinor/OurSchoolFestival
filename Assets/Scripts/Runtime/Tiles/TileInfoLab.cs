public class TileInfoLab : CTile
{
    public TileInfoLab()
    {
        this._name = "정보 처리실";
        this._description = "과학 동아리의 인공지능으로 새로운 아이디어를 얻어봅시다.";
        this._additionalDescription = "<size=150%>액션 : LLM 기동</size>\n<sprite=0> 2 사용\n카드를 한 장 뽑습니다.";
        this._tileState = ETileState.Built | ETileState.Action;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.InfoLab;
        this._actionName = "LLM 기동";
        this._actionCost = new SCost(2, 0, 0, 0, 0, 0);
        this._actionEnabled = true;
        this._actionUsed = false;
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
