using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� �������� �������� ������������
/// </summary>
public class OpacitySoundController : MonoBehaviour
{
    [Tooltip("���������� ���������� ������� ��������� ��������")]
    [SerializeField]
    OpacityController opacityController = null;

    [Tooltip("���� ���������/���������� ������ ������������")]
    [SerializeField]
    AudioSource opacityModeChangedSound = null;

    [Tooltip("���� ���������� �������������/������������ �������� ���������")]
    [SerializeField]
    AudioSource opacityLimitAchievedSound = null;

    /// <summary>
    /// ���������� �������� ���������
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
    /// ���������� ������� ��������� �������� ��������� ����������� ���������
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
    /// ��������� ����
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
