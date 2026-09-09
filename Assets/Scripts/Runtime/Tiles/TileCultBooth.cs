using System.Collections.Generic;

public class TileCultBooth : CTile
{
    public TileCultBooth()
    {
        this._name = "미스터리 본부";
        this._description = "피노가 자주 쓰는 부스입니다. 미스터리한 물건들이 가득합니다.";
        this._additionalDescription = "<size=150%>액션 : 미스터리 탐험</size>\n<sprite=4> 1 사용\n미스터리 카드를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Action;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.CultBooth;
        this._actionName = "미스터리 탐험";
        this._actionCost = new SCost(0, 0, 0, 0, 1, 0);
        this._actionEnabled = true;
        this._actionUsed = false;
    }

    public override bool OnAction(GameManager gameManager)
    {
        Logger.V3($"{_name} : 액션 발동", _tilePosition);
        gameManager.PayCost(_actionCost);
        List<CTile> tileList = gameManager.GetAllTiles();
        gameManager.GetMysteryCard();
        ShowParticle(0, 0, 0.5f);
        _actionUsed = true;
        if (!gameManager.MysteryAvailable()) _actionEnabled = false;
        return true;
    }

    public override void OnSelected()
    {
        if (!_gameManager.MysteryAvailable()) _actionEnabled = false;
        base.OnSelected();
    }
}
