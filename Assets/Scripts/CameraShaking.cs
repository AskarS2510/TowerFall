using DG.Tweening;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private float _shakeDuration;
    private float _shakeStrength;
    private int _superShakeCount = 10;

    void Start()
    {
        _shakeStrength = 0;

        _shakeDuration = GameManager.Instance.EffectsDuration;

        EventManager.DoneDestruction.AddListener(ShakeCamera);
        EventManager.Stuck.AddListener(ShakeNoWait);
    }

    private void ShakeCamera()
    {
        if (GameManager.Instance.DestroyedOnWave == 0)
            return;

        SetShakeStrength();

        transform.DOShakePosition(_shakeDuration, _shakeStrength);
    }
    private void ShakeNoWait()
    {
        transform.DOShakePosition(_shakeDuration, Vector3.up * 0.5f);
    }

    private void SetShakeStrength()
    {
        int destroyedCount = GameManager.Instance.DestroyedOnWave;

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
