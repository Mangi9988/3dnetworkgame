using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreItemSpawner : MonoBehaviour
{
    public float Interval;        // 몇 초마다 생성할 것인지
    private float _intervalTimer;
    public float Range;                 // 랜덤한 범위

    private void Start()
    {
        Interval = Random.Range(10f, 20f);
        Range = Random.Range(5f, 15f);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        _intervalTimer += Time.deltaTime;

        if (_intervalTimer >= Interval)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * Range;
            randomPosition.y = 3f;
            
            ItemObjectFactory.Instance.RequestCreate(EItemType.Score, randomPosition);
            
            _intervalTimer = 0f;
        }
    }
}