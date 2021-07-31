using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private Slider sliderValue;
    [SerializeField] private Text healthValueText;
    [SerializeField] private DamageableObject healthObject;

    private void Update()
    {
        sliderValue.value = healthObject.Health / healthObject.MaxHealth;
        healthValueText.text = $"{healthObject.Health} / {healthObject.MaxHealth}";
    }

}
