using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Flags]
public enum SaveSlot
{
    None    = 0,     
    Save01  = 1 << 0,
    Save02  = 1 << 1,
    Save03  = 1 << 2,
}

[Serializable]
public class CSaveData
{
    #region Member Variable
    public int _version;
    public int _randomSeed;
    public int _currentDay;
    public List<int> _resources;
    public List<int> _currentTechEnum;
    public List<int> _currentTechLevel;
    public List<int> _cardScoresList;
    public List<int> _cardsOnHand;
    public List<int> _cardsOnDeck;
    public List<int> _tileInt;
    public List<int> _tilePoint;
    public int _scoreTotal;
    #endregion

    #region Property
    public int Version
    {
        get { return _version; }
        private set { _version = value; }
    }

    public int RandomSeed
    {
        get { return _randomSeed; }
        private set { _randomSeed = value; }
    }

    public int CurrentDay
    {
        get { return _currentDay; }
        private set { _currentDay = value; }
    }
    public CResources Resources
    {
        get
        {
            if (_resources == null || _resources.Count < 9) return default;
            return new CResources(
                _resources[0],
                _resources[1],
                _resources[2],
                _resources[3],
                _resources[4],
                _resources[5],
                _resources[6],
                _resources[7],
                _resources[8],
                _resources[9]);
        }
        set
        {
            if (_resources == null) _resources = new List<int>();
            _resources.Clear();
            _resources.Add(value.moneyCurrent);
            _resources.Add(value.moneyIncrease);
            _resources.Add(value.materialsCurrent);
            _resources.Add(value.materialsIncrease);
            _resources.Add(value.menpowerCurrent);
            _resources.Add(value.menpowerIncrease);
            _resources.Add(value.menpowerRemain);
            _resources.Add(value.festivalSuccess);
            _resources.Add(value.festivalInterest);
            _resources.Add(value.festivalRoad);
        }
    }
    public Dictionary<ETech, int> CurrentTech
    {
        get
        {
            Dictionary<ETech, int> tempDict = new Dictionary<ETech, int>();
            if (_currentTechEnum == null || _currentTechLevel == null) return tempDict;
            if (_currentTechEnum.Count != _currentTechLevel.Count)
            {
                Logger.Error("저장 데이터 오류 : CurrentTech");
            }
            int count = Mathf.Min(_currentTechEnum.Count, _currentTechLevel.Count);
            for (int i = 0; i < count; i++)
            {
                tempDict[(ETech)_currentTechEnum[i]] = _currentTechLevel[i];
            }
            return tempDict;
        }
        set
        {
            if (_currentTechEnum == null) _currentTechEnum = new List<int>();
            if (_currentTechLevel == null) _currentTechLevel = new List<int>();
            _currentTechEnum.Clear();
            _currentTechLevel.Clear();
            if (value == null) return;
            foreach (KeyValuePair<ETech, int> dictData in value)
            {
                _currentTechEnum.Add((int)dictData.Key);
                _currentTechLevel.Add(dictData.Value);
            }
        }
    }
    public List<int> CardScoresList
    {
        get { return _cardScoresList; }
        private set { _cardScoresList = value; }
    }
    public List<int> CardsOnHand
    {
        get { return _cardsOnHand; }
        private set { _cardsOnHand = value; }
    }
    public List<int> CardsOnDeck
    {
        get { return _cardsOnDeck; }
        private set { _cardsOnDeck = value; }
    }
    public List<int> TileInt
    {
        get { return _tileInt; }
        private set { _tileInt = value; }
    }
    public List<int> TilePoint
    {
        get { return _tilePoint; }
        private set { _tilePoint = value; }
    }

    public int ScoreTotal
    {
        get { return _scoreTotal; }
        private set { _scoreTotal = value; }
    }
    #endregion

    public CSaveData
        (
            int version,
            int randomSeed,
            int currentDay,
            CResources resources,
            Dictionary<ETech, int> currentTech,
            List<int> cardScoresList,
            List<int> cardsOnHand,
            List<int> cardsOnDeck,
            List<int> tileInt,
            List<int> tilePoint,
            int scoreTotal
        )
    {
        _version = version;
        _randomSeed = randomSeed;
        _currentDay = currentDay;
        this.Resources = resources;
        this.CurrentTech = currentTech;
        _cardScoresList = new List<int>(cardScoresList);
        Logger.Log($"카드점수 저장 - {_cardScoresList.Count}");
        _cardsOnHand = new List<int>(cardsOnHand);
        _cardsOnDeck = new List<int>(cardsOnDeck);
        _tileInt = new List<int>(tileInt);
        _tilePoint = new List<int>(tilePoint);
        _scoreTotal = scoreTotal;
    }
}

public static class SaveManager
{
    private static int _maxSaveSlot = System.Enum.GetValues(typeof(SaveSlot)).Length - 1;
    private static CSaveData[] _saveData = new CSaveData[_maxSaveSlot];
    private static SaveSlot _saveFlag = SaveSlot.None;
    private readonly static bool _isPersist = false;

    public static int MaxSaveSlot
    {
        get { return _maxSaveSlot; }
        private set { _maxSaveSlot = value; }
    }

    public static SaveSlot SaveFlag
    {
        get { return _saveFlag; }
        private set { _saveFlag = value; }
    }

    private static string GetSavepath(int slot)
    {
        if (slot >= _maxSaveSlot)
        {
            Logger.Error("갯수를 초과한 슬롯번호 호출");
            return null;
        }
        if (_isPersist) return Path.Combine(Application.persistentDataPath, $"save{slot:D2}.json");
        else return Path.Combine(Application.dataPath, $"save{slot:D2}.json");
    }
    public static void RefreshAllData()
    {
        FindAvailableData();
        for (int i = 0; i < _maxSaveSlot; i++)
        {
            if ((_saveFlag & (SaveSlot)(1 << i)) != SaveSlot.None)
            {
                _saveData[i] = LoadData(i);
            }
            else
            {
                _saveData[i] = null;
            }
        }
        Logger.Log($"_saveFlag = {_saveFlag}");
    }
    public static void RefreshData(int index)
    {
        CSaveData tempData = LoadData(index);
        if (tempData == null)
        {
            Logger.Error($"저장데이터 새로고침 : {index} 데이터 없음");
            _saveFlag &= ~(SaveSlot)(1 << index);
        }
        else
        {
            Logger.Log($"저장데이터 불러옴 : {index}");
            _saveData[index] = tempData;
            _saveFlag |= (SaveSlot)(1 << index);
        }
    }
    public static SaveSlot FindAvailableData()
    {
        for(int i = 0; i < _maxSaveSlot; i++)
        {
            string path = GetSavepath(i);
            if (File.Exists(path))
            {
                _saveFlag |= (SaveSlot)(1 << i);
                _saveData[i] = LoadData(i);
            }
            else
            {
                _saveFlag &= ~(SaveSlot)(1 << i);
                _saveData[i] = null;
            }
        }
        return _saveFlag;
    }
    public static void SaveData(int index, CSaveData data)
    {
        string path = GetSavepath(index);
        _saveData[index] = data;
        File.WriteAllText(path, JsonUtility.ToJson(data));
        _saveFlag |= (SaveSlot)(1 << index);
        Logger.Success($"저장 성공 : {index} 데이터 저장함");
    }
    public static CSaveData LoadData(int index)
    {
        if (_saveData[index] == null)
        {
            Logger.Log($"불러오기 시도 : {index} 데이터가 없어, 로컬에서 시도함.");
            string path = GetSavepath(index);
            if (File.Exists(path))
            {
                CSaveData loadedData = JsonUtility.FromJson<CSaveData>
                    (
                        File.ReadAllText(path)
                    );
                if (loadedData != null)
                {
                    _saveData[index] = loadedData;
                }
                else
                {
                    Logger.Error($"불러오기 오류 : {index} 데이터가 잘못됨.");
                    return null;
                }
            }
        }
        Logger.Success($"불러오기 성공 : {index} 데이터 불러옴");
        return _saveData[index];
    }
    public static bool DeleteData(int index)
    {
        string path = GetSavepath(index);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public static bool Available(int index)
    {
        SaveSlot targetFlag = (SaveSlot)(1 << index);
        if ((_saveFlag & targetFlag) != SaveSlot.None) return true;
        else return false;
    }
}
