using System.Collections.Generic;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _ambientAudioList;

    private void Start()
    {
        EventManager.GameIsWon.AddListener(PlayWinLoseMusic);
    }

    private void PlayWinLoseMusic(bool isWin)
    {
        _ambientAudioList[2].Stop();

        if (isWin)
            _ambientAudioList[0].Play();
        else
            _ambientAudioList[1].Play();
    }

    private void StartAmbientDefault()
    {
        _ambientAudioList[0].Stop();
        _ambientAudioList[1].Stop();

        _ambientAudioList[2].Play();
    }
}
