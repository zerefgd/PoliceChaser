using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCar : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _cars;

    [SerializeField]
    private List<Gradient> _colors;

    private Vector3 _startPos;
    private TrailRenderer _trailRenderer;

    public void Init()
    {
        Vector2 spawnPos = UnityEngine.Random.Range(0, 2) == 0 ? new Vector2(UnityEngine.Random.Range(-6,6),0) :
            new Vector2(0, UnityEngine.Random.Range(-10, 10));
        transform.Translate(spawnPos);
        transform.Translate(new Vector2(-1, -1) * UnityEngine.Random.Range(20, 50));
        transform.eulerAngles = new Vector3(0, 0, -45);
        GetComponent<Rigidbody2D>().velocity = new Vector2(1, 1) * UnityEngine.Random.Range(4, 20);
        int randomIndex = Random.Range(0, _cars.Count);
        GetComponent<SpriteRenderer>().sprite = _cars[randomIndex];
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.colorGradient = _colors[randomIndex];
        _startPos = transform.position;
    }

    private void Update()
    {
        if(transform.position.x > 18f || transform.position.y > 20f)
        {
            _trailRenderer.enabled = false;
            transform.position = _startPos;
            Invoke("TrailOn", 1.2f);
        }
    }

    void TrailOn()
    {
        _trailRenderer.enabled = true;
    }
}
