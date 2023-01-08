using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class LorentzCurrentPresenter : MonoBehaviour
{
    [SerializeField] private Slider _currentSlider;
    [SerializeField] private Button _currentUpButton;
    [SerializeField] private Button _currentDownButton;
    [SerializeField] private Text _currentText;
    [SerializeField] private Slider _magneticSlider;
    [SerializeField] private Button _magneticUpButton;
    [SerializeField] private Button _magneticDownButton;
    [SerializeField] private Text _magneticText;
    [SerializeField] private LorentzForceWire _lorentzForceWire;
    // Start is called before the first frame update
    void Start()
    {
        _currentSlider.maxValue = 2.0f;
        _currentSlider.minValue = 0;

        _currentSlider.onValueChanged.AsObservable().Subscribe((value) =>
        {
            _lorentzForceWire.CurrentReactiveProperty.Value = value;
        });

        _currentUpButton.onClick.AsObservable().Subscribe((value) =>
        {
            _currentSlider.value += 0.1f;
        });

        _currentDownButton.onClick.AsObservable().Subscribe((value) =>
        {
            _currentSlider.value -= 0.1f;
        });

        _lorentzForceWire.CurrentReactiveProperty.Subscribe((value) =>
        {
            _currentSlider.value = value;
            _currentText.text = value.ToString();
        });

        _magneticSlider.maxValue = 1.0f;
        _magneticSlider.minValue = -1.0f;

        _magneticSlider.onValueChanged.AsObservable().Subscribe((value) =>
        {
            _lorentzForceWire.MagneticFluxDensityReactiveProperty.Value = value;
        });

        _magneticUpButton.onClick.AsObservable().Subscribe((value) =>
        {
            _magneticSlider.value += 0.1f;
        });

        _magneticDownButton.onClick.AsObservable().Subscribe((value) =>
        {
            _magneticSlider.value -= 0.1f;
        });

        _lorentzForceWire.MagneticFluxDensityReactiveProperty.Subscribe((value) =>
        {
            _magneticSlider.value = value;
            _magneticText.text = value.ToString();
        });


    }

    // Update is called once per frame
    void Update()
    {

    }
}
