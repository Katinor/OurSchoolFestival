public static class CCardStatic
{
    public static bool CardMoneyCurrent(Test_TilemapSelector manager, int level)
    {
        manager.Resources.moneyCurrent += level;
        return true;
    }

    public static bool CardMoneyIncrease(Test_TilemapSelector manager, int level)
    {
        manager.Resources.moneyIncrease += level;
        return true;
    }

    public static bool CardMaterialsCurrent(Test_TilemapSelector manager, int level)
    {
        manager.Resources.materialsCurrent += level;
        return true;
    }

    public static bool CardMaterialsIncrease(Test_TilemapSelector manager, int level)
    {
        manager.Resources.materialsIncrease += level;
        return true;
    }

    public static bool CardMenpowerCurrent(Test_TilemapSelector manager, int level)
    {
        manager.Resources.menpowerCurrent += level;
        return true;
    }

    public static bool CardMenpowerIncrease(Test_TilemapSelector manager, int level)
    {
        manager.Resources.menpowerIncrease += level;
        return true;
    }
    public static bool CardSuccess(Test_TilemapSelector manager, int level)
    {
        manager.Resources.festivalSuccess += level;
        return true;
    }
    public static bool CardInterest(Test_TilemapSelector manager, int level)
    {
        manager.Resources.festivalInterest += level;
        return true;
    }
    public static bool CardRoad(Test_TilemapSelector manager, int level)
    {
        manager.Resources.festivalRoad += level;
        return true;
    }

    public static bool CardScience(Test_TilemapSelector manager, int level)
    {
        return CardTech(manager, ETech.Science, level);
    }

    public static bool CardMusic(Test_TilemapSelector manager, int level)
    {
        return CardTech(manager, ETech.Music, level);
    }

    public static bool CardArt(Test_TilemapSelector manager, int level)
    {
        return CardTech(manager, ETech.Art, level);
    }

    public static bool CardExercise(Test_TilemapSelector manager, int level)
    {
        return CardTech(manager, ETech.Exercise, level);
    }

    public static bool CardCult(Test_TilemapSelector manager, int level)
    {
        return CardTech(manager, ETech.Cult, level);
    }

    public static bool CardTile(Test_TilemapSelector manager, int level)
    {
        return false;
    }

    public static bool CardCustom(Test_TilemapSelector manager, int level)
    {
        return false;
    }

    private static bool CardTech(Test_TilemapSelector manager, ETech tech, int level)
    {
        bool returnValue = CardTechValue(manager, tech, level);
        manager.ReloadTech();
        return returnValue;
    }

    private static bool CardTechValue(Test_TilemapSelector manager, ETech tech, int level)
    {
        if (!manager.CurrentTech.ContainsKey(tech) && level > 0)
        {
            manager.CurrentTech[tech] = level;
            return true;
        }

        manager.CurrentTech[tech] += level;

        if (manager.CurrentTech[tech] < 0)
        {
            manager.CreateError($"테크 값 에러 : {manager.CurrentTech[tech]}");
            return false;
        }
        else if (manager.CurrentTech[tech] == 0)
        {
            manager.CurrentTech.Remove(tech);
        }
        return true;
    }
}
