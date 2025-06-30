using Photon.Pun;
using UnityEngine;

public class PlayerHealth : PlayerAbility
{
    [SerializeField] private Animator _animator;
    private bool _isDead;

    
    public void TakeDamage(float damage)
    {
        if (_isDead)
        {
            return;
        }
        
        _owner.Stat.Health = Mathf.Max(0, _owner.Stat.Health - damage);
        // 피격 이팩트 소환!
        if (_photonView.IsMine)
        {
            _owner.GetAbility<PlayerHealthBar>().Refresh();
        }
        
        if (_owner.Stat.Health <= 0)
        {
            Die();
        }
        else
        {
            _photonView.RPC(nameof(PlayDamagedAnimation), RpcTarget.All);
        }
    }
    
    private void Die()
    {
        if (_photonView.IsMine)
        {
            _isDead = true;
            _photonView.RPC(nameof(PlayDeadAnimation), RpcTarget.All);

            InputHandler.BlockInput = true;
        }
    }

    private void PlayDamagedAnimation()
    {
        _animator.SetTrigger("Damaged");
    }

    private void PlayDeadAnimation()
    {
        _animator.SetTrigger("Dead");
    }
}
