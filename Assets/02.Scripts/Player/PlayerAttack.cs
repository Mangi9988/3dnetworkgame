using UnityEngine;

public class PlayerAttack : PlayerAbility
{
    [SerializeField] private Animator _animator;
    private bool _isAttacking = false;  // 공격 중 여부 플래그
    private float _attackTimer = 0f;
    
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
        if (InputHandler.GetMouseButtonDown(0) && _attackTimer >= attackInterval)
        {
            _animator.SetTrigger($"Attack{Random.Range(1, 3)}");
            
            _isAttacking = true;  // 공격 시작 시 플래그 세팅
        }
    }

    // 공격 애니메이션 이벤트로 호출하는 함수 (애니메이션 클립 마지막에 이벤트 등록 필요)
    public void OnAttackEnd()
    {
        _isAttacking = false;
    }

}
