using UnityEngine;
using UnityEngine.UI;
using YG;

public class SliderManager : MonoBehaviour
{
    private float _startValue;
    private Slider _slider;
    [SerializeField] private string _sourceName;
    [SerializeField] private Button _resetButton;

    private void Start()
    {
        _slider = GetComponent<Slider>();

        _startValue = _slider.value;

        _slider.onValueChanged.AddListener(ChangeValue);

        if (_resetButton != null)
            _resetButton.onClick.AddListener(ResetValue);

        if (GameManager.Instance.userDeviceType == DeviceType.Desktop && name == "Slider Sensitivity")
            gameObject.SetActive(false);

        LoadSettings();
    }

    public void ChangeValue(float volume)
    {
        EventManager.RaisedSlider?.Invoke(_sourceName, volume);

        SaveSettings();
    }

    public void ResetValue()
    {
        _slider.value = _startValue;

        ChangeValue(_startValue);
    }

    private void LoadSettings()
    {
        //if (name == "Slider Sensitivity")
        //    _slider.value = PlayerPrefs.GetFloat(_sourceName, GameManager.Instance.DefaultSense);
        //else
        //    _slider.value = PlayerPrefs.GetFloat(_sourceName, GameManager.Instance.DefaultAudioValue);

        if (_sourceName == "Music")
            _slider.value = YandexGame.savesData.MusicVolume;

        if (_sourceName == "Effects")
            _slider.value = YandexGame.savesData.EffectsVolume;

        if (_sourceName == "Sensitivity")
            _slider.value = YandexGame.savesData.SenseValue;

        ChangeValue(_slider.value);
    }

    private void SaveSettings()
    {
        //PlayerPrefs.SetFloat(_sourceName, _slider.value);

        if (_sourceName == "Music")
            YandexGame.savesData.MusicVolume = _slider.value;

        if (_sourceName == "Effects")
            YandexGame.savesData.EffectsVolume = _slider.value;

        if (_sourceName == "Sensitivity")
            YandexGame.savesData.SenseValue = _slider.value;
    }
}
