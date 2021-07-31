using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (Physics.Raycast(startPoint, direction, out RaycastHit hitInfo, maxDistance, 
            GameManager.Instance.ObstacleLayers, QueryTriggerInteraction.Ignore))
        {
            return hitInfo;
        }
        else
        {
            return new RaycastHit();
        }
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
