using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private DamageableObject player;
    [SerializeField]
    private LayerMask obstacleLayers;
    [SerializeField]
    private LayerMask enemyLayers;

    public static GameManager Instance { get; private set; }
    public DamageableObject Player => player;
    public LayerMask ObstacleLayers => obstacleLayers;
    public LayerMask EnemyLayers => enemyLayers;

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
