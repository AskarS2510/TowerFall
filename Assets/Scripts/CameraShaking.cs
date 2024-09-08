using DG.Tweening;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private float _shakeDuration;
    private float _shakeStrength;
    private int _superShakeCount = 10;

    void Start()
    {
        _shakeDuration = GameManager.DelayBetweenWaves;

        EventManager.DoneDestruction.AddListener(ShakeCamera);
    }

    private void ShakeCamera()
    {
        SetShakeStrength();

        transform.DOShakePosition(_shakeDuration, _shakeStrength);
    }

    private void SetShakeStrength()
    {
        int destroyedCount = GameManager.DestroyedOnWave;

        if (destroyedCount == 0)
        {
            _shakeStrength = 0.0f;

            return;
        }

        if (destroyedCount <= _superShakeCount)
        {
            _shakeStrength = 0.2f;

            return;
        }

        if (destroyedCount > _superShakeCount)
        {
            _shakeStrength = 1f;

            return;
        }
    }
}
