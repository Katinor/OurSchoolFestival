using System;
using System.Collections.Generic;

public static partial class CCardStatic
{
    public static Func<GameManager, int, bool> CardCustom(int level)
    {
        switch (level)
        {
            case 1:
                return CardCustom01;
            default:
                Logger.Error($"해당하는 함수 찾을 수 없음 : {level}");
                return null;
        }
    }

    public static bool CardCustom01(GameManager manager, int level)
    {
        int tempCount = 0;
        List<CTile> tiles = manager.GetAllTiles();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileInCatalog == ETileCatalog.Booth)  tempCount += 1;
        }
        if (tempCount >= 3) manager.Resources.materialsIncrease += 3;
        return true;
    }
}
