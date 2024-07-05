using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    [SerializeField] GameObject Joystick = null;
    [SerializeField] GameObject JoystickBackground = null;

    internal Vector2 joystickVector { get; private set; } = Vector2.zero;
    private Vector2 joystickTouchPosition = Vector2.zero;
    private Vector2 joystickOriginalPosition = Vector2.zero;
    private float joystickRadius = 0.0f;

    private GameManager gameManager = null;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.InitJoystick(this);

        joystickOriginalPosition = JoystickBackground.transform.localPosition;
        joystickRadius = JoystickBackground.GetComponent<RectTransform>().sizeDelta.y / 4.0f;
    }

    private void OnDisable()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        JoysticPointerUp();
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
        PointerUp();
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Update()
    {
        if (gameManager.Transition || gameManager.Player.IsDashing)
        {
            JoysticPointerUp();
            return;
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                JoysticPointerDown(touch.position);
            if (touch.phase == TouchPhase.Moved)
                JoysticDrag(touch.position);
            if (touch.phase == TouchPhase.Ended)
                JoysticPointerUp();
        }
    }

    private void JoysticPointerDown(Vector2 position)
    {
        JoystickBackground.transform.position = position;
        joystickTouchPosition = position;
    }

    private void JoysticDrag(Vector2 position)
    {
        Vector2 dragPos = position;
        joystickVector = (dragPos - joystickTouchPosition).normalized;

        float joystickDistance = Vector2.Distance(dragPos, joystickTouchPosition);

        if (joystickDistance < joystickRadius)
            Joystick.transform.position = joystickTouchPosition + joystickVector * joystickDistance;

        else
            Joystick.transform.position = joystickTouchPosition + joystickVector * joystickRadius;
    }

    private void JoysticPointerUp()
    {
        joystickVector = Vector2.zero;
        JoystickBackground.transform.localPosition = joystickOriginalPosition;
        Joystick.transform.localPosition = Vector2.zero;
    }

#endif

#if UNITY_EDITOR || UNITY_STANDALONE
    private void Update()
    {
        if (gameManager.Transition || gameManager.Player.IsDashing)
        {
            PointerUp();
        }
    }

    public void PointerDown()
    {
        if (gameManager.Transition)
            return;

        JoystickBackground.transform.position = Input.mousePosition;
        joystickTouchPosition = Input.mousePosition;
    }


    public void Drag(BaseEventData baseEventData)
    {
        if (gameManager.Transition)
            return;

        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVector = (dragPos - joystickTouchPosition).normalized;

        float joystickDistance = Vector2.Distance(dragPos, joystickTouchPosition);

        if (joystickDistance < joystickRadius)
            Joystick.transform.position = joystickTouchPosition + joystickVector * joystickDistance;

        else
            Joystick.transform.position = joystickTouchPosition + joystickVector * joystickRadius;
    }

    public void PointerUp()
    {
        joystickVector = Vector2.zero;
        JoystickBackground.transform.localPosition = joystickOriginalPosition;
        Joystick.transform.localPosition = Vector2.zero;
    }
#endif
}
