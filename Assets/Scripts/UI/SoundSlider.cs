using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    private Slider _slider;
    [SerializeField] private string sourceName;

    private void Start()
    {
        _slider = GetComponent<Slider>();

        _slider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ChangeVolume(float volume)
    {
        EventManager.RaisedVolume?.Invoke(sourceName, volume);
    }
}
