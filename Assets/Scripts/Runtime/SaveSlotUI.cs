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
                () => { _input = -1; });
        }
        if (_loadButton != null)
        {
            _loadButton.onClick.AddListener(
                () => { _input = 1; });
        }
    }


    public void LoadSavedata(CSaveData data)
    {
        if (data.CurrentDay >= 16) _dayDesc.text = $"게임 종료";
        else _dayDesc.text = $"{data.CurrentDay}일차";

        CResources tempResources = data.Resources;

        _slotDesc.text =
            $"<sprite=0> {tempResources.moneyCurrent}<color=yellow>+{tempResources.moneyIncrease}</color> " +
            $"<sprite=2> {tempResources.materialsCurrent}<color=yellow>+{tempResources.materialsIncrease}</color> " +
            $"<sprite=4> {tempResources.menpowerCurrent}<color=yellow>+{tempResources.menpowerIncrease}</color>" + "\n" +
            $"<sprite=6> {tempResources.festivalSuccess} / 14 " +
            $"<sprite=7> {tempResources.festivalInterest} / 19 " +
            $"<sprite=8> {tempResources.festivalRoad} / 8 " +
            $"<sprite=9> {data.ScoreTotal}";
        _deleteButton.interactable = true;
    }

    public void ResetSavedata()
    {
        _dayDesc.text = "-";
        _slotDesc.text = "-데이터 없음-";
        _deleteButton.interactable = false;
    }
}
