using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static bool s_isRotating;

    private int _rotateSpeed = 360;
    private int _direction;
    private Quaternion _desiredRotQ;
    private float _positionOffset = 1.2f;

    // Start is called before the first frame update
    private void Start()
    {
        s_isRotating = false;

        EventManager.ClickedRotate.AddListener(StartRotation);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (s_isRotating)
        {
            var step = _rotateSpeed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _desiredRotQ, step);

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
}
