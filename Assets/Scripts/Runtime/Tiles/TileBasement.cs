using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBasement : CTile
{
    public TileBasement()
    {
        this._name = "공터";
        this._description = "아무것도 없는 공터입니다. 도로를 제외한 모든 것을 설치할 수 있습니다.";
        this._tileState = ETileState.Upgradable;
        this._upgradeResult = ETempTileCatalog.Trees;
    } 
}
