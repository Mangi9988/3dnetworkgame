using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private EItemType _itemType;
    public EItemType ItemType => _itemType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!player.GetComponent<PhotonView>().IsMine)
            {
                return;
            }
            
            ApplyItemEffect(player);
            
            Debug.Log(other.gameObject.name);
            
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
    
    private void ApplyItemEffect(Player player)
    {
        switch (_itemType)
        {
            case EItemType.Score:
                player.Score += 10;
                break;

            case EItemType.Health:
                player.RecoverHealth(20f);
                break;

            case EItemType.Stamina:
                player.RecoverStamina(15f);
                break;

            default:
                Debug.LogWarning($"Unknown item type: {_itemType}");
                break;
        }
    }
}
