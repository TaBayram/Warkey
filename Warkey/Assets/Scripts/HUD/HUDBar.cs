using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDBar : MonoBehaviour
{
    private const float valueThreshold = 0.5f;
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TMP_Text text;
    public float smoothing = 5f;
    private float targetValue;

    private float currentMaxValue = 0;
    private float currentValue = 0;
    private string currentText;


    [SerializeField] Unit preplacedUnit;
    private void Start() {
        if (preplacedUnit != null) {
            preplacedUnit.FinitePropertyChanged += Unit_FinitePropertyChanged;
        }
    }

    private void Unit_FinitePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if (e.PropertyName == "health") {
            SetMaxValue(((FiniteField)sender).Max);
            SetValue(((FiniteField)sender).Current);
        }
    }

    public void SetMaxValue(float value)
    {
        if(currentMaxValue != value) {
            slider.maxValue = value;
            currentMaxValue = value;
        }        
    }
    public void SetValue(float value)
    {
        if (value != currentValue && Mathf.Abs(currentValue - value) > valueThreshold)
        {
            targetValue = value;
        }
       
    }

    private void Update()
    {
        float difference = Mathf.Abs(currentValue - targetValue);
        if (difference > smoothing) {
            slider.value = Mathf.Lerp(slider.value,targetValue,smoothing*Time.deltaTime);
            fill.color = gradient.Evaluate(slider.normalizedValue);
            currentValue = slider.value;
        }
        else if(difference != 0) {
            slider.value = targetValue;
            currentValue = slider.value;
        }
        if (text != null && text.text != currentText) {
            text.text = ((int)currentValue) + " / " + ((int)currentMaxValue);
        }


    }

}
