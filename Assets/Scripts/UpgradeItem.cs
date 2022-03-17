using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField]
    private string _detail;

    [SerializeField]
    private TMP_Text _detailsText, _levelsText, _costText;

    [SerializeField]
    private GameObject _BG;

    private int _level, _cost, _score;

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.UPDATE_ITEM, UpdateItem);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.UPDATE_ITEM, UpdateItem);
    }

    private void UpdateItem(Dictionary<string,object> message)
    {
        _detailsText.text = _detail;
        _level = PlayerPrefs.HasKey(_detail) ? PlayerPrefs.GetInt(_detail) : 1;
        UpgradeData current = GameManager.instance.GetUpgradeData(_detail, _level);
        _score = PlayerPrefs.HasKey(Constants.Data.SCORE) ? PlayerPrefs.GetInt(Constants.Data.SCORE) : 0;
        _cost = current.cost;
        _levelsText.text = "LEVEL " + _level.ToString();
        if (_cost < 0)
        {
            _BG.SetActive(false);
            _costText.text = "MAXED";
            return;
        }
        _costText.text = (_cost / 1000).ToString() + "K P";
        _BG.SetActive(_score < _cost);
    }

    public void BuyUpgradeItem()
    {
        if (_cost < 0 || _score < _cost) return;
        _score -= _cost;
        PlayerPrefs.SetInt(Constants.Data.SCORE, _score);
        PlayerPrefs.SetInt(_detail, _level + 1);
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_ITEM, null);
    }
}
