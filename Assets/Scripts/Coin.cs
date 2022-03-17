using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private GameObject _player;

    [SerializeField]
    private AudioClip _coinClip;

    private void Start()
    {
        _player = GameManager.instance._player;
        if (_player.GetComponent<Player>()._isMagnetOn)
        {
            StartCoroutine(StartMoving());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.Tags.PLAYER))
        {
            StartCoroutine(StartMoving());
        }
    }

    IEnumerator StartMoving()
    {
        Vector3 offset = transform.position - _player.transform.position;
        float speed = offset.magnitude * 5f;
        while(offset.sqrMagnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
            offset = transform.position - _player.transform.position;
            yield return null;
        }
        EventManager.TriggerEvent(Constants.EventNames.UPDATE_COIN, null);
        SoundManager.instance.PlaySound(_coinClip);
        Destroy(gameObject);
    }
}
