using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _musicSource;
    [SerializeField] private string _musicName;

    void Start()
    {
        _musicSource = GetComponent<AudioSource>();

        EventManager.RaisedSlider.AddListener(ChangeVolume);

        if (gameObject.name == "Explosion")
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
        if (GameManager.Instance.DestroyedOnWave > 0)
            _musicSource.Play();
    }
}
