using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер звуковых эффектов прозрачности
/// </summary>
public class OpacitySoundController : MonoBehaviour
{
    [Tooltip("Контроллер управления уровнем видимости объектов")]
    [SerializeField]
    OpacityController opacityController = null;

    [Tooltip("Звук включения/отключения режима прозрачности")]
    [SerializeField]
    AudioSource opacityModeChangedSound = null;

    [Tooltip("Звук достижения максимального/минимального значения видимости")]
    [SerializeField]
    AudioSource opacityLimitAchievedSound = null;

    /// <summary>
    /// Предыдущее значение видимости
    /// </summary>
    private float previousOpacityValue = 0;

    private void Start()
    {
        previousOpacityValue = opacityController.OpacityValue;
        opacityController.RegisterOpacityChangedListener(OnOpacityControllerOpacityChanged);
    }

    private void OnDestroy()
    {
        opacityController.UnregisterOpacityChangedListener(OnOpacityControllerOpacityChanged);
    }

    /// <summary>
    /// Обработчик события изменения значения видимости контроллера видимости
    /// </summary>
    private void OnOpacityControllerOpacityChanged()
    {
        float currentOpacityValue = opacityController.OpacityValue;

        if (previousOpacityValue < OpacityController.MaxOpacityValue
           && currentOpacityValue >= OpacityController.MaxOpacityValue
           || previousOpacityValue > OpacityController.MinOpacityValue
           && currentOpacityValue <= OpacityController.MinOpacityValue)
        {
            PlaySound(opacityLimitAchievedSound);
        }

        if (previousOpacityValue >= OpacityController.MaxOpacityValue
            && currentOpacityValue < OpacityController.MaxOpacityValue
            || previousOpacityValue <= OpacityController.MinOpacityValue
            && currentOpacityValue > OpacityController.MinOpacityValue)
        {
            PlaySound(opacityModeChangedSound);
        }
        previousOpacityValue = currentOpacityValue;
    }

    /// <summary>
    /// Проиграть звук
    /// </summary>
    /// <param name="audioSource"></param>
    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.Play();
    }
}
