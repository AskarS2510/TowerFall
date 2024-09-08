using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    private float _startValue;
    private Slider _slider;
    [SerializeField] private string sourceName;
    [SerializeField] private Button _resetButton;

    private void Start()
    {
        _slider = GetComponent<Slider>();

        _startValue = _slider.value;

        _slider.onValueChanged.AddListener(ChangeValue);

        if (_resetButton != null)
            _resetButton.onClick.AddListener(ResetValue);
    }

    public void ChangeValue(float volume)
    {
        EventManager.RaisedSlider?.Invoke(sourceName, volume);
    }

    public void ResetValue()
    {
        _slider.value = _startValue;

        ChangeValue(_startValue);
    }
}
