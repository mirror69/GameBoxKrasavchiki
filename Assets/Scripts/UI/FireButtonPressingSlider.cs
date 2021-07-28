using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireButtonPressingSlider : MonoBehaviour
{
    [SerializeField] private Slider sliderValue;
    [SerializeField] private Text healthValueText;
    [SerializeField] private DamageableObject playerHealth;

    private void Update()
    {
        sliderValue.value = playerHealth.Health / playerHealth.MaxHealth;
        healthValueText.text = $"{playerHealth.Health} / {playerHealth.MaxHealth}";
    }

}
