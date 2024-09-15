using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private string _audioName;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        EventManager.RaisedSlider.AddListener(ChangeVolume);

        SubscribeByNames();

        LoadSettings();
    }

    private void SubscribeByNames()
    {
        if (gameObject.name == "StickSound")
            EventManager.DoneDestruction.AddListener(PlayEffectsStick);

        if (gameObject.name == "NewLayerSound")
            EventManager.AnimatedBottomLayer.AddListener(PlayEffectsNewLayer);
    }

    private void ChangeVolume(string sourceName, float volume)
    {
        if (sourceName != _audioName)
            return;

        _audioSource.volume = volume;
    }

    private void PlayEffectsStick()
    {
        if (GameManager.Instance.DestroyedOnWave == 0)
            _audioSource.Play();
    }

    private void PlayEffectsNewLayer()
    {
        _audioSource.Play();
    }

    private void LoadSettings()
    {
        _audioSource.volume = PlayerPrefs.GetFloat(_audioName, GameManager.Instance.DefaultAudioValue);
    }
}
