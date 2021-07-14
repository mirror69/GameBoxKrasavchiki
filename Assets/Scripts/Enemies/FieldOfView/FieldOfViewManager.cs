using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Синглтон-менеджер объектов, реализующих поле зрения
/// </summary>
public class FieldOfViewManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask obstacleLayers;

    private readonly List<FieldOfView> fieldsOfView = new List<FieldOfView>();

    public static FieldOfViewManager Instance { get; private set; }
    public List<FieldOfView> FieldsOfView => fieldsOfView;
    public LayerMask ObstacleLayers => obstacleLayers;

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
