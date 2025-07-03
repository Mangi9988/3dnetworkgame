using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EPlayerState
{
    Live,
    Death,
}

[RequireComponent(typeof(PlayerMove))]
public class Player : MonoBehaviour, IDamaged
{
    public PlayerStat Stat;

    public int Score = 0;

    private Dictionary<Type, PlayerAbility> _abilitiesCache;
    
    private EPlayerState _state = EPlayerState.Live;
    public EPlayerState State => _state;
  
    private PhotonView _photonView;
    private Animator _animator;
    private CharacterController _characterController;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _abilitiesCache = new Dictionary<Type, PlayerAbility>();
    }

    [PunRPC]
    public void Damaged(float damage, int actorNumber)
    {
        if (_state == EPlayerState.Death) return;

        Stat.Health = Mathf.Max(0, Stat.Health - damage);

        if (Stat.Health <= 0)
        {
            _state = EPlayerState.Death;
          
            // 사망 애니메이션 실행
            _animator.SetTrigger("Death");
          
            // 5초 동안(못 움직이고 , 못 맞고, 못 때린다.)
            // 5초 후에 체력과 스태미너 회복된 상태에서 랜덤한 위치에 리스폰
            StartCoroutine(Death_Coroutine());
            
            RoomManager.Instance.OnPlayerDeath(_photonView.Owner.ActorNumber, actorNumber);

            if (_photonView.IsMine)
            {
                MakeItems(Random.Range(1, 3));
            }
        }
        else
        {
            // RPC로 호출 X
            GetAbility<PlayerShakingAbility>().Shake();
        }
    }

    private void MakeItems(int count)
    {
        MakePotion(Random.Range(0, 10));
        
        for (int i = 0; i < count; i++)
        {
            // 포톤의 네트워크 객체의 생명 주기
            // Player : 플레이어가 생성하고, 플레이어가 나가면 자동 삭제 (PhotonNetwork.Instantiate/Destroy)
            // Room   : 룸이 생성하고, 룸이 없어지면 삭제 (PhotonNetwork.InstantiateRoomObject/Destroy)
            // PhotonNetwork.InstantiateRoomObject("ScoreItem", transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            
            ItemObjectFactory.Instance.RequestCreate(EItemType.Score, transform.position);
        }
    }

    private void MakePotion(int randomNumber)
    {
        if (randomNumber <= 2)
        {
            ItemObjectFactory.Instance.RequestCreate(EItemType.Stamina, transform.position);
        }
        else if(randomNumber <= 4)
        {
            ItemObjectFactory.Instance.RequestCreate(EItemType.Health, transform.position);
        }
    }
    
    public void RecoverStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.Log("스테미너 회복량이 0보다 작을 수 없습니다.");
        }
        Stat.Stamina = Mathf.Min(Stat.Stamina + amount, Stat.MaxStamina);
    }

    public void RecoverHealth(float amount)
    {
        if(amount < 0)
        {
            throw new Exception("체력 회복량이 0보다 작을 수 없습니다.");
        }
        Stat.Health = Mathf.Min(Stat.Health + amount, Stat.MaxHealth);
    }
    
    private IEnumerator Death_Coroutine()
    {
        var wait = new WaitForSeconds(5f);
        
        _characterController.enabled = false;

        yield return wait;


        Stat.Health = Stat.MaxHealth;
        Stat.Stamina = Stat.MaxStamina;

        _state = EPlayerState.Live;
        _animator.SetTrigger("Live");        
        
        // 리스폰 코드
        if (_photonView.IsMine)
        {
            var randomSpawnPoint = SpawnPoint.Instance.GetRandomSpawnPoint();
            transform.position = randomSpawnPoint;
        }
        
        _characterController.enabled = true;
    }
    
    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        
        // 게으른 초기화 / 로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라 필요할 때만 하는, 뒤로 미루는 기법
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            
            return ability as T;
        }
        
        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");

        return null;
    }
}
