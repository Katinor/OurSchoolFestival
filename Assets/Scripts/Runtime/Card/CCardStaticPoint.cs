using System;

public static partial class CCardStatic
{
    public static Func<GameManager, SScoreInfo> FindPointFunction(int level)
    {
        switch (level)
        {
            case 1:
                return CardPoint01;
            case 2:
                return CardPoint02;
            case 3:
                return CardPoint03;
            case 4:
                return CardPoint04;
            case 5:
                return CardPoint05;
            case 6:
                return CardPoint06;
            case 7:
                return CardPoint07;
            case 8:
                return CardPoint08;
            default:
                Logger.Error($"해당하는 함수 찾을 수 없음 : {level}");
                return null;
        }
    }

    public static SScoreInfo CardPoint01(GameManager manager)
    {
        return new SScoreInfo(2, "기타");
        // return new SScoreInfo(2, "환경미화");
    }

    public static SScoreInfo CardPoint02(GameManager manager)
    {
        return new SScoreInfo(2, "기타");
        // return new SScoreInfo(2, "기념 자연물");
    }
    public static SScoreInfo CardPoint03(GameManager manager)
    {
        return new SScoreInfo(3, "기타");
        // return new SScoreInfo(3, "과학 세미나");
    }
    public static SScoreInfo CardPoint04(GameManager manager)
    {
        return new SScoreInfo(1, "기타");
        // return new SScoreInfo(1, "가로수 대량수입");
    }
    public static SScoreInfo CardPoint05(GameManager manager)
    {
        return new SScoreInfo(1, "기타");
        // return new SScoreInfo(1, "과학부 출동!");
    }
    public static SScoreInfo CardPoint06(GameManager manager)
    {
        return new SScoreInfo(2, "기타");
        // return new SScoreInfo(2, "대규모 자재 후원");
    }
    public static SScoreInfo CardPoint07(GameManager manager)
    {
        return new SScoreInfo(2, "기타");
        // return new SScoreInfo(2, "미술품 공방");
    }
    public static SScoreInfo CardPoint08(GameManager manager)
    {
        return new SScoreInfo(2, "기타");
        // return new SScoreInfo(2, "길거리 라디오");
    }
}
