using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject attackingDisplayPanel = null;

    private FieldOfView[] fovArray = null;
    private int detectedCounter = 0;

    private void Start()
    {
        attackingDisplayPanel.SetActive(false);
        fovArray = FindObjectsOfType<FieldOfView>();
        foreach (var item in fovArray)
        {
            item.RegisterTargetDetectedListener(OnFovTargetDetected);
            item.RegisterTargetLostListener(OnFovTargetLost);
        }
    }

    private void OnDestroy()
    {
        foreach (var item in fovArray)
        {
            item.UnregisterTargetDetectedListener(OnFovTargetDetected);
            item.UnregisterTargetLostListener(OnFovTargetLost);
        }
    }

    private void OnFovTargetDetected(Transform targetTransform)
    {
        detectedCounter++;
        attackingDisplayPanel.SetActive(true);
    }
    private void OnFovTargetLost()
    {
        detectedCounter--;
        if (detectedCounter == 0)
        {
            attackingDisplayPanel.SetActive(false);
        }
    }
}
