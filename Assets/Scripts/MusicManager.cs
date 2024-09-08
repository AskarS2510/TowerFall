using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _musicSource;
    [SerializeField] private string _musicName;

    void Start()
    {
        _musicSource = GetComponent<AudioSource>();

        EventManager.RaisedSlider.AddListener(ChangeVolume);

        if (_musicName == "Effects")
            EventManager.DoneDestruction.AddListener(PlayEffects);
    }

    private void ChangeVolume(string sourceName, float volume)
    {
        if (sourceName != _musicName)
            return;

        _musicSource.volume = volume;
    }

    private void PlayEffects()
    {
        if (GameManager.DestroyedOnWave > 0)
            _musicSource.Play();
    }
}
