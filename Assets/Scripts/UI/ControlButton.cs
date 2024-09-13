using UnityEngine;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    private Button _button;
    private CubeBlock _cubeBlock;
    [SerializeField] private int _xIndex, _zIndex;
    [SerializeField] private bool _rotateLeft, _rotateRight;

    private void Start()
    {
        if (GameManager.Instance.userDeviceType == DeviceType.Desktop)
        {
            gameObject.SetActive(false);

            return;
        }

        _button = GetComponent<Button>();

        _button.onClick.AddListener(CheckRotatePress);
        _button.onClick.AddListener(CheckMovePress);

        EventManager.DoneRotation.AddListener(RotateControls);
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

    public void ClickedDropDown()
    {
        EventManager.RaisedDropDown?.Invoke();
    }
}
