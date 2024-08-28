using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    private Button _button;
    private CubeBlock _cubeBlock;
    [SerializeField] private int _xIndex, _zIndex;
    [SerializeField] private bool _rotateLeft, _rotateRight;
    [SerializeField] private bool _speedDown;
    //public CubeBlock Block;


    // Start is called before the first frame update
    private void Start()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            gameObject.SetActive(false);

            return;
        }

        _button = GetComponent<Button>();

        _button.onClick.AddListener(CheckRotatePress);
        _button.onClick.AddListener(CheckMovePress);
        
        EventManager.RaisedRotate.AddListener(RotateControls);
    }

    private void RotateControls(bool left, bool right)
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

    private void CheckRotatePress()
    {
        if (_rotateLeft == true || _rotateRight == true)
        {
            if (CameraController.s_isRotating)
            {
                return;
            }

            EventManager.RaisedRotate?.Invoke(_rotateLeft, _rotateRight);
        }
    }

    private void CheckMovePress()
    {
        if (_xIndex != 0 || _zIndex != 0)
        {
            EventManager.RaisedMove?.Invoke(_xIndex, _zIndex);
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
