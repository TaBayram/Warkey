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

    private float currentMaxValue = 0;
    private float currentValue = 0;
     

    private void Start() {

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
            currentMaxValue = slider.value;
        }


    }

}
