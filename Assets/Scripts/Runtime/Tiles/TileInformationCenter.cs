using System.Collections.Generic;

public class TileInformationCenter : CTile
{
    public TileInformationCenter()
    {
        this._name = "축제 안내소";
        this._description = "나름 부지가 넓다보니, 안내를 전문적으로 할 시설이 필요합니다.";
        this._additionalDescription = "<size=150%>액션 : 선배의 후원</size>\n<sprite=4> 1 사용\n<sprite=0>을 5 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Action;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.InformationCenter;
        this._actionName = "선배의 후원";
        this._actionCost = new SCost(0, 0, 0, 0, 1, 0);
        this._actionEnabled = true;
        this._actionUsed = false;
    }

    public override bool OnAction(GameManager gameManager)
    {
        Logger.V3($"{_name} : 액션 발동", _tilePosition);
        gameManager.PayCost(_actionCost);
        gameManager.Resources.moneyCurrent += 5;
        ShowParticle(0, 0, 0.5f);
        _actionUsed = true;
        return true;
    }
}
