using System;
using Photon.Pun;
using UnityEngine;

// 아이템 공장 : 아이템 생성을 담당
[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    private static ItemObjectFactory _instance;
    public static ItemObjectFactory Instance => _instance;

    private PhotonView _photonView;
    
    private void Awake()
    {
        _instance = this;
        
        _photonView = GetComponent<PhotonView>();
    }

    public void RequestCreate(EItemType itemType, Vector3 dropPosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Create(itemType, dropPosition + new Vector3(0f, 2f, 0f));
        }
        else
        {
            _photonView.RPC(nameof(Create), RpcTarget.MasterClient, itemType, dropPosition + new Vector3(0f, 2f, 0f));   
        }
    }
    
    [PunRPC]
    private void Create(EItemType itemType, Vector3 dropPosition)
    {
        // 포톤의 네트워크 객체의 생명 주기
        // Player : 플레이어가 생성하고, 플레이어가 나가면 자동 삭제 (PhotonNetwork.Instantiate/Destroy)
        // Room   : 룸이 생성하고, 룸이 없어지면 삭제 (PhotonNetwork.InstantiateRoomObject/Destroy)
            
        PhotonNetwork.InstantiateRoomObject($"{itemType}Item", dropPosition, Quaternion.identity);
    }

    public void RequestDelete(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            _photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }
    
    [PunRPC]
    private void Delete(int viewID)
    {
        GameObject objectToDelete = PhotonView.Find(viewID).gameObject;
        if(objectToDelete == null) return;
        
        PhotonNetwork.Destroy(objectToDelete);
    }
}
