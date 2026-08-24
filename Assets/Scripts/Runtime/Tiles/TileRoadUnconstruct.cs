using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRoadUnconstruct : CTile
{
    public TileRoadUnconstruct()
    {
        this._name = "도로터";
        this._description = "도로를 짓기 좋은 땅입니다.";
        this._tileState = ETileState.Road | ETileState.Upgradable;
        this._upgradeResult = ETempTileCatalog.RoadBuilt;
    }
}
