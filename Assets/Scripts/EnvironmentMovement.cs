using UnityEngine;

public class EnvironmentMovement : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private float _startArg;
    public static Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;

        transform.Rotate(Vector3.up, _rotateSpeed * Time.unscaledTime);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.unscaledDeltaTime);
        transform.position = _startPosition + Vector3.up * Mathf.Sin(_startArg + Time.unscaledTime * _verticalSpeed);
    }
}
