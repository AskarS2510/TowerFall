using UnityEngine;

public class RotationController : MonoBehaviour
{
    public static bool s_isRotating;

    private int _rotateSpeed = 360;
    //private float _rotateSpeed = 20f;
    private int _direction;
    private Quaternion _desiredRotQ;
    private float _positionOffset = 1.2f;
    private float _startMinimapRotationY = -45;
    [SerializeField] private GameObject _minimapCamera;

    private void Start()
    {
        s_isRotating = false;

        EventManager.RaisedRotate.AddListener(StartRotation);
        EventManager.ChangedPosition.AddListener(ChangeMinimapPosition);
        EventManager.SpawnedPlayerBlock.AddListener(ResetPosition);
    }

    private void LateUpdate()
    {
        if (s_isRotating)
        {
            var step = _rotateSpeed * Time.deltaTime;
            //var step = _rotateSpeed;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _desiredRotQ, step);

            float yRotation = _startMinimapRotationY + transform.eulerAngles.y;

            _minimapCamera.transform.eulerAngles = new Vector3(_minimapCamera.transform.eulerAngles.x, yRotation, _minimapCamera.transform.eulerAngles.z);

            if (transform.rotation.eulerAngles == _desiredRotQ.eulerAngles)
            {
                s_isRotating = false;
            }
        }
    }

    public void StartRotation(bool left, bool right)
    {
        if (s_isRotating)
        {
            return;
        }

        if ((left || right) == false)
        {
            return;
        }

        if (left)
        {
            _direction = 1;
        }

        if (right)
        {
            _direction = -1;
        }

        _desiredRotQ.eulerAngles = transform.rotation.eulerAngles + new Vector3(0, _direction * 90, 0);

        s_isRotating = true;
    }

    private void ChangeMinimapPosition(int xIndex, int zIndex)
    {
        _minimapCamera.transform.position = new Vector3(xIndex * _positionOffset, _minimapCamera.transform.position.y, zIndex * _positionOffset);
    }

    private void ResetPosition()
    {
        _minimapCamera.transform.position = new Vector3(0, _minimapCamera.transform.position.y, 0);
    }
}
