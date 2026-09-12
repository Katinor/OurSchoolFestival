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
    public int _achievement;
    public List<int> _resources;
    public List<int> _currentTechEnum;
    public List<int> _currentTechLevel;
    public List<int> _cardScoresList;
    public List<int> _cardsOnHand;
    public List<int> _cardsOnDeck;
    public List<int> _cardsPinoDeck;
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
    public EGameAchievement Achievement
    {
        get { return (EGameAchievement) _achievement; }
        private set { _achievement = (int) value; }
    }
    public CResources Resources
    {
        get
        {
            if (_resources == null || _resources.Count < 10) return default;
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
    public List<int> CardsPinoDeck
    {
        get { return _cardsPinoDeck; }
        private set { _cardsPinoDeck = value; }
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
            EGameAchievement achievement,
            CResources resources,
            Dictionary<ETech, int> currentTech,
            List<int> cardScoresList,
            List<int> cardsOnHand,
            List<int> cardsOnDeck,
            List<int> cardsPinoDeck,
            List<int> tileInt,
            List<int> tilePoint,
            int scoreTotal
        )
    {
        _version = version;
        _randomSeed = randomSeed;
        _currentDay = currentDay;
        this.Achievement = achievement;
        this.Resources = resources;
        this.CurrentTech = currentTech;
        _cardScoresList = new List<int>(cardScoresList);
        _cardsOnHand = new List<int>(cardsOnHand);
        _cardsOnDeck = new List<int>(cardsOnDeck);
        _cardsPinoDeck = new List<int>(cardsPinoDeck);
        _tileInt = new List<int>(tileInt);
        _tilePoint = new List<int>(tilePoint);
        _scoreTotal = scoreTotal;
    }
}

public static class SaveManager
{
    private static int _maxSaveSlot = System.Enum.GetValues(typeof(SaveSlot)).Length - 1;
    private static CSaveData[] _saveData = new CSaveData[_maxSaveSlot];
    private static int[] _saveDataErrorCode = new int[_maxSaveSlot];
    private static SaveSlot _saveFlag = SaveSlot.None;
    private readonly static bool _isPersist = true;
    public readonly static int Version = 1;

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
    public static int[] SaveErrorCode
    {
        get { return _saveDataErrorCode; }
        private set { _saveDataErrorCode = value; }
    }

    private static string GetSavepath(int slot)
    {
        if (slot >= _maxSaveSlot || slot < 0)
        {
            Logger.Error($"{slot} : 잘못된 슬롯 번호");
            return null;
        }
        if (_isPersist) return Path.Combine(Application.persistentDataPath, $"save{slot:D2}.json");
        else return Path.Combine(Application.dataPath, $"save{slot:D2}.json");
    }
    public static void RefreshAllData()
    {
        FindAvailableData();
        Logger.Log($"_saveFlag = {_saveFlag}");
    }

    public static void RefreshData(int index)
    {
        if (index < 0 || index >= _maxSaveSlot)
        {
            Logger.Error($"{index} : 잘못된 슬롯 번호");
            return;
        }

        // 파일을 다시 읽어야해서 캐시 지워두기
        _saveData[index] = null;
        _saveFlag &= ~(SaveSlot)(1 << index);

        
        if (LoadData(index))
        {
            Logger.Success($"{index} : 저장데이터 불러옴");
            _saveFlag |= (SaveSlot)(1 << index);
            return;
        }
        if (_saveDataErrorCode[index] == -10 || _saveDataErrorCode[index] == -11)
        {
            Logger.Error($"{index} : 저장데이터 새로고침 - 데이터 없음");
            _saveFlag &= ~(SaveSlot)(1 << index);
            return;
        }
        else
        {
            Logger.Error($"{index} : 저장데이터 새로고침 - 데이터 오류있지만, UI에서 처리하도록.");
            _saveFlag |= (SaveSlot)(1 << index);
            return;
        }
    }
    public static SaveSlot FindAvailableData()
    {
        for (int i = 0; i < _maxSaveSlot; i++)
        {
            RefreshData(i);
        }
        return _saveFlag;
    }
    public static bool SaveData(int index, CSaveData data)
    {
        if (index < 0 || index >= _maxSaveSlot)
        {
            Logger.Error($"{index} : 잘못된 슬롯 번호");
            return false;
        }

        if(CheckSaveData(data) != 0)
        {
            Logger.Error($"{index} : 저장데이터 검증 실패");
            return false;
        }

        try
        {
            string path = GetSavepath(index);
            // 임시 파일 만들기
            string tempPath = path + ".tmp";
            string backupPath = path + ".bak";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(tempPath, JsonUtility.ToJson(data));

            if (File.Exists(path))
            {
                // 백업
                File.Replace(tempPath, path, backupPath);
            }
            else
            {
                // 백업 없음
                File.Move(tempPath, path);
            }

            _saveData[index] = data;
            _saveDataErrorCode[index] = 0;
            _saveFlag |= (SaveSlot)(1 << index);

            Logger.Success($"{index} : 저장 성공, 데이터 저장함");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"저장 실패: {ex.Message}");
            return false;
        }
    }

    public static bool LoadData(int index)
    {
        if (index < 0 || index >= _maxSaveSlot)
        {
            Logger.Error($"{index} : 잘못된 슬롯 번호");
            return false;
        }
        _saveDataErrorCode[index] = -1;
        if (_saveData[index] == null)
        {
            Logger.Log($"{index} : 불러오기 시도 - 데이터가 없어, 로컬에서 시도함.");
            string path = GetSavepath(index);

            try
            {
                CSaveData loadedData = JsonUtility.FromJson<CSaveData>
                    (
                        File.ReadAllText(path)
                    );

                _saveDataErrorCode[index] = CheckSaveData(loadedData);
                if (_saveDataErrorCode[index] != 0)
                {
                    _saveData[index] = null;
                    return false;
                }
                else
                {
                    _saveData[index] = loadedData;
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                _saveDataErrorCode[index] = -10;
                _saveData[index] = null;
                // 파일 없음 (보통 위에서 처리되지만 포함함)
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                _saveDataErrorCode[index] = -11;
                _saveData[index] = null;
                // 저장 폴더 에러
                return false;
            }
            catch (IOException ex)
            {
                _saveDataErrorCode[index] = -12;
                _saveData[index] = null;
                Logger.Error($"{index} : 파일 읽기 실패 - {ex.Message}");
                // 파일 읽기 실패
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                _saveDataErrorCode[index] = -13;
                _saveData[index] = null;
                Logger.Error($"{index} : 파일 접근 실패 - {ex.Message}");
                // 파일 접근 실패
                return false;
            }
            catch (ArgumentException ex)
            {
                _saveDataErrorCode[index] = -14;
                _saveData[index] = null;
                Logger.Error($"{index} : 파일 형식 오류 - {ex.Message}");
                // 파일 형식 오류
                return false;
            }
        }
        else
        {
            _saveDataErrorCode[index] = 0;
            return true;
        }
    }

    public static int CheckSaveData(CSaveData loadedData)
    {
        if (loadedData == null)
        {
            // 저장 데이터 비어있음
            return 1;
            
        }
        if (loadedData.Version != Version)
        {
            // 저장 데이터 버전 다름
            return 2;
        }
        if (loadedData._resources == null || loadedData._resources.Count != 10)
        {
            // 자원 데이터 오류
            return 3;
        }

        if (loadedData._currentTechEnum == null || loadedData._currentTechLevel == null ||
            loadedData._currentTechEnum.Count != loadedData._currentTechLevel.Count)
        {
            // 기술 데이터 오류
            return 4;
        }

        if (loadedData.CardsOnHand == null || loadedData.CardsOnDeck == null || loadedData.CardsPinoDeck == null)
        {
            // 카드 데이터 오류
            return 5;
        }

        if (loadedData.TileInt == null || loadedData.TilePoint == null ||
            loadedData.TileInt.Count == 0 || loadedData.TileInt.Count != loadedData.TilePoint.Count)
        {
            // 타일 데이터 오류
            return 6;
        }
        if (loadedData.CurrentDay < 1 || loadedData.CurrentDay > 16)
        {
            // 진행 일수 오류
            return 7;
        }
        return 0;
    }

    public static bool DeleteData(int index)
    {
        if (index < 0 || index >= _maxSaveSlot)
        {
            Logger.Error($"{index} : 잘못된 슬롯 번호");
            return false;
        }

        try
        {
            string path = GetSavepath(index);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            _saveData[index] = null;
            _saveDataErrorCode[index] = -10;
            _saveFlag &= ~(SaveSlot)(1 << index);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"{index} : 삭제 오류 - {ex.Message}");
            return false;
        }
        
    }

    public static bool Available(int index)
    {
        if (index < 0 || index >= _maxSaveSlot)
        {
            Logger.Error($"{index} : 잘못된 슬롯 번호");
            return false;
        }
        SaveSlot targetFlag = (SaveSlot)(1 << index);
        if ((_saveFlag & targetFlag) != SaveSlot.None) return true;
        else return false;
    }

    public static bool GetData(int index, out CSaveData data)
    {
        data = null;

        if (!LoadData(index))
        {
            return false;
        }
            
        data = _saveData[index];
        return true;
    }
}
