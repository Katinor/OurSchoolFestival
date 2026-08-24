using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRoadBuilt : CTile
{
    public TileRoadBuilt()
    {
        this._name = "도로";
        this._description = "축제의 중심으로 뻗은 도로는 축제의 안정도를 크게 높여줍니다.";
        this._tileState = ETileState.Road | ETileState.Built;
    }
}
