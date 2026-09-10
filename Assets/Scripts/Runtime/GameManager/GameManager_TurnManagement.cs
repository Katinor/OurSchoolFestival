using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class GameManager
{
    private void CallNextDay()
    {
        if (_gameState == EGameState.NoInput || _gameState == EGameState.NextDay || _gameState == EGameState.DailyEvent)
        {
            return;
        }
        _gameState = EGameState.NextDay;
        StartCoroutine(CallNextDayCoroutine());
    }
    private IEnumerator CallNextDayCoroutine()
    {
        if (_currentDay == 15) _soundManager.PlayBGM(EBackgroundSound.Result);
        yield return StartCoroutine(_DayManager.LoadingScreenOn());
        yield return StartCoroutine(_DayManager.StartDayResult(this, _soundManager));
        if (_currentDay >= 15)
        {
            CallGotoTitleResult();
            _currentDay = 16;
            SaveData();
            yield break;
        }
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
        
        _currentDay++;
        SetDayButton(_currentDay);
        if(_currentDay == 6) _soundManager.PlayBGM(EBackgroundSound.Part2);
        if(_currentDay == 11) _soundManager.PlayBGM(EBackgroundSound.Part3);
        yield return StartCoroutine(_DayManager.LoadingScreenOff());
        ClearUndo();
        StartCoroutine(StartDailyEvent());
        SaveData();
        _gameState = EGameState.Idle;
    }

    public int GetFestivalScore()
    {
        return Clamp(_resources.festivalSuccess, 0, SuccessMax) + Clamp(_resources.festivalInterest, 0, InterestMax) + Clamp(_resources.festivalRoad, 0, RoadMax);
    }

    public string GetFestivalDesc()
    {
        return $"완성도 {Clamp(_resources.festivalSuccess, 0, SuccessMax)} / 관심도 {Clamp(_resources.festivalInterest, 0, InterestMax)} / 안정도 {Clamp(_resources.festivalRoad, 0, RoadMax)}";
    }

    public EGameAchievement CalculateScore()
    {
        _scoreSet.tileScore = CalcTileScore();
        _scoreSet.cardScore = CalcCardScore();
        EGameAchievement tempAchievement = CheckAchievement();
        _scoreSet.achievementScore = CalcAchievementScore();
        _scoreTotal = (int) (( GetFestivalScore() + _scoreSet.tileScore.Score + _scoreSet.cardScore.Score ) * (1 + (_scoreSet.achievementScore.Score * 0.1)));
        return tempAchievement;
    }

    public EGameAchievement CheckAchievement()
    {
        int _maxAchevement = System.Enum.GetValues(typeof(EGameAchievement)).Length - 1;
        Logger.Log($"최대 과제 갯수 : {_maxAchevement}");
        EGameAchievement tempAchievement = EGameAchievement.None;
        List<CTile> tempList = null;
        int count;
        for (int i = 0; i < _maxAchevement; i++)
        {
            EGameAchievement targetSwitch = (EGameAchievement)(1 << i);
            if ((_achievement & targetSwitch) != EGameAchievement.None) continue;
            switch (targetSwitch)
            {
                case EGameAchievement.GameClear:
                    if (    
                            _resources.festivalSuccess >= SuccessMax &&
                            _resources.festivalInterest >= InterestMax &&
                            _resources.festivalRoad >= RoadMax
                       )
                    {
                        _achievement |= EGameAchievement.GameClear;
                        tempAchievement |= EGameAchievement.GameClear;
                    }
                    break;
                case EGameAchievement.FoodMaster:
                    if (tempList == null) tempList = GetAllTiles();
                    count = 0;
                    for(int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].TileInCatalog == ETileCatalog.Foodbooth) count++;
                    }
                    if ( count >= 3 )
                    {
                        _achievement |= EGameAchievement.FoodMaster;
                        tempAchievement |= EGameAchievement.FoodMaster;
                    }
                    break;
                case EGameAchievement.NatureMaster:
                    if (tempList == null) tempList = GetAllTiles();
                    count = 0;
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].TileInCatalog == ETileCatalog.Trees) count++;
                    }
                    if (count >= 3)
                    {
                        _achievement |= EGameAchievement.NatureMaster;
                        tempAchievement |= EGameAchievement.NatureMaster;
                    }
                    break;
                case EGameAchievement.BrainMaster:
                    {
                        if (_cardHand.GetHandSize() >= 15)
                        {
                            _achievement |= EGameAchievement.BrainMaster;
                            tempAchievement |= EGameAchievement.BrainMaster;
                        }
                    }
                    break;
                case EGameAchievement.ScienceMaster:
                    if (_currentTech.ContainsKey(ETech.Science) && _currentTech[ETech.Science] >= 5)
                    {
                        _achievement |= EGameAchievement.ScienceMaster;
                        tempAchievement |= EGameAchievement.ScienceMaster;
                    }
                    break;
                case EGameAchievement.MusicMaster:
                    if (_currentTech.ContainsKey(ETech.Music) && _currentTech[ETech.Music] >= 5)
                    {
                        _achievement |= EGameAchievement.MusicMaster;
                        tempAchievement |= EGameAchievement.MusicMaster;
                    }
                    break;
                case EGameAchievement.ArtMaster:
                    if (_currentTech.ContainsKey(ETech.Art) && _currentTech[ETech.Art] >= 5)
                    {
                        _achievement |= EGameAchievement.ArtMaster;
                        tempAchievement |= EGameAchievement.ArtMaster;
                    }
                    break;
                case EGameAchievement.SportsMaster:
                    if (_currentTech.ContainsKey(ETech.Sports) && _currentTech[ETech.Sports] >= 5)
                    {
                        _achievement |= EGameAchievement.SportsMaster;
                        tempAchievement |= EGameAchievement.SportsMaster;
                    }
                    break;
            }
        }

        // 게임 클리어에 관해서는 만족 못한 경우 다시 취소되도록
        if (  !(
                _resources.festivalSuccess >= SuccessMax &&
                _resources.festivalInterest >= InterestMax &&
                _resources.festivalRoad >= RoadMax
               ) )
        {
            _achievement &= ~EGameAchievement.GameClear;
            tempAchievement &= ~EGameAchievement.GameClear;
        }
        return tempAchievement;
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
        string tempString = string.Empty;
        int _maxAchevement = System.Enum.GetValues(typeof(EGameAchievement)).Length - 1;
        int count = 0;
        for (int i = 0; i < _maxAchevement; i++)
        {
            EGameAchievement targetSwitch = (EGameAchievement)(1 << i);
            if ((_achievement & targetSwitch) != EGameAchievement.None)
            {
                if (!string.IsNullOrEmpty(tempString)) tempString += ", ";
                switch (targetSwitch)
                {
                    case EGameAchievement.GameClear:
                        count += 5;
                        tempString += "축제성공(5)";
                        break;
                    case EGameAchievement.FoodMaster:
                        count += 1;
                        tempString += "먹거리장인(1)";
                        break;
                    case EGameAchievement.NatureMaster:
                        count += 1;
                        tempString += "환경보호가(1)";
                        break;
                    case EGameAchievement.BrainMaster:
                        count += 1;
                        tempString += "아이디어뱅크(1)";
                        break;
                    case EGameAchievement.ScienceMaster:
                        count += 1;
                        tempString += "과학왕(1)";
                        break;
                    case EGameAchievement.MusicMaster:
                        count += 1;
                        tempString += "음악왕(1)";
                        break;
                    case EGameAchievement.ArtMaster:
                        count += 1;
                        tempString += "미술왕(1)";
                        break;
                    case EGameAchievement.SportsMaster:
                        count += 1;
                        tempString += "체육왕(1)";
                        break;
                }
            }
        }
        tempString += $"\n총점에 배율로 적용 : x{1f + (count / 10f):F1}";
        return new SScoreInfo(count, tempString);
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
        (List<int> tileIdList, List<int> tilePointList) = GetAllTilesForSave();
        SaveManager.SaveData(_saveSlot, new CSaveData
        (
            _version,
            _randomSeed,
            _currentDay,
            _achievement,
            _resources,
            _currentTech,
            _cardScoresList,
            _cardHand.GetAllHandByInt(),
            _cardHand.GetCardDeckByInt(),
            _cardHand.GetPinoDeckByInt(),
            tileIdList,
            tilePointList,
            _scoreTotal
            ));
    }

    private void LoadData()
    {
        LoadDataCore();
    }

    private void LoadDataCore()
    {
        CSaveData savedData = SaveManager.LoadData(_saveSlot);
        if (savedData == null)
        {
            Logger.Error("세이브 데이터를 불러오지 못했습니다.");
            return;
        }
        if (_version != savedData.Version)
        {
            Logger.Error("버전이 틀립니다.");
        }
        _randomSeed = savedData.RandomSeed;
        _currentDay = savedData.CurrentDay;
        SetDayButton(_currentDay);
        _achievement = savedData.Achievement;
        _resources = savedData.Resources;
        _currentTech = savedData.CurrentTech;
        ReloadTech();
        if (savedData.CardScoresList == null)
        {
            _cardScoresList = new List<int>();
        }
        else
        {
            _cardScoresList = new List<int>(savedData.CardScoresList);
        }
        _cardScores = new List<Func<GameManager, SScoreInfo>>();
        for (int i = 0; i < _cardScoresList.Count; i++)
        {
            _cardScores.Add(CCardStatic.FindPointFunction(_cardScoresList[i]));
        }
        _cardHand.LoadSavedHand(savedData.CardsOnHand);
        _cardHand.LoadSavedDeck(savedData.CardsOnDeck, savedData.CardsPinoDeck);
        LoadTilesFromSave(savedData.TileInt, savedData.TilePoint);
        UnityEngine.Random.InitState(_randomSeed);
    }
}