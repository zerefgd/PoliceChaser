using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _moveStartForce;
    private float _startTorque;
    private float _startBoostTimer;
    private float _startRevengeTimer;
    private int _startHealth;
    private Sprite _startSprite;

    [SerializeField]
    private Transform _directionObject;

    [SerializeField]
    private GameObject _boostObject, _revengeObject, _shieldObject, _magnetObject, _explosionPrefab;

    [SerializeField]
    private TrailRenderer[] _trails;

    [SerializeField]
    private AudioClip _bombClip;

    private float _input;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerMovement _movement;
    private bool _canMove;
    private bool _isRevengeMode;

    private int _missCount;
    private int _currentHealth;
    private float _boostTimer;
    private float _revengeTimer;
    private float _powerUpTimer;
    private float _boostForce;
    private float _appliedForce;
    private float _appliedTorque;

    public bool _isMagnetOn;
    public bool _isShieldOn;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movement = GetComponent<PlayerMovement>();

        _isRevengeMode = false;
        _isMagnetOn = false;
        _isShieldOn = false;
        _canMove = false;

        _boostObject.SetActive(false);
        _revengeObject.SetActive(false);
        _shieldObject.SetActive(false);
        _magnetObject.SetActive(false);
    }

    private void Start()
    {
        _moveStartForce = GameManager.instance.PlayerStartSpeed;
        _startTorque = _moveStartForce * 1200 / 24;
        _appliedForce = _moveStartForce;
        _appliedTorque = _startTorque;
        _boostForce = 1.5f * _moveStartForce;

        _missCount = 0;
        _startBoostTimer = GameManager.instance.PlayerBoostTimer;
        _startRevengeTimer = GameManager.instance.PlayerRevengeTimer;
        _boostTimer = _startBoostTimer;
        _revengeTimer = _startRevengeTimer;
        _powerUpTimer = 4f;
        _startHealth = GameManager.instance.PlayerHealth;
        _currentHealth = _startHealth;
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_HEALTH, new Dictionary<string, object>()
            {
                {Constants.ScoreMessage.AMOUNT,_currentHealth}
            });

        _startSprite = GameManager.instance.PlayerSprite;
        _spriteRenderer.sprite = _startSprite;

    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.GAME_START, StartGame);
        EventManager.StartListening(Constants.EventNames.MISS, CalculateMiss);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.GAME_START, StartGame);
        EventManager.StopListening(Constants.EventNames.MISS, CalculateMiss);
    }

    private void Update()
    {
        _input = _movement._horizontal;
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        _rb.velocity = _rb.velocity * 0.99f;
        Vector3 moveDirection = _directionObject.position - transform.position;
        _rb.AddForce(moveDirection * _appliedForce);
        _rb.angularVelocity = _rb.angularVelocity * 0.95f;
        if (Mathf.Abs(_input) > 0.05f)
        {
            _rb.AddTorque(-_appliedTorque * Time.deltaTime * _input);
            foreach (var item in _trails)
            {
                item.emitting = true;
            }
        }
        else
        {
            foreach (var item in _trails)
            {
                item.emitting = false;
            }
        }
    }

    private void StartGame(Dictionary<string,object> message)
    {
        _canMove = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag(Constants.Tags.ENEMY))
        {
            if(_isRevengeMode)
            {
                _missCount++;
                EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object>() {
                    { Constants.ScoreMessage.TYPE , Constants.ScoreMessage.REVENGE },
                    { Constants.ScoreMessage.AMOUNT, _missCount },
                    { Constants.ScoreMessage.POSITION, transform.position }
                });
                var explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
                Destroy(collision.gameObject);
                return;
            }

            if (_isShieldOn) return;

            _currentHealth--;
            if (_currentHealth > 0)
            {
                StartCoroutine(ApplyBooost());
                StartCoroutine(ApplyShield());
            }
            else
            {
                _canMove = false;
                GameManager.instance.GameOver();
                Destroy(gameObject, 2f);
            }
            EventManager.TriggerEvent(Constants.EventNames.UPDATE_HEALTH, new Dictionary<string, object>()
            {
                {Constants.ScoreMessage.AMOUNT,_currentHealth}
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.MAGNET))
        {
            StartCoroutine(ApplyMagnet());
            Destroy(collision.gameObject);
        }
        else if(collision.CompareTag(Constants.Tags.SHIELD))
        {
            StartCoroutine(ApplyShield());
            Destroy(collision.gameObject);
        }
        else if(collision.CompareTag(Constants.Tags.BOMB))
        {
            ApplyBomb();
            SoundManager.instance.PlaySound(_bombClip);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator ApplyMagnet()
    {
        float timer = _powerUpTimer;
        _isMagnetOn = true;
        _magnetObject.SetActive(true);
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _isMagnetOn = false;
        _magnetObject.SetActive(false);
    }

    IEnumerator ApplyShield()
    {
        float timer = _powerUpTimer;
        _isShieldOn= true;
        _shieldObject .SetActive(true);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _isShieldOn = false;
        _shieldObject.SetActive(false);
    }

    private void ApplyBomb()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnBombed();
        }
    }

    private void CalculateMiss(Dictionary<string,object> message)
    {
        if (_isRevengeMode || _isShieldOn || !_canMove) return;
        switch(_missCount)
        {
            case 0: case 1:
                if(_boostTimer >= _startBoostTimer)
                {
                    StartCoroutine(ApplyBooost());
                }
                else
                {
                    _boostTimer = _startBoostTimer;
                }
                _missCount++;
                EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object>() {
                    { Constants.ScoreMessage.TYPE , Constants.ScoreMessage.MISS },
                    { Constants.ScoreMessage.AMOUNT, _missCount },
                    { Constants.ScoreMessage.POSITION, transform.position }
                });
                break;
            case 2:
                _missCount++;
                EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object>() {
                    { Constants.ScoreMessage.TYPE , Constants.ScoreMessage.MISS },
                    { Constants.ScoreMessage.AMOUNT, _missCount },
                    { Constants.ScoreMessage.POSITION, transform.position }
                });
                _missCount = 0;
                _isRevengeMode = true;
                StartCoroutine(ApplyRevenge());
                break;
            default:
                break;
        }
    }


    IEnumerator ApplyBooost()
    {
        _boostObject.SetActive(true);
        _appliedForce = _boostForce;
        while (_boostTimer > 0f)
        {
            _boostTimer -= Time.deltaTime;
            yield return null;
        }
        _boostTimer = _startBoostTimer;
        _boostObject.SetActive(false);
        _appliedForce = _moveStartForce;
    }

    IEnumerator ApplyRevenge()
    {
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_SCORE, new Dictionary<string, object> {
            { Constants.ScoreMessage.TYPE, Constants.ScoreMessage.REVENGE },
            { Constants.ScoreMessage.AMOUNT, _missCount },
            { Constants.ScoreMessage.POSITION, transform.position }
        });

        transform.localScale = Vector3.one * 2f;
        _revengeObject.SetActive(true);
        _appliedTorque *= 10f;
        
        while(_revengeTimer > 0f)
        {
            _revengeTimer -= Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        _revengeObject.SetActive(false);
        _isRevengeMode = false;
        _revengeTimer = _startRevengeTimer;
        _missCount = 0;
        _appliedTorque = _startTorque;
    }
}