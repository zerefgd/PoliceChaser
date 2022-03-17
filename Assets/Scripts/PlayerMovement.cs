using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Button _left, _right;

    private bool _isLeftPressed, _isRightPressed;
    [HideInInspector]
    public float _horizontal;

    private void Update()
    {
#if UNITY_EDITOR
        _horizontal = Input.GetAxis("Horizontal");
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        _horizontal = _isLeftPressed ? -1 : _isRightPressed ? 1 : 0;
#endif
    }

    private void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        _isLeftPressed = _isRightPressed = false;
        EventTrigger leftTrigger = _left.gameObject.AddComponent<EventTrigger>();
        EventTrigger rightTrigger = _right.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener(e => { _isLeftPressed = true; });
        leftTrigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener(e => { _isLeftPressed = false; });
        leftTrigger.triggers.Add(pointerUp);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener(e => { _isRightPressed= true; });
        rightTrigger.triggers.Add(pointerDown);

        pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener(e => { _isRightPressed= false; });
        rightTrigger.triggers.Add(pointerUp);
#endif
    }

}
