using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Bear : MonoBehaviour, IDamaged
{
    public enum EnemyState
    {
        Idle,
        RandomPatrol,
        Trace,
        Attack,
        Damaged,
        Die
    }
    public GameObject Player;
    private NavMeshAgent _agent;
    private Animator _animator;
    
    [Header("범위")]
    public float FindDistance = 7f;   // 탐색 범위
    public float AttackDistance = 2f; // 공격 범위
    public float PartolPositionDistance = 1.2f;
    private Vector3 _lastPosition;
    private Vector3 _startPosition;

    [Header("공격")]
    public float AttackCooltime = 1.5f;
    private float _attackCooltimer = 0f;
    public int AttackDamageValue = 10;
    
    [Header("스텟")]
    public float MoveSpeed = 3.3f;
    public float MaxHealth = 100;
    public float _currentHealth;
    
    [Header("시간")]
    public float DamagedTime = 0.5f;
    public float DieTime = 2f;
    public float IdleTime = 5f;

    [Header("랜덤순찰")]
    private Coroutine _idleCoroutine;
    private Vector3 _randomTarget;
    private bool _hasRandomTarget = false;
    public float RandomPartolDistance = 7f;
    
    private void Start()
    {
        _currentHealth = MaxHealth;
        // UpdateHealthBar();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;
        
        _animator = _animator = GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
        
        _startPosition = transform.position;
        _lastPosition = _startPosition;
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    // 2. 현재 상태를 지정한다
    public EnemyState CurrentState = EnemyState.Idle;

    private void Update()
    {
        // 나의 현재 상태에 따라 상태 함수를 호출한다.
        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                Idle();
                break;
            }
            case EnemyState.RandomPatrol:
            {
                RandomPatrol();
                break;
            }
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
        }
    }

    public void Damaged(float damage, int actorNumber)
    {
        // 사망했거나 공격받고 있는 중이면
        if(CurrentState == EnemyState.Damaged || CurrentState == EnemyState.Die)
        {
            return;
        }
        
        _animator.SetTrigger("Hit");
        _currentHealth -= damage;
        // UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            Debug.Log($"상태 전환 : {CurrentState} -> Die");
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
            return;
        }
        CurrentState = EnemyState.Damaged;

        StartCoroutine(Damaged_Coroutine());
    }

    // private void UpdateHealthBar()
    // {
    //     HealthGauge.fillAmount = _currentHealth / MaxHealth;
    // }

    // 3. 상태 함수들을 구현한다
    private void Idle()
    {
        // 플레이어 감지를 먼저 체크
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance)
        {
            if (_idleCoroutine != null)
            {
                StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
            }
            Debug.Log("Idle -> Trace");
            CurrentState = EnemyState.Trace;
            _animator.SetTrigger("IdleToTrace");
            return;
        }
        
        // 플레이어가 없을 때만 순찰 코루틴 시작
        if (_idleCoroutine == null)
        {
            _idleCoroutine = StartCoroutine(IdleWaitAndPatrol());
        }
    }

    private IEnumerator IdleWaitAndPatrol()
    {
        yield return new WaitForSeconds(IdleTime);
        _idleCoroutine = null;
        Debug.Log("Idle -> RandomPatrol");
        CurrentState = EnemyState.RandomPatrol;
        _animator.SetTrigger("IdleToWalk");
    }
    

    private void RandomPatrol()
    {
        // 플레이어 감지되면 Trace 상태로 전환
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance)
        {
            Debug.Log("RandomPatrol -> Trace");
            _hasRandomTarget = false;
            CurrentState = EnemyState.Trace;
            _animator.SetTrigger("WalkToTrace");
            return;
        }

        // 아직 랜덤 타겟이 없다면 하나 생성
        if (!_hasRandomTarget)
        {
            _randomTarget = SetRandomPointAround(_lastPosition, RandomPartolDistance);
            _hasRandomTarget = true;
        }

        _agent.SetDestination(_randomTarget);

        // 도착했으면 -> Idle
        if (Vector3.Distance(transform.position, _randomTarget) <= PartolPositionDistance)
        {
            Debug.Log("RandomPatrol -> Idle");
            _animator.SetTrigger("WalkToIdle");
            _hasRandomTarget = false;
            _lastPosition = _randomTarget;
            CurrentState = EnemyState.Idle;
        }
    }

    private Vector3 SetRandomPointAround(Vector3 center, float range)
    {
        while (true)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-range, range),
                0f,
                Random.Range(-range, range)
            ) + center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
    }

    private void Trace()
    {
        // 전이 : 플레이어와 멀어지거나 복귀 지점과 멀어지면 -> Idle
        if (Vector3.Distance(transform.position, Player.transform.position) >= FindDistance)
        {
            Debug.Log("Trace -> Idle");
            _animator.SetTrigger("TraceToWalk");
            CurrentState = EnemyState.Idle;
            return;
        }
        
        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, Player.transform.position) < AttackDistance)
        {
            Debug.Log("Trace -> Attack");
            _animator.SetTrigger("MoveToAttackDelay");
            CurrentState = EnemyState.Attack;
            return;
        }

        _agent.SetDestination(Player.transform.position);
    }
    
    public void Attack()
    {
        // 전이 : 공격 범위보다 멀어지면 -> Trace
        if (Vector3.Distance(transform.position, Player.transform.position) >= AttackDistance)
        {
            Debug.Log("Attack -> Trace");
            _animator.SetTrigger("AttackDelayToMove");
            CurrentState = EnemyState.Trace;
            _attackCooltimer = 0f;
            return;
        }
        
        // 공격한다
        _attackCooltimer += Time.deltaTime;
        if (_attackCooltimer >= AttackCooltime)
        {
            _attackCooltimer = 0f;
            _animator.SetTrigger($"AttackDelayToAttack{Random.Range(1, 3)}");
        }
    }

    private IEnumerator Damaged_Coroutine()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Hit");
        yield return new WaitForSeconds(DamagedTime);
        Debug.Log("Damaged -> Trace");
        _animator.SetTrigger("AttackDelayToMove");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Dead");
        yield return new WaitForSeconds(DieTime);
        Debug.Log("꾸엉");
        // _poolItem.ReturnToPoolAs<Enemy>();
    }
}
