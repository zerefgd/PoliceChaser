using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _coinPrefab, _enemyPrefab;

    [SerializeField]
    private List<GameObject> _powerUps;

    [HideInInspector]
    public List<GameObject> _tempEnemies = new List<GameObject>()
    {       
    };

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.GAME_START, StartSpawning);
        EventManager.StartListening(Constants.EventNames.GAME_OVER, StoptSpawning);
        EventManager.StartListening(Constants.EventNames.SPAWN_COIN, SpawnCoin);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.GAME_START, StartSpawning);
        EventManager.StopListening(Constants.EventNames.GAME_OVER, StoptSpawning);
        EventManager.StopListening(Constants.EventNames.SPAWN_COIN, SpawnCoin);
    }

    private void StartSpawning(Dictionary<string,object> message)
    {
        InvokeRepeating("SpawnEnemy", 0f, UnityEngine.Random.Range(1.5f, 3.5f));
        InvokeRepeating("SpawnPowerUp", 5f, 10f);
    }

    private void StoptSpawning(Dictionary<string, object> message)
    {
        CancelInvoke("SpawnEnemy");
        CancelInvoke("SpawnPowerUp");
    }

    private void SpawnEnemy()
    {
        var enemy = Instantiate(_enemyPrefab);
        enemy.GetComponent<Enemy>().Init();
    }

    private void SpawnPowerUp()
    {
        var powerUp = _powerUps[UnityEngine.Random.Range(0, _powerUps.Count)];
        int randomIndex = UnityEngine.Random.Range(0, 4);
        Vector3 tempOffsetDirection;
        switch (randomIndex)
        {
            case 0:
                tempOffsetDirection = new Vector3(0, 1, 0);
                break;
            case 1:
                tempOffsetDirection = new Vector3(1, 0, 0);
                break;
            case 2:
                tempOffsetDirection = new Vector3(0, -1, 0);
                break;
            case 3:
                tempOffsetDirection = new Vector3(-1, 0, 0);
                break;
            default:
                tempOffsetDirection = new Vector3(0, 0, 0);
                break;
        }
        tempOffsetDirection *= 10f;
        tempOffsetDirection += GameManager.instance._player.transform.position;
        var current = Instantiate(powerUp, tempOffsetDirection, Quaternion.identity);
        Destroy(current, 10f);
    }

    private void SpawnCoin(Dictionary<string,object> message)
    {
        var tempCoin = Instantiate(_coinPrefab, (Vector3)message[Constants.ScoreMessage.POSITION], Quaternion.identity);
        Destroy(tempCoin, 5f);
    }
}
