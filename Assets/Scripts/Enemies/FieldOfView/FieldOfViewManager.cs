using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Синглтон-менеджер объектов, реализующих поле зрения
/// </summary>
public class FieldOfViewManager : MonoBehaviour
{
    private readonly List<FieldOfView> fieldsOfView = new List<FieldOfView>();

    public static FieldOfViewManager Instance { get; private set; }
    public List<FieldOfView> FieldsOfView => fieldsOfView;

    public static RaycastHit GetFovRaycastHit(Vector3 startPoint, Vector3 direction, float maxDistance)
    {
        // Необходимо проверить все препятствия на пути, т.к. Raycast находит также триггерные коллайдеры,
        // которые необходимо исключать
        RaycastHit[] hitInfo = Physics.RaycastAll(startPoint, direction, maxDistance,
            GameManager.Instance.ObstacleLayers);

        foreach (var item in hitInfo)
        {
            if (item.collider != null && !item.collider.isTrigger)
            {
                return item;
            }
        }
        return new RaycastHit();
    }

    public void AddFieldOfView(FieldOfView fov)
    {
        fieldsOfView.Add(fov);
    }
    public void RemoveFieldOfView(FieldOfView fov)
    {
        fieldsOfView.Remove(fov);
    }

    private void Awake()
    {        
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
