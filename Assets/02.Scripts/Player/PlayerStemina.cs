using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStemina : PlayerAbility
{
    [Header("Stamina Costs")]
    public float DashStaminaCost = 10f;
    public float JumpStaminaCost = 10f;
    public float AttackStaminaCost = 20f;
    public float MinDashStamina = 1f; // 대시 최소 필요 스태미나
    
    [Header("Fatigue System")]
    public float FatigueDuration = 2f;
    
    [Header("Debug Info")]
    private bool _isFatigued = false;
    
    [SerializeField] private float _currentStamina = 0f;
    private float _fatigueTimer = 0f;
    public bool IsUsingStamina = false; // 스태미나 사용 중인지 체크
    
    private void Start()
    {
        _currentStamina = _owner.Stat.MaxStamina;
    }

    private void Update()
    {
        // 탈진 상태 처리
        if (_isFatigued)
        {
            _fatigueTimer -= Time.deltaTime;

            if (_fatigueTimer <= 0f)
            {
                _isFatigued = false;
                Debug.Log("탈진 상태 해제");
            }
            return;
        }
        
        // 스태미나 사용 중이 아닐 때만 회복
        if (!IsUsingStamina)
        {
            RecoverStamina(_owner.Stat.RecoverStaminaAmount * Time.deltaTime);
        }
    }

    public void ConsumeStamina(float amount)
    {
        _currentStamina -= amount;
        _currentStamina = Mathf.Max(_currentStamina, -1f);
    }

    public bool UseDashStamina()
    {
        if (_isFatigued || _currentStamina <= MinDashStamina)
        {
            CheckFatigue();
            return false;
        }

        float cost = DashStaminaCost * Time.deltaTime;
        IsUsingStamina = true;
        ConsumeStamina(cost);
        return true;
    }

    public bool UseJumpStamina()
    {
        if (_isFatigued || _currentStamina < JumpStaminaCost)
        {
            CheckFatigue();
            return false;
        }
        
        IsUsingStamina = true;
        ConsumeStamina(JumpStaminaCost);
        return true;
    }

    public bool UseAttackStamina()
    {
        if (_isFatigued || _currentStamina < AttackStaminaCost)
        {
            CheckFatigue();
            return false;
        }
        
        IsUsingStamina = true;
        ConsumeStamina(AttackStaminaCost);
        return true;
    }

    private void RecoverStamina(float amount)
    {
        if (!_isFatigued)
        {
            _currentStamina = Mathf.Min(_currentStamina + amount, _owner.Stat.MaxStamina);
        }
    }

    private void CheckFatigue()
    {
        if (!_isFatigued)
        {
            _isFatigued = true;
            _fatigueTimer = FatigueDuration;
            Debug.Log("탈진 상태 진입");
        }
    }

    // 외부에서 확인용 프로퍼티
    public float CurrentStamina => _currentStamina;
    public float StaminaPercentage => _currentStamina / _owner.Stat.MaxStamina;
}