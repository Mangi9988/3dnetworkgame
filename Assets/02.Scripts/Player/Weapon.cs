using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerAttack _playerAttack;

    private void Start()
    {
        _playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playerAttack == null)
        {
            return;
        }
        
        // 자기 자신과 부딪혔다면 어무것도 안함
        if (other.transform == _playerAttack.transform)
        {
            return;
        }
        
        // IDamaged 인터페이스를 구현하고 있는지 확인
        if (other.GetComponent<IDamaged>() != null)
        {
            _playerAttack.Hit(other);
        }
    }
}
