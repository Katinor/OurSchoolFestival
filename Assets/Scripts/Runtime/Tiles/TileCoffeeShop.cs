using System.Collections.Generic;

public class TileCoffeeShop : CTile
{
    public TileCoffeeShop()
    {
        this._name = "출장 음료부스";
        this._description = "축제에 먹거리만 있다면 목이 막힐테니, 음료수도 함께 마셔봅시다!";
        this._additionalDescription = "<size=150%>액션 : 음료 판매</size>\n<sprite=4> 1 사용\n설치된 모든 간이 음식점만큼 <sprite=0>을 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Action;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._tileInCatalog = ETileCatalog.CoffeeBooth;
        this._actionName = "음료 판매";
        this._actionCost = new SCost(0, 0, 0, 0, 1, 0);
        this._actionEnabled = true;
        this._actionUsed = false;
    }

    public override bool OnAction(GameManager gameManager)
    {
        CPrint.V3($"{_name} : 액션 발동", _tilePosition);
        gameManager.PayCost(_actionCost);
        List<CTile> tileList = gameManager.GetAllTiles();
        int count = 0;
        for(int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].TileInCatalog == ETileCatalog.Foodbooth) count++;
        }
        gameManager.Resources.moneyCurrent += count;
        ShowParticle(0, 0, 0.5f);
        _actionUsed = true;
        return true;
    }
}
