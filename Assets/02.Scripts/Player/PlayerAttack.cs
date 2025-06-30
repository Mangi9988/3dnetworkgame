using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : PlayerAbility
{
    [SerializeField] private Animator _animator;
    private bool _isAttacking = false;  // 공격 중 여부 플래그
    private float _attackTimer = 0f;
    
    // 위치/회전 처럼 상시로 확인이 필요한 데이터 동깋화 : IPunObservable(OnPhotonSerializeView)
    // 트리거/공격/피격 처럼 간헐적으로 특정한 이벤트가 발생했을 때의 변화된 데이터 동기화 : RPC
    // RPC : Remote Procedure Call
    //          ㄴ 물리적으로 떨어져 있는 다른 디바이스의 함수를 호출하는 기능
    //          ㄴ RPC 함수를 호출하면 네트워크를 통해 다른 사용자의 스크립트에서 해당 함수가 호출된다.
    
    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        
        // 문제
        // Ability에서 다른 Ability에 접근하는 효율적(편하고 좋은거)인 방법
        // ex) PlayerMoveAbillty의 IsMovee 속성에 따라 공격 여부를 정하고 싶다...
        
        _attackTimer += Time.deltaTime;
        
        float attackInterval = 1f / _owner.Stat.AttackSpeed;

        if (_isAttacking && _attackTimer >= attackInterval)
        {
            _isAttacking = false;
        }
        
        // 마우스 클릭 && 공격 쿨타임이 지났으며, 공격중이 아닐 때에만 공격
        if (InputHandler.GetMouseButtonDown(0) && !_isAttacking)
        {
            if (!_owner.GetAbility<PlayerStemina>().UseAttackStamina())
            {
                return;
            }
            
            // 1. 일반 메서드 호출 방식
            // PlayAttackAnimation(Random.Range(1, 4));
            
            // 2. RPc 메서드 호출 방식
            _photonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1, 4));
            
            _isAttacking = true;  // 공격 시작 시 플래그 세팅
        }
    }

    // 공격 애니메이션 이벤트로 호출하는 함수 (애니메이션 클립 마지막에 이벤트 등록 필요)
    public void OnAttackEnd()
    {
        //_isAttacking = false;
    }
    
    // RPC로 호출할 함수는 반드시 [PunRPC]를 호출해줘야 한다.
    [PunRPC]
    private void PlayAttackAnimation(int randomNumber)
    {
        _animator.SetTrigger($"Attack{randomNumber}");
    }
}
