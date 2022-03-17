using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField]
    private List<StoreData> _data;

    [SerializeField]
    private Image _currentImage;

    [SerializeField]
    TMPro.TMP_Text _coinsText;

    [SerializeField]
    private Sprite _trailSprite;

    private string DEFAULT_CAR = Constants.Store.CAR_1;
    private string DEFAULT_TRAIL = Constants.Store.TRAIL_1;
    private Dictionary<string, Sprite> _cars;
    private Dictionary<string, Color> _colors;

    [HideInInspector] public string CURRENT_CAR;
    [HideInInspector] public string CURRENT_TRAIL;
    [HideInInspector] public string _CURRENTLY_SELECTED;
    [HideInInspector] public string _CURRENTLY_SELECTED_TYPE;

    [HideInInspector] public static StoreManager _instance;

    private void Awake()
    {
        _instance = this;
        _cars = new Dictionary<string, Sprite>();
        for (int i = 0; i < _data.Count; i++)
        {
            _cars[_data[i].name] = _data[i].image;
        }
        _colors = new Dictionary<string, Color>()
        {
            { Constants.Store.TRAIL_1, Color.red },
            { Constants.Store.TRAIL_2, Color.black },
            { Constants.Store.TRAIL_3, Color.blue },
            { Constants.Store.TRAIL_4, Color.green },
            { Constants.Store.TRAIL_5, Color.yellow },
            { Constants.Store.TRAIL_6, Color.white },
        };

        if(PlayerPrefs.HasKey(Constants.Store.CURRENT_CAR))
        {
            CURRENT_CAR = PlayerPrefs.GetString(Constants.Store.CURRENT_CAR);
        }
        else
        {
            CURRENT_CAR = DEFAULT_CAR;
            PlayerPrefs.SetString(Constants.Store.CURRENT_CAR, CURRENT_CAR);
            PlayerPrefs.SetInt(CURRENT_CAR, 1);
        }


        if (PlayerPrefs.HasKey(Constants.Store.CURRENT_TRAIL))
        {
            CURRENT_TRAIL = PlayerPrefs.GetString(Constants.Store.CURRENT_TRAIL);
        }
        else
        {
            CURRENT_TRAIL= DEFAULT_TRAIL;
            PlayerPrefs.SetString(Constants.Store.CURRENT_TRAIL, CURRENT_TRAIL);
            PlayerPrefs.SetInt(CURRENT_TRAIL, 1);
        }

        _CURRENTLY_SELECTED_TYPE = Constants.Data.CAR;
        _CURRENTLY_SELECTED = CURRENT_CAR;
    }

    private void Start()
    {
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_STORE_ITEM, null);
    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.UPDATE_STORE_ITEM, UpdateStoreCoin);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.UPDATE_STORE_ITEM, UpdateStoreCoin);
    }

    private void UpdateStoreCoin(Dictionary<string,object> message)
    {
        int coins = PlayerPrefs.HasKey(Constants.Data.COIN) ? PlayerPrefs.GetInt(Constants.Data.COIN) : 0;
        _coinsText.text = coins.ToString();
    }

    public void SelectItem(string storeItemName, string storeItemType)
    {
        _CURRENTLY_SELECTED = storeItemName;
        _CURRENTLY_SELECTED_TYPE = storeItemType;
        if(_CURRENTLY_SELECTED_TYPE == Constants.Store.CAR)
        {
            _currentImage.sprite = GetPlayerSprite(_CURRENTLY_SELECTED);
            _currentImage.color = Color.white;
        }
        else if (_CURRENTLY_SELECTED_TYPE == Constants.Store.TRAIL)
        {
            _currentImage.sprite = _trailSprite;
            _currentImage.color = GetPlayerColor(_CURRENTLY_SELECTED);
        }
    }

    public void BuyItem()
    {
        int isAlreadyBought = PlayerPrefs.HasKey(_CURRENTLY_SELECTED) ? PlayerPrefs.GetInt(_CURRENTLY_SELECTED) : 0;
        if (isAlreadyBought == 1) return;

        int coins = PlayerPrefs.HasKey(Constants.Data.COIN) ? PlayerPrefs.GetInt(Constants.Data.COIN) : 0;
        if (coins < 200) return;

        coins -= 200;
        PlayerPrefs.SetInt(_CURRENTLY_SELECTED, 1);
        PlayerPrefs.SetInt(Constants.Data.COIN, coins);

        if(_CURRENTLY_SELECTED_TYPE == Constants.Store.CAR)
        {
            CURRENT_CAR = _CURRENTLY_SELECTED;
            PlayerPrefs.SetString(Constants.Store.CURRENT_CAR, CURRENT_CAR);
        }
        else if(_CURRENTLY_SELECTED_TYPE == Constants.Store.TRAIL)
        {
            CURRENT_TRAIL = _CURRENTLY_SELECTED;
        }

        EventManager.TriggerEvent(Constants.EventNames.UPDATE_STORE_ITEM, null);
    }

    public void ActivateItem()
    {
        string isActive = _CURRENTLY_SELECTED_TYPE == Constants.Store.CAR ? CURRENT_CAR : CURRENT_TRAIL;
        if (isActive == _CURRENTLY_SELECTED) return;

        int isAlreadyBought = PlayerPrefs.HasKey(_CURRENTLY_SELECTED) ? PlayerPrefs.GetInt(_CURRENTLY_SELECTED) : 0;
        if (isAlreadyBought == 0) return;

        if (_CURRENTLY_SELECTED_TYPE == Constants.Store.CAR)
        {
            CURRENT_CAR = _CURRENTLY_SELECTED;
            PlayerPrefs.SetString(Constants.Store.CURRENT_CAR, CURRENT_CAR);
        }
        else if (_CURRENTLY_SELECTED_TYPE == Constants.Store.TRAIL)
        {
            CURRENT_TRAIL = _CURRENTLY_SELECTED;
        }

        EventManager.TriggerEvent(Constants.EventNames.UPDATE_STORE_ITEM, null);
    }

    public Sprite GetPlayerSprite(string key)
    {
        Sprite temp;
        if(_cars.TryGetValue(key,out temp))
        {

        }
        return temp;
    }

    public Color GetPlayerColor(string key)
    {
        Color temp;
        if(_colors.TryGetValue(key,out temp))
        {

        }
        return temp;
    }
}

[System.Serializable]
public struct StoreData
{
    public string name;
    public Sprite image;
}
