using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    private GameObject _openHelpPanel, _settingsPanel, _storePanel, _carPrefab;

    [SerializeField]
    private Image _setttingsMusic, _settings_Sound, _settingsShake;
    #endregion

    #region START_METHODS
    private void Awake()
    {
        _openHelpPanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _storePanel.SetActive(false);
        SpawnAndSetCars();
    }

    private void SpawnAndSetCars()
    {
        for (int i = 0; i < 16; i++)
        {
            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        var currentCar = Instantiate(_carPrefab, _carPrefab.transform.position, Quaternion.identity);
        currentCar.GetComponent<MenuCar>().Init();
    }
    #endregion

    #region BUTTON_FUNCTION
    public void OpenFacebook()
    {
        Application.OpenURL("https://www.facebook.com/zerefgd/");
    }

    public void OpenInstagram()
    {
        Application.OpenURL("https://www.instagram.com/zerefgd/");
    }

    public void OpenHelpPanel()
    {
        _openHelpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        _openHelpPanel.SetActive(false);
    }

    public void OpenStorePanel()
    {
        _storePanel.SetActive(true);
    }

    public void CloseStorePanel()
    {
        _storePanel.SetActive(false);
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

    public void OpenSettings()
    {
        _settingsPanel.SetActive(true);
        SetSettingsButton(_setttingsMusic, Constants.Settings.SETTINGS_MUSIC);
        SetSettingsButton(_settings_Sound, Constants.Settings.SETTINGS_SOUND);
        SetSettingsButton(_settingsShake, Constants.Settings.SETTINGS_SHAKE);
    }

    public void ClickSwitchMusic()
    {
        SwitchSettingsButton(_setttingsMusic, Constants.Settings.SETTINGS_MUSIC);
        SoundManager.instance.SetMusic();
    }

    public void ClickSwitchSound()
    {
        SwitchSettingsButton(_settings_Sound, Constants.Settings.SETTINGS_SOUND);
        SoundManager.instance.SetEffect();
    }

    public void ClickSwitchShake()
    {
        SwitchSettingsButton(_settingsShake, Constants.Settings.SETTINGS_SHAKE);        
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
    }

    #endregion

    #region HELPER_FUNCTION
    void SetSettingsButton(Image currentImage,string key)
    {
        int result;
        if(PlayerPrefs.HasKey(key))
        {
            result = PlayerPrefs.GetInt(key);
        }
        else
        {
            result = 1;
        }
        PlayerPrefs.SetInt(key, result);
        currentImage.color = result == 1 ? Color.white : Color.gray;
    }

    void SwitchSettingsButton(Image currentImage, string key)
    {
        int result;
        if (PlayerPrefs.HasKey(key))
        {
            result = PlayerPrefs.GetInt(key);
        }
        else
        {
            result = 1;
        }
        result = result == 1 ? 0 : 1;
        PlayerPrefs.SetInt(key, result);
        currentImage.color = result == 1 ? Color.white : Color.gray;
    }
    #endregion
}
