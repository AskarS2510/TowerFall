using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;
    private const bool k_isLeftRotate = true;
    private const bool k_isRightRotate = true;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private float _swipeForce = 0.08f * Screen.width;

    private void Start()
    {
        EventManager.RaisedRotate.AddListener(RotateControls);
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

            //EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);

            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _endPos = touch.position;
                Vector2 deltaPos = _endPos - _startPos;

                if (deltaPos.x > _swipeForce && deltaPos.y > _swipeForce)
                {
                    _startPos = _endPos;
                    EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
                }

                if (deltaPos.x < -_swipeForce && deltaPos.y < -_swipeForce)
                {
                    _startPos = _endPos;
                    EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
                }

                if (deltaPos.x < -_swipeForce && deltaPos.y > _swipeForce)
                {
                    _startPos = _endPos;
                    EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
                }

                if (deltaPos.x > _swipeForce && deltaPos.y < -_swipeForce)
                {
                    _startPos = _endPos;
                    EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
                }
            }
        }
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

        if (Input.GetKey(KeyCode.DownArrow))
            EventManager.TurnedOnSpeed?.Invoke();

        if (Input.GetKeyUp(KeyCode.DownArrow))
            EventManager.TurnedOffSpeed?.Invoke();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!CameraController.s_isRotating)
            {
                EventManager.RaisedRotate?.Invoke(k_isLeftRotate, !k_isRightRotate);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!CameraController.s_isRotating)
            {
                EventManager.RaisedRotate?.Invoke(!k_isLeftRotate, k_isRightRotate);
            }
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
