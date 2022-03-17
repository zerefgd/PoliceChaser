using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private GameObject _pauseButton, _pausePanel, _gameActiveUI, _startButton, _gameEndUI, _storePanel, _inputPanel;

    public GameObject _player;

    public bool hasGameStarted;

    public readonly List<UpgradeData> _upgradeData = new List<UpgradeData>()
    {
        new UpgradeData() { type = Constants.Data.CAR, level = 1 , cost = 5000, value = 25 },
        new UpgradeData() { type = Constants.Data.CAR, level = 2 , cost = 10000, value = 26 },
        new UpgradeData() { type = Constants.Data.CAR, level = 3 , cost = 15000, value = 27 },
        new UpgradeData() { type = Constants.Data.CAR, level = 4 , cost = 25000, value = 28 },
        new UpgradeData() { type = Constants.Data.CAR, level = 5 , cost = -1, value = 30 },

        new UpgradeData() { type = Constants.Data.BOOST, level = 1 , cost = 5000, value = 1f },
        new UpgradeData() { type = Constants.Data.BOOST, level = 2 , cost = 10000, value = 1.25f },
        new UpgradeData() { type = Constants.Data.BOOST, level = 3 , cost = 15000, value = 1.5f },
        new UpgradeData() { type = Constants.Data.BOOST, level = 4 , cost = 25000, value = 1.75f },
        new UpgradeData() { type = Constants.Data.BOOST, level = 5 , cost = -1, value = 2f },

        new UpgradeData() { type = Constants.Data.REVENGE, level = 1 , cost = 5000, value = 4f },
        new UpgradeData() { type = Constants.Data.REVENGE, level = 2 , cost = 10000, value = 4.25f },
        new UpgradeData() { type = Constants.Data.REVENGE, level = 3 , cost = 15000, value = 4.5f },
        new UpgradeData() { type = Constants.Data.REVENGE, level = 4 , cost = 25000, value = 4.75f },
        new UpgradeData() { type = Constants.Data.REVENGE, level = 5 , cost = -1, value = 5f },

        new UpgradeData() { type = Constants.Data.HEALTH, level = 1 , cost = 5000, value = 1 },
        new UpgradeData() { type = Constants.Data.HEALTH, level = 2 , cost = 10000, value = 2 },
        new UpgradeData() { type = Constants.Data.HEALTH, level = 3 , cost = -1, value = 3 },
    };

    [SerializeField]
    private List<StoreData> _data;

    Sprite GetPlayerSprite(string key)
    {
        Sprite result = _player.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].name == key)
                result = _data[i].image;
        }
        return result;
    }

    public Sprite PlayerSprite
    {
        get
        {
            string currentCar = PlayerPrefs.GetString(Constants.Store.CURRENT_CAR);
            return GetPlayerSprite(currentCar);
        }
    }

    public int PlayerStartSpeed
    {
        get
        {
            int level = 1;
            UpgradeData current;
            if(PlayerPrefs.HasKey(Constants.Data.CAR))
            {
                level = PlayerPrefs.GetInt(Constants.Data.CAR);
            }
            else
            {
                PlayerPrefs.SetInt(Constants.Data.CAR, level);
            }
            current = GetUpgradeData(Constants.Data.CAR, level);
            return (int)current.value;
        }
    }

    public float PlayerBoostTimer
    {
        get
        {
            int level = 1;
            UpgradeData current;
            if (PlayerPrefs.HasKey(Constants.Data.BOOST))
            {
                level = PlayerPrefs.GetInt(Constants.Data.BOOST);
            }
            else
            {
                PlayerPrefs.SetInt(Constants.Data.BOOST, level);
            }
            current = GetUpgradeData(Constants.Data.BOOST, level);
            return current.value;
        }
    }

    public float PlayerRevengeTimer
    {
        get
        {
            int level = 1;
            UpgradeData current;
            if (PlayerPrefs.HasKey(Constants.Data.REVENGE))
            {
                level = PlayerPrefs.GetInt(Constants.Data.REVENGE);
            }
            else
            {
                PlayerPrefs.SetInt(Constants.Data.REVENGE, level);
            }
            current = GetUpgradeData(Constants.Data.REVENGE, level);
            return current.value;
        }
    }

    public int PlayerHealth
    {
        get
        {
            int level = 1;
            UpgradeData current;
            if (PlayerPrefs.HasKey(Constants.Data.HEALTH))
            {
                level = PlayerPrefs.GetInt(Constants.Data.HEALTH);
            }
            else
            {
                PlayerPrefs.SetInt(Constants.Data.HEALTH, level);
            }
            current = GetUpgradeData(Constants.Data.HEALTH, level);
            return (int)current.value;
        }
    }

    public UpgradeData GetUpgradeData(string key,int level)
    {
        UpgradeData result = new UpgradeData();
        for (int i = 0; i < _upgradeData.Count; i++)
        {
            UpgradeData current = _upgradeData[i];
            if(current.type == key && current.level == level)
            {
                result = current;
                break;
            }
        }
        return result;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        hasGameStarted = false;
        _startButton.SetActive(true);
        _gameActiveUI.SetActive(true);
        _gameEndUI.SetActive(false);
        _pausePanel.SetActive(false);
        _inputPanel.SetActive(false);
        _storePanel.SetActive(false);
        EventManager.TriggerEvent(Constants.EventNames.REFRESH_COIN, null);
        SoundManager.instance.AddButtonSound();
    }

    public void StartGame()
    {
        hasGameStarted = true;
        _startButton.SetActive(false);
        _inputPanel.SetActive(true);
        EventManager.TriggerEvent(Constants.EventNames.GAME_START, null);
    }

    public void OpenPausePanel()
    {
        _inputPanel.SetActive(false);
        _pausePanel.SetActive(true);
        _pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ClosePausePanel()
    {
        _inputPanel.SetActive(true);
        _pausePanel.SetActive(false);
        _pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

    public void GameOver()
    {
        _gameActiveUI.SetActive(false);
        _inputPanel.SetActive(false);
        _gameEndUI.SetActive(true);
        EventManager.TriggerEvent(Constants.EventNames.GAME_OVER, null);
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_ITEM, null);
    }

    public void AddCoins()
    {
        PlayerPrefs.SetInt(Constants.Data.COIN, 10000);
        EventManager.TriggerEvent(Constants.EventNames.REFRESH_COIN, null);
    }

    public void AddScore()
    {
        PlayerPrefs.SetInt(Constants.Data.SCORE, 100000);
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_ITEM, null);
    }

    public void OpenStore()
    {
        _storePanel.SetActive(true);
    }

    public void CloseStore()
    {
        _storePanel.SetActive(false);
    }
}

[System.Serializable]
public struct UpgradeData
{
    public string type;
    public int level;
    public int cost;
    public float value;
}

