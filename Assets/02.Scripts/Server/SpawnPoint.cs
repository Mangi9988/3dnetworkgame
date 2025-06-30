using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static SpawnPoint _instance;
    public static SpawnPoint Instance => _instance;
    
    [SerializeField] private List<Transform> _spawnPoints;

    
    
    private void Awake()
    {
        _instance = this;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
    }
}
