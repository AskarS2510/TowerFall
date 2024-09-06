using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;
    private Vector2 _startPos;
    private Vector2 _currentPos;
    private const float _swipeScreenPercent = 0.1f;
    private readonly float _swipeForce = _swipeScreenPercent * Screen.width;
    private readonly float _doubleSwipe = _swipeScreenPercent * Screen.width * 2;
    // Косинус и синус -45 градусов
    private readonly float _cos = Mathf.Sqrt(2) / 2;
    private readonly float _sin = -Mathf.Sqrt(2) / 2;

    private void Start()
    {
        //Application.targetFrameRate = 30;

        EventManager.DoneRotation.AddListener(RotateControls);

        EventManager.PausedGame.AddListener(() => gameObject.SetActive(false));
        EventManager.UnpausedGame.AddListener(() => gameObject.SetActive(true));
    }

    private void Update()
    {
        ProcessKeyboardInput();

        ProcessTouchInput();
    }

    private void ProcessTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _currentPos = touch.position;
                Vector2 deltaPos = _currentPos - _startPos;

                Vector2 angledDelta = AngledCoords(deltaPos);
                Vector2 angledStartPos = AngledCoords(_startPos);

                if (angledDelta.y > _swipeForce)
                {
                    angledStartPos = angledStartPos + new Vector2(0, _doubleSwipe);
                    _startPos = DefaultCoords(angledStartPos);

                    EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
                }

                if (angledDelta.y < -_swipeForce)
                {
                    angledStartPos = angledStartPos + new Vector2(0, -_doubleSwipe);
                    _startPos = DefaultCoords(angledStartPos);

                    EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
                }

                if (angledDelta.x < -_swipeForce)
                {
                    angledStartPos = angledStartPos + new Vector2(-_doubleSwipe, 0);
                    _startPos = DefaultCoords(angledStartPos);

                    EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
                }

                if (angledDelta.x > _swipeForce)
                {
                    angledStartPos = angledStartPos + new Vector2(_doubleSwipe, 0);
                    _startPos = DefaultCoords(angledStartPos);

                    EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
                }
            }
        }
    }

    private Vector2 AngledCoords(Vector2 pos)
    {
        float x = pos.x;
        float y = pos.y;

        return new Vector2(x * _cos + y * _sin, y * _cos - x * _sin);
    }

    private Vector2 DefaultCoords(Vector2 pos)
    {
        float x = pos.x;
        float y = pos.y;

        return new Vector2(x * _cos - y * _sin, x * _sin + y * _cos);
    }

    private void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);

        if (Input.GetKeyDown(KeyCode.A))
            EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);

        if (Input.GetKeyDown(KeyCode.S))
            EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);

        if (Input.GetKeyDown(KeyCode.D))
            EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            EventManager.RaisedSwitchSpeed?.Invoke();

        if (Input.GetKeyDown(KeyCode.Space))
            EventManager.RaisedDropDown?.Invoke();

        //if (Input.GetKeyUp(KeyCode.DownArrow))
        //    EventManager.TurnedOffSpeed?.Invoke();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            EventManager.RaisedRotate?.Invoke(true, false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            EventManager.RaisedRotate?.Invoke(false, true);
        }
    }

    private void RotateControls(bool left, bool right)
    {
        if (right)
        {
            int temp = _upArrowX;

            _upArrowX = -_upArrowZ;

            _upArrowZ = temp;


            temp = _leftArrowX;

            _leftArrowX = -_leftArrowZ;

            _leftArrowZ = temp;
        }

        if (left)
        {
            int temp = _upArrowX;

            _upArrowX = _upArrowZ;

            _upArrowZ = -temp;


            temp = _leftArrowX;

            _leftArrowX = _leftArrowZ;

            _leftArrowZ = -temp;
        }
    }
}
