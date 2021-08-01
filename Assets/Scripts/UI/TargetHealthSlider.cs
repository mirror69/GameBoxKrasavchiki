using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetHealthSlider : MonoBehaviour
{
    [SerializeField]
    private Slider sliderValue;
    [SerializeField]
    private Text healthValueText;
    [SerializeField]
    private PlayerInput playerInput;

    private void Update()
    {
        IDamageable target = playerInput.GetTarget();
        if (target == null)
        {
            if (IsSliderActive())
            {
                SetSliderActive(false);
            }
            return;
        }

        sliderValue.value = target.Health / (float)target.MaxHealth;
        healthValueText.text = $"{target.Health} / {target.MaxHealth}";
        
        if (!IsSliderActive())
        {
            SetSliderActive(true);
        }        
    }

    private bool IsSliderActive()
    {
        return sliderValue.gameObject.activeSelf;
    }
    private void SetSliderActive(bool active)
    {
        sliderValue.gameObject.SetActive(active);
    }
}
