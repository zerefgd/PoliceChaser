using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    private Vector3 _offset;
    private bool _hasGameFinished;

    private void Start()
    {
        _hasGameFinished = false;
        _offset = transform.position - _player.position;
    }

    private void OnEnable()
    {
        EventManager.StartListening(Constants.EventNames.GAME_OVER, GameOver);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Constants.EventNames.GAME_OVER, GameOver);
    }

    private void LateUpdate()
    {
        if (_hasGameFinished) return;
        transform.position = _player.position + _offset;
    }

    private void GameOver(Dictionary<string,object> message)
    {
        _hasGameFinished = true;
    }

}
