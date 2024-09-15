using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    private float _waitBeforeEnable = 2f;
    private float _flyTime = 0.1f;
    [SerializeField] private AudioSource _animationAudio;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_waitBeforeEnable);

        transform.DOLocalMoveY(140, _flyTime).SetEase(Ease.OutQuad);

        _animationAudio.Play();
    }

    public void StartGame()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame?.Invoke();
    }
}
