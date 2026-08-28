using System;

public struct SCost
{
    public int moneyCurrent;
    public int moneyIncrease;
    public int materialsCurrent;
    public int materialsIncrease;
    public int menpowerCurrent;
    public int menpowerIncrease;
    public bool canUseMaterials;

    public SCost(int moneyCurrent, int moneyIncrease, int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease)
    {
        this.moneyCurrent = moneyCurrent;
        this.moneyIncrease = moneyIncrease;
        this.materialsCurrent = materialsCurrent;
        this.materialsIncrease = materialsIncrease;
        this.menpowerCurrent = menpowerCurrent;
        this.menpowerIncrease = menpowerIncrease;
        this.canUseMaterials = false;
    }

    public SCost(int moneyCurrent, int moneyIncrease, int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease, bool canUseMaterials)
        : this(moneyCurrent, moneyIncrease, materialsCurrent, materialsIncrease, menpowerCurrent, menpowerIncrease)
    {
        this.canUseMaterials = canUseMaterials;
    }

    public SCost(SCostInfo costInfo, bool canUseMaterials)
        : this(costInfo.moneyCurrent, costInfo.moneyIncrease, costInfo.materialsCurrent, costInfo.materialsIncrease,
              costInfo.menpowerCurrent, costInfo.menpowerIncrease, canUseMaterials)
    {
    
    }

    public string toCostString()
    {
        string tempString = "";
        if (this.moneyCurrent > 0) tempString += $"<sprite=0> {this.moneyCurrent}";
        if (this.canUseMaterials) tempString += $"(<sprite=2> 가능)";
        if (this.moneyIncrease > 0) tempString += $"<sprite=1> {this.moneyIncrease}";
        if (this.materialsCurrent > 0) tempString += $"<sprite=2> {this.materialsCurrent}";
        if (this.materialsIncrease > 0) tempString += $"<sprite=3> {this.materialsIncrease}";
        if (this.menpowerCurrent > 0) tempString += $"<sprite=4> {this.menpowerCurrent}";
        if (this.menpowerIncrease > 0) tempString += $"<sprite=5> {this.menpowerIncrease}";
        return tempString;
    }
}

[Serializable]
public struct TechData
{
    public ETech tag;
    public int level;
}

[Serializable]
public struct ActionData
{
    public EAction action;
    public int level;
}

[Serializable]
public struct SCostInfo
{
    public int moneyCurrent;
    public int moneyIncrease;
    public int materialsCurrent;
    public int materialsIncrease;
    public int menpowerCurrent;
    public int menpowerIncrease;
}

public class CResources
{
    public int moneyCurrent;
    public int moneyIncrease;
    public int materialsCurrent;
    public int materialsIncrease;
    public int menpowerCurrent;
    public int menpowerIncrease;
    public int menpowerRemain;
    public int festivalSuccess;
    public int festivalInterest;
    public int festivalRoad;

    public CResources(int moneyCurrent, int moneyIncrease, int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease,
        int menpowerRemain = 0, int festivalSuccess = 0, int festivalInterest = 0, int festivalRoad = 0)
    {
        this.moneyCurrent = moneyCurrent;
        this.moneyIncrease = moneyIncrease;
        this.materialsCurrent = materialsCurrent;
        this.materialsIncrease = materialsIncrease;
        this.menpowerCurrent = menpowerCurrent;
        this.menpowerIncrease = menpowerIncrease;
        this.menpowerRemain = menpowerRemain;
        this.festivalSuccess = festivalSuccess;
        this.festivalInterest = festivalInterest;
        this.festivalRoad = festivalRoad;
    }

    public void PayCost(SCost cost)
    {
        this.moneyCurrent -= cost.moneyCurrent;
        this.moneyIncrease -= cost.moneyIncrease;
        this.materialsCurrent -= cost.materialsCurrent;
        this.materialsIncrease -= cost.materialsIncrease;
        this.menpowerCurrent -= cost.menpowerCurrent;
        this.menpowerIncrease -= cost.menpowerIncrease;
    }
    public void PayCost(int moneyCurrent, int moneyIncrease, int materialsCurrent, int materialsIncrease, int menpowerCurrent, int menpowerIncrease)
    {
        PayCost(new SCost(moneyCurrent, moneyIncrease, materialsCurrent, materialsIncrease, menpowerCurrent, menpowerIncrease));
    }
}

public enum ETech
{
    Structure,  // 자재로 지불 가능
    Success,    // 축제 완성도 필요카드 (양수는 이상, 음수는 이하)
    Interest,   // 축제 관심도 필요카드 (양수는 이상, 음수는 이하)
    Road,       // 축제 안정도 필요카드 (양수는 이상, 음수는 이하)
    Science,    // 과학 필요카드
    Music,      // 음악 필요카드
    Art,        // 미술 필요카드
    Exercise,   // 친목 필요카드
    Cult,       // 사교 필요카드
}

public enum EAction
{
    moneyCurrent,
    moneyIncrease,
    materialsCurrent,
    materialsIncrease,
    menpowerCurrent,
    menpowerIncrease,
    Success,
    Interest,
    Road,
    Science,
    Music,
    Art,
    Exercise,
    Cult,
    Tile,
    CustomScript
}