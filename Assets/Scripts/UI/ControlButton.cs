using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlButton : MonoBehaviour
{
    private Button _button;
    private CubeBlock _cubeBlock;
    [SerializeField] private int _xIndex, _zIndex;
    [SerializeField] private bool _rotateLeft, _rotateRight;
    [SerializeField] private bool _speedDown;
    //public CubeBlock Block;


    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();

        //EventManager.StoppedMovement.AddListener(GrabControl);
        //EventManager.SpawnedPlayerBlock.AddListener(UpdateControlledObject);

        _button.onClick.AddListener(CheckRotatePress);
        _button.onClick.AddListener(CheckMovePress);
        
        EventManager.ClickedRotate.AddListener(RotateControls);
    }

    void RotateControls(bool left, bool right)
    {
        if (right)
        {
            int temp = _xIndex;

            _xIndex = -_zIndex;

            _zIndex = temp;
        }

        if (left)
        {
            int temp = _xIndex;

            _xIndex = _zIndex;

            _zIndex = -temp;
        }
    }

    void CheckRotatePress()
    {
        if (_rotateLeft == true || _rotateRight == true)
        {
            if (CameraController.s_isRotating)
            {
                return;
            }

            EventManager.ClickedRotate?.Invoke(_rotateLeft, _rotateRight);
        }
    }

    void CheckMovePress()
    {
        if (_xIndex != 0 || _zIndex != 0)
        {
            EventManager.ClickedMove?.Invoke(_xIndex, _zIndex);
        }
    }

    public void ClickedSpeedDown()
    {
        if (_speedDown)
            EventManager.TurnedOnSpeed?.Invoke();
    }

    public void UnclickedSpeedDown()
    {
        if (_speedDown)
            EventManager.TurnedOffSpeed?.Invoke();
    }
}
