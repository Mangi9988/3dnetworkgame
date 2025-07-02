using System;
using Photon.Pun;
using UnityEngine;

public enum EItemType
{
    Score,
    Health,
    Stamina
}

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviourPun
{
    [SerializeField] private EItemType itemType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            
            ApplyItemEffect(player);
            
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
    
    private void ApplyItemEffect(Player player)
    {
        switch (itemType)
        {
            case EItemType.Score:
                player.Score += 10;
                break;

            case EItemType.Health:
                player.RecoverHealth(20f); // 예시: 체력 20 회복
                break;

            case EItemType.Stamina:
                player.RecoverStamina(15f); // 예시: 스태미나 15 회복
                break;

            default:
                Debug.LogWarning($"Unknown item type: {itemType}");
                break;
        }
    }
}
