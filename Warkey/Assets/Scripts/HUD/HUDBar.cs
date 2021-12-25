using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDBar : MonoBehaviour
{
    private const float valueThreshold = 0.5f;
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public float smoothing = 5f;
    private float targetValue;

    public Unit unit;

    private void Start() {
        if (unit != null) {
            unit.FinitePropertyChanged += Unit_FinitePropertyChanged;
        }
    }

    private void Unit_FinitePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if (e.PropertyName == "health") {
            SetMaxValue(((FiniteField)sender).Max);
            SetValue(((FiniteField)sender).Current);
        }
    }


    public void SetMaxValue(float health)
    {
        slider.maxValue = health;
        
    }
    public void SetValue(float health)
    {
        if (Mathf.Abs(slider.value - health)> valueThreshold)
        {
            targetValue = health;
        }
       
    }

    private void Update()
    {
        
        if(targetValue < slider.value)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, smoothing * Time.deltaTime);
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
        else if(targetValue > slider.value)
        {
            slider.value = Mathf.Lerp(targetValue, slider.value, smoothing * Time.deltaTime);
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }


    }

}
