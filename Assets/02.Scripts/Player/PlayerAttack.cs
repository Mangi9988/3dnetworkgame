using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _attackCooldown = 0.8f;
    private float _lastAttackTime = Mathf.NegativeInfinity;
    private bool _isAttacking = false;  // 공격 중 여부 플래그
    
    private readonly string[] _attackTriggers = { "Attack1", "Attack2", "Attack3" };

    private void Update()
    {
        IsAttackCooldownOver();    
        // 마우스 클릭 && 공격 쿨타임이 지났으며, 공격중이 아닐 때에만 공격
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            TriggerRandomAttack();
            _lastAttackTime = Time.time;
            _isAttacking = true;  // 공격 시작 시 플래그 세팅
        }
    }
    
    private bool CanAttack()
    {
        // 아직 공격중이 아니고, 쿨타임이 지났는지 체크
        return !_isAttacking && Time.time >= _lastAttackTime + _attackCooldown;
    }

    private void IsAttackCooldownOver()
    {
        if (_isAttacking && Time.time >= _lastAttackTime + _attackCooldown)
        {
            _isAttacking = false;
        }
    }

    private void TriggerRandomAttack()
    {
        int index = Random.Range(0, _attackTriggers.Length);
        string selectedTrigger = _attackTriggers[index];
        _animator.SetTrigger(selectedTrigger);
    }

    // 공격 애니메이션 이벤트로 호출하는 함수 (애니메이션 클립 마지막에 이벤트 등록 필요)
    public void OnAttackEnd()
    {
        _isAttacking = false;
    }

}
