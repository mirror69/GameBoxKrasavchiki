using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDeathEffect : DeathEffect
{
    [SerializeField]
    private Vector3 dissolveStartOffset;
    [SerializeField]
    private Vector3 dissolveEndOffset;

    private List<Material> materials;
    private Vector3 dissolveChangeDirection;
    private float dissolveChangeVectorLength;

    protected override void ProcessDeath()
    {
        base.ProcessDeath();
        StartCoroutine(PerformDeath(deathDuration));
    }

    protected override void Awake()
    {
        base.Awake();
        // Добавляем в список все материалы объекта и дочерних объектов для последующего изменения
        // их прозрачности
        Renderer[] meshRenderers = damageableObject.Transform.GetComponentsInChildren<Renderer>();
        materials = new List<Material>();
        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.materials)
            {
                materials.Add(material);
                material.SetDirectionDissolve(dissolveStartOffset);
            }
        }
        Vector3 dissolveChangeVector = dissolveEndOffset - dissolveStartOffset;
        dissolveChangeVectorLength = dissolveChangeVector.magnitude;
        dissolveChangeDirection = dissolveChangeVector / dissolveChangeVectorLength;
    }

    private IEnumerator PerformDeath(float deathDuration)
    {
        float changeSpeed = dissolveChangeVectorLength / deathDuration;
        float endTime = Time.time + deathDuration;
        Vector3 currentDissolveOffset = dissolveStartOffset;
        while (Time.time < endTime)
        {
            yield return null;
            currentDissolveOffset += changeSpeed * Time.deltaTime * dissolveChangeDirection;
            foreach (var material in materials)
            {
                material.SetDirectionDissolve(currentDissolveOffset);
            }
        }
    }
}
