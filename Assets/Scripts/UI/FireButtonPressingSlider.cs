using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireButtonPressingSlider : MonoBehaviour
{
    [SerializeField] private Slider sliderValue;
    [SerializeField] private PlayerInput playerInput;

    private void Update()
    {
        sliderValue.value = playerInput.fireButtonPressedTimer;
    }

}
