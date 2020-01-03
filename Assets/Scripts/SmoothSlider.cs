using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothSlider : MonoBehaviour
{
    public float dampTime = 0.1f;
    Slider slider;

    float targetValue;
    float vel;

    public void SetTargetValue(float value)
    {
        targetValue = value;
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
        targetValue = slider.value;
    }

    private void Update()
    {
        slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref vel, dampTime);
    }

    
}
