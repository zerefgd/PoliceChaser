using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText, _endScoreText, _totalScoreText, _scorePrefab, _coinsText;

    [SerializeField]
    private Slider _revengeSlider;

    [SerializeField]
    private List<Image> _boostHolders;

    [SerializeField]
    private List<GameObject> _healthHolders;

    [SerializeField]
    private GameObject _boostHolder;

    private float _scoreMultiplier;
    private float _score;
    private bool _hasGameFinished;

    private void Awake()
    {
        _score = 0;
        _scoreMultiplier = 32;
        _hasGameFinished = false;
        _revengeSlider.gameObject.SetActive(false);
        int tempScore = (int)_score;
        _scoreText.text = tempScore.ToString() + " PTS";
        UpdateBoost(0);
    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.GAME_START, GameStart);
        EventManager.StartListening(Constants.EventNames.GAME_OVER, GameOver);
        EventManager.StartListening(Constants.EventNames.UPDATE_SCORE, UpdateScore);
        EventManager.StartListening(Constants.EventNames.UPDATE_COIN, UpdateCoin);
        EventManager.StartListening(Constants.EventNames.REFRESH_SCORE, RefreshScore);
        EventManager.StartListening(Constants.EventNames.REFRESH_COIN, RefreshCoin);
        EventManager.StartListening(Constants.EventNames.UPDATE_HEALTH, UpdateHealth);
        EventManager.StartListening(Constants.EventNames.UPDATE_ITEM, RefreshScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.GAME_START, GameStart);
        EventManager.StopListening(Constants.EventNames.GAME_OVER, GameOver);
        EventManager.StopListening(Constants.EventNames.UPDATE_SCORE, UpdateScore);
        EventManager.StopListening(Constants.EventNames.UPDATE_COIN, UpdateCoin);
        EventManager.StopListening(Constants.EventNames.REFRESH_SCORE, RefreshScore);
        EventManager.StopListening(Constants.EventNames.REFRESH_COIN, RefreshCoin);
        EventManager.StopListening(Constants.EventNames.UPDATE_HEALTH, UpdateHealth);
        EventManager.StopListening(Constants.EventNames.UPDATE_ITEM, RefreshScore);
    }

    private void GameStart(Dictionary<string,object> message)
    {
        StartCoroutine(StartCounting());
    }

    IEnumerator StartCounting()
    {
        while(!_hasGameFinished)
        {
            _score += (_scoreMultiplier * Time.deltaTime);
            int tempScore = (int)_score;
            _scoreText.text = tempScore.ToString() + " PTS";
            yield return null;
        }
    }

    private void GameOver(Dictionary<string,object> message)
    {
        _hasGameFinished = true;
        int tempScore = (int)_score;
        _endScoreText.text = "SCORE "  + tempScore.ToString() + " PTS";
        int totalScore = PlayerPrefs.HasKey(Constants.Data.SCORE) ? PlayerPrefs.GetInt(Constants.Data.SCORE) : 0;
        totalScore += tempScore;
        PlayerPrefs.SetInt(Constants.Data.SCORE,totalScore);
        _totalScoreText.text = totalScore.ToString() + " PTS";        
    }

    private void UpdateScore(Dictionary<string,object> message)
    {
        var scoreType = message[Constants.ScoreMessage.TYPE].ToString();

        switch(scoreType)
        {

            case Constants.ScoreMessage.NORMAL_COLLISION:
                _score += 40;
                int tempScore = (int)_score;
                _scoreText.text = tempScore.ToString() + " PTS";

                Vector3 spawnPos = (Vector3)message[Constants.ScoreMessage.POSITION];
                var tempScoreText = Instantiate(_scorePrefab,spawnPos,Quaternion.identity);
                tempScoreText.text = "+40";
                Destroy(tempScoreText, 3f);

                break;

            case Constants.ScoreMessage.MISS:
                int increase = (int)message[Constants.ScoreMessage.AMOUNT];
                UpdateBoost(increase);

                spawnPos = (Vector3)message[Constants.ScoreMessage.POSITION] + Vector3.up*2f;
                tempScoreText = Instantiate(_scorePrefab, spawnPos, Quaternion.identity);
                tempScoreText.text = "MISS X " + increase.ToString();
                Destroy(tempScoreText, 3f);

                increase = increase == 1 ? 80 : increase == 2 ? 120 : 200;
                _score += increase;                
                tempScore = (int)_score;
                _scoreText.text = tempScore.ToString() + " PTS";

                spawnPos = (Vector3)message[Constants.ScoreMessage.POSITION];
                tempScoreText = Instantiate(_scorePrefab, spawnPos, Quaternion.identity);
                tempScoreText.text = "+" + increase.ToString();
                Destroy(tempScoreText, 3f);

                break;

            case Constants.ScoreMessage.REVENGE:
                increase = (int)message[Constants.ScoreMessage.AMOUNT];

                if(increase == 0)
                {

                    spawnPos = (Vector3)message[Constants.ScoreMessage.POSITION] + Vector3.up*3f;
                    tempScoreText = Instantiate(_scorePrefab, spawnPos, Quaternion.identity);
                    tempScoreText.text = "REVENGE ";
                    Destroy(tempScoreText, 3f);
                    StartCoroutine(StartRevengeSlider());
                }
                increase = increase == 0 ? 200 : increase * 40;
                _score += increase;
                tempScore = (int)_score;
                _scoreText.text = tempScore.ToString() + " PTS";

                spawnPos = (Vector3)message[Constants.ScoreMessage.POSITION];
                tempScoreText = Instantiate(_scorePrefab, spawnPos, Quaternion.identity);
                tempScoreText.text = "+" + increase.ToString();
                Destroy(tempScoreText, 3f);
                break;

            default:
                break;
        }
    }

    IEnumerator StartRevengeSlider()
    {
        float _revengeTime = GameManager.instance.PlayerRevengeTimer;
        _boostHolder.SetActive(false);
        _revengeSlider.gameObject.SetActive(true);
        while(_revengeTime > 0f)
        {
            _revengeTime -= Time.deltaTime;
            _revengeSlider.value = _revengeTime / 4f;
            yield return null;
        }

        _revengeSlider.gameObject.SetActive(false);
        _boostHolder.SetActive(true);
        UpdateBoost(0);
    }

    private void UpdateBoost(int amount)
    {
        for (int i = 0; i < _boostHolders.Count; i++)
        {
            _boostHolders[i].color = Color.black;
        }

        for (int i = 0; i < amount; i++)
        {
            _boostHolders[i].color = Color.green;
        }
    }

    private void UpdateHealth(Dictionary<string,object> message)
    {
        int amount = (int)message[Constants.ScoreMessage.AMOUNT];
        for (int i = 0; i < _healthHolders.Count; i++)
        {
            _healthHolders[i].SetActive(false);
        }
        for (int i = 0; i < amount; i++)
        {
            _healthHolders[i].SetActive(true);
        }
    }

    private void UpdateCoin(Dictionary<string,object> message)
    {
        int coins = PlayerPrefs.HasKey(Constants.Data.COIN) ? PlayerPrefs.GetInt(Constants.Data.COIN) : 0;
        coins++;
        PlayerPrefs.SetInt(Constants.Data.COIN, coins);
        _coinsText.text = coins.ToString();
    }

    private void RefreshScore(Dictionary<string,object> message)
    {
        int score = PlayerPrefs.HasKey(Constants.Data.SCORE) ? PlayerPrefs.GetInt(Constants.Data.SCORE) : 0;
        _totalScoreText.text = score.ToString() + " PTS";
    }

    private void RefreshCoin(Dictionary<string, object> message)
    {
        int coins = PlayerPrefs.HasKey(Constants.Data.COIN) ? PlayerPrefs.GetInt(Constants.Data.COIN) : 0;
        _coinsText.text = coins.ToString();
    }

}
