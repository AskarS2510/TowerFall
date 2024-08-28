using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;
    private const bool k_isLeftRotate = true;
    private const bool k_isRightRotate = true;

    private void Start()
    {
        EventManager.RaisedRotate.AddListener(RotateControls);
    }

    private void Update()
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
            if (CameraController.s_isRotating)
            {
                return;
            }

            EventManager.RaisedRotate?.Invoke(k_isLeftRotate, !k_isRightRotate);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CameraController.s_isRotating)
            {
                return;
            }

            EventManager.RaisedRotate?.Invoke(!k_isLeftRotate, k_isRightRotate);
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
