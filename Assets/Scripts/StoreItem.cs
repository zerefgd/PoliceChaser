using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private string _storeItemName;
    [SerializeField] private string _storeItemType;
    [SerializeField] private Image _storeItemSprite;
    [SerializeField] private GameObject _lockedImage, _unlockedImage;
    [SerializeField] private Sprite _trailSprite;


    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.UPDATE_STORE_ITEM, UpdateStoreItem);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.UPDATE_STORE_ITEM, UpdateStoreItem);
    }

    private void UpdateStoreItem(Dictionary<string,object> message)
    {
        _lockedImage.SetActive(false);
        _unlockedImage.SetActive(false);
        
        int _storeItemValue;
        if(PlayerPrefs.HasKey(_storeItemName))
        {
            _storeItemValue = PlayerPrefs.GetInt(_storeItemName);
        }
        else
        {
            _storeItemValue = 0;
            PlayerPrefs.SetInt(_storeItemName, _storeItemValue);
        }

        string currentActive = "";
        if(_storeItemType == Constants.Store.CAR)
        {
            Sprite sprite = StoreManager._instance.GetPlayerSprite(_storeItemName);
            _storeItemSprite.sprite = sprite;
            _storeItemSprite.color = Color.white;
            currentActive = StoreManager._instance.CURRENT_CAR;
        }
        else if (_storeItemType == Constants.Store.TRAIL)
        {
            Color color= StoreManager._instance.GetPlayerColor(_storeItemName);
            _storeItemSprite.sprite = _trailSprite;
            _storeItemSprite.color = color;
            currentActive = StoreManager._instance.CURRENT_TRAIL;
        }

        if(_storeItemValue == 0)
        {
            _lockedImage.SetActive(true);
        }
        else
        {
            if(_storeItemName == currentActive)
            {
                _unlockedImage.SetActive(true);
            }
        }       
    }

    public void SelectedItem()
    {
        StoreManager._instance.SelectItem(_storeItemName, _storeItemType);
    }
}
