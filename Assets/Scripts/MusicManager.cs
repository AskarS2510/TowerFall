using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _musicSource;

    void Start()
    {
        _musicSource = GetComponent<AudioSource>();

        EventManager.RaisedSlider.AddListener(ChangeVolume);
    }

    private void ChangeVolume(string sourceName, float volume)
    {
        if (sourceName != "Music")
            return;

        _musicSource.volume = volume;
    }
}
