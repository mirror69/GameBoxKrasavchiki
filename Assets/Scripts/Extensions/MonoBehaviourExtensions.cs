using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void StopAndNullCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
    {
        if (coroutine == null)
        {
            return;
        }
        monoBehaviour.StopCoroutine(coroutine);
        coroutine = null;
    }
}
