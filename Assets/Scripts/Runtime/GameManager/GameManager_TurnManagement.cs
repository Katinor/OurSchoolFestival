using System;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager
{
    private void CallNextDay()
    {
        _gameState = EGameState.NextDay;
        StartCoroutine(CallNextDayCoroutine());
    }
    private IEnumerator CallNextDayCoroutine()
    {
        yield return _DayManager.StartCoroutine(_DayManager.LoadingScreenOn());
        yield return _DayManager.StartDayResult(this, _soundManager);
        List<CTile> tileList = GetAllTiles();
        for(int i = 0; i < tileList.Count; i++)
        {
            tileList[i].ActionUsed = false;
        }
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

        _randomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(_randomSeed);
        _cardHand.AddCards(4);
        _currentDay++;
        SetDayButton(_currentDay);
        if(_currentDay == 6) _soundManager.PlayBGM(EBackgroundSound.Part2);
        if(_currentDay == 11) _soundManager.PlayBGM(EBackgroundSound.Part3);
        SaveData();
        yield return _DayManager.StartCoroutine(_DayManager.LoadingScreenOff());
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
        _scoreTotal = GetFestivalScore() + _scoreSet.tileScore.Score + _scoreSet.cardScore.Score + _scoreSet.achievementScore.Score;
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
        return _scoreTotal;
    }

    private void SaveData()
    {
        (List<int> tileIdList, List<int> tilePointList) = GetAllTilesByInt();
        SaveManager.SaveData(_saveSlot, new CSaveData
        (
            _version,
            _randomSeed,
            _currentDay,
            _resources,
            _currentTech,
            _cardScoresList,
            _cardHand.GetAllHandByInt(),
            _cardHand.GetCardDeckByInt(),
            tileIdList,
            tilePointList,
            _scoreTotal
            ));
    }

    private void LoadData()
    {
        StartCoroutine(LoadDataCoroutine());
    }

    private IEnumerator LoadDataCoroutine()
    {
        yield return _DayManager.StartCoroutine(_DayManager.LoadingScreenOn(0.5f));
        LoadDataCore();
        yield return _DayManager.StartCoroutine(_DayManager.LoadingScreenOff(0.5f));
    }

    private void LoadDataCore()
    {
        CSaveData savedData = SaveManager.LoadData(_saveSlot);
        if (_version != savedData.Version) Logger.Error("버전이 틀립니다.");
        _randomSeed = savedData.RandomSeed;
        _currentDay = savedData.CurrentDay;
        SetDayButton(_currentDay);
        _resources = savedData.Resources;
        _currentTech = savedData.CurrentTech;
        ReloadTech();
        _cardScoresList = new List<int>(savedData.CardScoresList);
        _cardScores = new List<Func<GameManager, SScoreInfo>>();
        if (_cardScoresList != null)
        {
            for (int i = 0; i < _cardScoresList.Count; i++)
            {
                _cardScores.Add(CCardStatic.FindPointFunction(_cardScoresList[i]));
            }
        }
        Logger.Log($"카드점수 - {savedData.CardScoresList.Count} => {_cardScoresList.Count} => {_cardScores.Count}");
        _cardHand.LoadSavedHand(savedData.CardsOnHand);
        _cardHand.LoadSavedDeck(savedData.CardsOnDeck);
        LoadTilesByInt(savedData.TileInt, savedData.TilePoint);
        UnityEngine.Random.InitState(_randomSeed);
    }
}
