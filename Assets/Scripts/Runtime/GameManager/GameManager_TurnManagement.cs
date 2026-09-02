using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GameManager
{
    private void CallNextDay()
    {
        _gameState = EGameState.NextDay;
        StartCoroutine(CallNextDayCoroutine());

    }
    private IEnumerator CallNextDayCoroutine()
    {
        yield return _DayManager.StartDayResult(this, _soundManager);
        _currentDay++;
        SetDayButton(_currentDay);
        _resources.moneyCurrent += _resources.moneyIncrease + GetFestivalScore();
        _resources.materialsCurrent += _resources.materialsIncrease;
        _resources.menpowerRemain += _resources.menpowerCurrent;
        while (_resources.menpowerRemain >= 8)
        {
            _resources.menpowerRemain -= 8;
            _resources.festivalInterest += 1;
        }
        _menpowerRamainsSlider.value = _resources.menpowerRemain / 8f;
        _resources.menpowerCurrent = _resources.menpowerIncrease;
        _cardHand.AddCards(4);
        _gameState = EGameState.Idle;
    }

    public int GetFestivalScore()
    {
        return Clamp(_resources.festivalSuccess, 0, 14) + Clamp(_resources.festivalInterest, 0, 19) + Clamp(_resources.festivalRoad, 0, 8);
    }

    public string GetFestivalDesc()
    {
        return $"완성도 {Clamp(_resources.festivalSuccess, 0, 14)} / 관심도 {Clamp(_resources.festivalInterest, 0, 19)} / 안정도 {Clamp(_resources.festivalRoad, 0, 8)}";
    }

    public void CalculateScore()
    {
        _scoreSet.tileScore = CalcTileScore();
        _scoreSet.cardScore = CalcCardScore();
        _scoreSet.achievementScore = CalcAchievementScore();
    }

    private SScoreInfo CalcTileScore()
    {
        string description = "";
        Dictionary<string, int> scoreDict = new Dictionary<string, int>();
        int score = 0;
        List<CTile> tiles = GetAllTiles();
        foreach (CTile tile in tiles)
        {
            if ((tile.TileState & ETileState.Point) != ETileState.None)
            {
                SScoreInfo tileScoreInfo = tile.OnScore();
                if (scoreDict.ContainsKey(tileScoreInfo.Description))
                {
                    scoreDict[tileScoreInfo.Description] += tileScoreInfo.Score;
                }
                else
                {
                    scoreDict[tileScoreInfo.Description] = tileScoreInfo.Score;
                }
                score += tileScoreInfo.Score;
            }
        }
        
        List<string> keys = new List<string>(scoreDict.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (i == keys.Count - 1)
            {
                description += $"{keys[i]}: {scoreDict[keys[i]]}";
            }
            else
            {
                description += $"{keys[i]}: {scoreDict[keys[i]]}, ";
            }
        }
        return new SScoreInfo(score,  description);
    }

    private SScoreInfo CalcCardScore()
    {
        string description = "";
        Dictionary<string, int> scoreDict = new Dictionary<string, int>();
        int score = 0;
        for (int i = 0; i < _cardScores.Count; i++)
        {
            SScoreInfo cardScoreInfo = _cardScores[i](this);
            if (scoreDict.ContainsKey(cardScoreInfo.Description))
            {
                scoreDict[cardScoreInfo.Description] += cardScoreInfo.Score;
            }
            else
            {
                scoreDict[cardScoreInfo.Description] = cardScoreInfo.Score;
            }
            score += cardScoreInfo.Score;
        }
        List<string> keys = new List<string>(scoreDict.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (i == keys.Count - 1)
            {
                description += $"{keys[i]}: {scoreDict[keys[i]]}";
            }
            else
            {
                description += $"{keys[i]}: {scoreDict[keys[i]]}, ";
            }
        }
        return new SScoreInfo(score, description);
    }

    private SScoreInfo CalcAchievementScore()
    {
        return new SScoreInfo(0, "");
    }
    public SScoreInfo GetTileScore()
    {
        return _scoreSet.tileScore;
    }

    public SScoreInfo GetCardScore()
    {
        return _scoreSet.cardScore;
    }

    public SScoreInfo GetAchievementScore()
    {
        return _scoreSet.achievementScore;
    }

    public int GetTotalScore()
    {
        return GetFestivalScore() + _scoreSet.tileScore.Score + _scoreSet.cardScore.Score + _scoreSet.achievementScore.Score;
    }
}
