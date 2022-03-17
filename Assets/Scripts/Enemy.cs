using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _moveForce;
    private float _torque;

    private float _moveStartForce;
    private float _startTorque;

    [SerializeField]
    private Transform _directionObject,_playerObject;

    [SerializeField]
    private AudioClip _bombClip;

    [SerializeField]
    private GameObject _explosionPrefab;

    private float _input;
    private Rigidbody2D _rb;
    private bool _haGameFinished;
    private SpawnManager spawnManager;
    public bool _canDestroy;

    private void Awake()
    {       
        _rb = GetComponent<Rigidbody2D>();
        _haGameFinished = false;
        spawnManager = FindObjectOfType<SpawnManager>();
        _canDestroy = false;
    }

    private void Start()
    {
        _moveStartForce = GameManager.instance.PlayerStartSpeed;
        _moveForce = _moveStartForce;
        _startTorque = _moveStartForce * 1200 / 24f;
        _torque = _startTorque;        
    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.GAME_OVER, GameOver);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.GAME_OVER, GameOver);
    }

    private void GameOver(Dictionary<string,object> message)
    {
        _haGameFinished = true;
    }

    private void Update()
    {
        if (_haGameFinished) return;
        if(_canDestroy)
        {
            Destroy(gameObject, 1f);
            gameObject.SetActive(false);
        }
        Vector3 tempDirection = _playerObject.position - transform.position;
        Vector3 moveDirection = _directionObject.position - transform.position;
        _input = Vector3.Cross(tempDirection.normalized, moveDirection).z;
    }

    private void FixedUpdate()
    {
        _rb.angularVelocity = _rb.angularVelocity * 0.95f;
        if (_haGameFinished) return;
        _rb.velocity = _rb.velocity * 0.99f;
        Vector3 moveDirection = _directionObject.position - transform.position;
        _rb.AddForce(moveDirection * _moveForce);
        if(Mathf.Abs(_input) > 0.05f)
        {
            _rb.AddTorque(-_input * Time.fixedDeltaTime * _torque * 1.3f);
        }
    }

    public void Init()
    {
        _playerObject = GameManager.instance._player.transform;
        int randomIndex = UnityEngine.Random.Range(0, 4);
        Vector3 tempOffsetDirection;
        switch(randomIndex)
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
        transform.position = _playerObject.position + tempOffsetDirection * 18f;
        transform.Rotate(new Vector3(0, 0, randomIndex * 90f + 180));
        _moveForce *= Random.Range(0.8f, 1.2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_haGameFinished || _canDestroy) return;

        if (collision.gameObject.CompareTag(Constants.Tags.ENEMY))
        {
            if(spawnManager._tempEnemies.Contains(collision.gameObject))
            {
                spawnManager._tempEnemies.Remove(collision.gameObject);
                return;
            }

            EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object>()
            {
                {Constants.ScoreMessage.TYPE,Constants.ScoreMessage.NORMAL_COLLISION },
                {Constants.ScoreMessage.POSITION, transform.position }
            });

            EventManager.TriggerEvent(Constants.EventNames.SPAWN_COIN, new Dictionary<string, object>()
            {
                {Constants.ScoreMessage.POSITION, transform.position }
            });

            spawnManager._tempEnemies.Add(gameObject);
            var explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
            _canDestroy = true;
            collision.gameObject.GetComponent<Enemy>()._canDestroy = true;
            SoundManager.instance.PlaySound(_bombClip);
        }
    }


    public void OnBombed()
    {
        if (_haGameFinished) return;
        
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object>()
        {
            { Constants.ScoreMessage.TYPE, Constants.ScoreMessage.NORMAL_COLLISION},
            { Constants.ScoreMessage.POSITION, transform.position }
        });

        EventManager.TriggerEvent(Constants.EventNames.SPAWN_COIN, new Dictionary<string, object>()
            {
                {Constants.ScoreMessage.POSITION, transform.position }
            });

        var explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);

        Destroy(gameObject);
    }

}
