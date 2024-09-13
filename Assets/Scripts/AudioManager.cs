using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private string _audioName;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        EventManager.RaisedSlider.AddListener(ChangeVolume);

        if (gameObject.name == "Explosion")
            EventManager.DoneDestruction.AddListener(PlayEffects);

        LoadSettings();
    }

    private void ChangeVolume(string sourceName, float volume)
    {
        if (sourceName != _audioName)
            return;

        _audioSource.volume = volume;
    }

    private void PlayEffects()
    {
        if (GameManager.Instance.DestroyedOnWave > 0)
            _audioSource.Play();
    }

    private void LoadSettings()
    {
        _audioSource.volume = PlayerPrefs.GetFloat(_audioName, GameManager.Instance.DefaultAudioValue);
    }
}
