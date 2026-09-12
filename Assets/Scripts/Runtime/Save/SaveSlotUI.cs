using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] TMP_Text _dayDesc;
    [SerializeField] TMP_Text _slotDesc;
    [SerializeField] Button _deleteButton;
    [SerializeField] Button _loadButton;

    private int _input = 0;

    public int Input
    {
        get { return _input; }
        set { _input = value; }
    }

    void Start()
    {
        if (_deleteButton != null)
        {
            _deleteButton.onClick.AddListener(
                () => _input = -1 );
        }
        if (_loadButton != null)
        {
            _loadButton.onClick.AddListener(
                () => _input = 1 );
        }
    }


    public void LoadSavedata(CSaveData data)
    {
        _input = 0;
        _loadButton.interactable = true;
        if (data.CurrentDay >= 16) _dayDesc.text = $"게임 종료";
        else _dayDesc.text = $"{data.CurrentDay}일차";

        CResources tempResources = data.Resources;

        _slotDesc.text =
            $"<sprite=0> {tempResources.moneyCurrent}<color=yellow>+{tempResources.moneyIncrease}</color> " +
            $"<sprite=2> {tempResources.materialsCurrent}<color=yellow>+{tempResources.materialsIncrease}</color> " +
            $"<sprite=4> {tempResources.menpowerCurrent}<color=yellow>+{tempResources.menpowerIncrease}</color>" + "\n" +
            $"<sprite=6> {Clamp(tempResources.festivalSuccess, 0, 18)} / 18 " +
            $"<sprite=7> {Clamp(tempResources.festivalInterest, 0, 19)} / 19 " +
            $"<sprite=8> {Clamp(tempResources.festivalRoad, 0, 8)} / 8 " +
            $"<sprite=9> {data.ScoreTotal}";
        _deleteButton.interactable = true;
    }

    public void ShowLoadError(int errorCode)
    {
        _dayDesc.text = "오류슬롯";
        string tempText = "저장 데이터 읽기 실패\n";
        switch (errorCode)
        {
            case -12:
                tempText += ": 파일 읽기 실패";
                break;
            case -13:
                tempText += ": 파일 접근 실패";
                break;
            case -14:
                tempText += ": 파일 형식 오류";
                break;
            case 1:
                tempText += ": 빈 파일";
                break;
            case 2:
                tempText += ": 버전 오류";
                break;
            case 3:
                tempText += ": 자원 데이터 오류";
                break;
            case 4:
                tempText += ": 기술 데이터 오류";
                break;
            case 5:
                tempText += ": 카드 데이터 오류";
                break;
            case 6:
                tempText += ": 타일 데이터 오류";
                break;
            case 7:
                tempText += ": 진행 일수 오류";
                break;
            default:
                tempText += $": 알 수 없는 오류 - {errorCode}";
                break;
        }
        _slotDesc.text = tempText;
        _loadButton.interactable = false;
        _deleteButton.interactable = true;
    }

    public void ResetSavedata()
    {
        _input = 0;
        _loadButton.interactable = true;
        _dayDesc.text = "-";
        _slotDesc.text = "-데이터 없음-";
        _deleteButton.interactable = false;
    }
    private int Clamp(int target, int min, int max)
    {
        if (target < min) return min;
        else if (target > max) return max;
        else return target;
    }
}
