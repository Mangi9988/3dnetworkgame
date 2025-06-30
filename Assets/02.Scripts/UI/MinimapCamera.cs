using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;    // 변경되지 않음
    public float YDistance = 20f;   // 변경되지 않음
    private Vector3 _initialEulerAngles;

// 이벤트 함수
    private void Start()
    {
        _initialEulerAngles = transform.eulerAngles;
    }

// 이벤트 함수
    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 targetPosition = Target.position;
        targetPosition.y += YDistance;

        transform.position = targetPosition;

        Vector3 targetEulerAngles = Target.eulerAngles;
        targetEulerAngles.x = _initialEulerAngles.x;
        targetEulerAngles.z = _initialEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }

}
