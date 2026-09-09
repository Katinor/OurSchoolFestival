using System;
using UnityEngine;

public static partial class CCardStatic
{
    public static bool CardEmpty(GameManager manager, int level)
    {
        return true;
    }
    public static bool CardMoneyCurrent(GameManager manager, int level)
    {
        manager.Resources.moneyCurrent += level;
        return true;
    }

    public static bool CardMoneyIncrease(GameManager manager, int level)
    {
        manager.Resources.moneyIncrease += level;
        return true;
    }

    public static bool CardMaterialsCurrent(GameManager manager, int level)
    {
        manager.Resources.materialsCurrent += level;
        return true;
    }

    public static bool CardMaterialsIncrease(GameManager manager, int level)
    {
        manager.Resources.materialsIncrease += level;
        return true;
    }

    public static bool CardMenpowerCurrent(GameManager manager, int level)
    {
        manager.Resources.menpowerCurrent += level;
        return true;
    }

    public static bool CardMenpowerIncrease(GameManager manager, int level)
    {
        manager.Resources.menpowerIncrease += level;
        return true;
    }
    public static bool CardSuccess(GameManager manager, int level)
    {
        manager.Resources.festivalSuccess += level;
        return true;
    }
    public static bool CardInterest(GameManager manager, int level)
    {
        manager.Resources.festivalInterest += level;
        return true;
    }
    public static bool CardRoad(GameManager manager, int level)
    {
        manager.Resources.festivalRoad += level;
        return true;
    }

    public static bool CardDraw(GameManager manager, int level)
    {
        manager.DrawCards(level);
        return true;
    }

    public static bool CardScience(GameManager manager, int level)
    {
        return CardTech(manager, ETech.Science, level);
    }

    public static bool CardMusic(GameManager manager, int level)
    {
        return CardTech(manager, ETech.Music, level);
    }

    public static bool CardArt(GameManager manager, int level)
    {
        return CardTech(manager, ETech.Art, level);
    }

    public static bool CardExercise(GameManager manager, int level)
    {
        return CardTech(manager, ETech.Exercise, level);
    }

    public static bool CardCult(GameManager manager, int level)
    {
        return CardTech(manager, ETech.Cult, level);
    }

    public static bool CardTile(GameManager manager, ETileCatalog tile, Vector3Int position)
    {
        return false;
    }

    public static bool CardIllust(GameManager manager, int level)
    {
        manager.ShowIllust(level);
        return true;
    }

    private static bool CardTech(GameManager manager, ETech tech, int level)
    {
        bool returnValue = CardTechValue(manager, tech, level);
        manager.ReloadTech();
        return returnValue;
    }

    private static bool CardTechValue(GameManager manager, ETech tech, int level)
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
    public static Func<GameManager, SScoreInfo> FindPointFunction(int level)
    {
        return (GameManager manager) =>
        {
            int source = Math.Abs(level / 100);
            switch (source)
            {
                case 0:
                    return new SScoreInfo(level % 100, "공용");
                case 1:
                    return new SScoreInfo(level % 100, "과학");
                case 2:
                    return new SScoreInfo(level % 100, "음악");
                case 3:
                    return new SScoreInfo(level % 100, "미술");
                case 4:
                    return new SScoreInfo(level % 100, "운동");
                case 5:
                    return new SScoreInfo(level % 100, "미스터리");
                default:
                    Logger.Error($"{level} => 카드점수 값이 잘못됨.");
                    return new SScoreInfo(level, "공용");
            }
        };
    }
}
