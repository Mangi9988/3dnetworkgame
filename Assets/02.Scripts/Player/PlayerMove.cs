using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    private Camera _mainCamera;
    private Transform _cameraTarget;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float SmoothSpeedUp = 15f;
    [SerializeField] private float SmoothSpeedDown = 5f;
    private float _smoothX = 0f;
    private float _smoothZ = 0f;

    [Header("Jump")]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.5f;
    public float FallTimeout = 0.15f;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private bool _wasGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _wasGrounded = _characterController.isGrounded;
    }

    private void Update()
    {
        JumpAndGravity();
        HandlePlayerMove();
    }

    private void HandlePlayerMove()
    {
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawZ = Input.GetAxisRaw("Vertical");

        _smoothX = (rawX > _smoothX)
            ? Mathf.MoveTowards(_smoothX, rawX, Time.deltaTime * SmoothSpeedUp)
            : Mathf.MoveTowards(_smoothX, rawX, Time.deltaTime * SmoothSpeedDown);

        _smoothZ = (rawZ > _smoothZ)
            ? Mathf.MoveTowards(_smoothZ, rawZ, Time.deltaTime * SmoothSpeedUp)
            : Mathf.MoveTowards(_smoothZ, rawZ, Time.deltaTime * SmoothSpeedDown);

        if (_animator != null)
        {
            _animator.SetFloat("MoveX", _smoothX);
            _animator.SetFloat("MoveZ", _smoothZ);
            _animator.SetBool("IsMove", Mathf.Abs(_smoothX) > 0f || Mathf.Abs(_smoothZ) > 0f);
        }

        if (_animator != null)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("Attack") || stateInfo.IsTag("Skill"))
                return;
        }

        float targetSpeed = (rawX == 0f && rawZ == 0f) ? 0f : _moveSpeed;

        Vector3 inputDir = new Vector3(rawX, 0f, rawZ).normalized;
        if (inputDir != Vector3.zero)
        {
            float targetRot = (_cameraTarget != null)
                ? _cameraTarget.eulerAngles.y
                : _mainCamera.transform.eulerAngles.y;
            inputDir = Quaternion.Euler(0f, targetRot, 0f) * inputDir;
        }

        Vector3 move = inputDir * targetSpeed * Time.deltaTime;
        move.y = _verticalVelocity * Time.deltaTime;

        if (_characterController.enabled)
        {
            _characterController.Move(move);
        }

        if (_animator != null)
        {
            Vector3 horizontalMove = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
            float normalizedSpeed = Mathf.Clamp01(horizontalMove.magnitude / (_moveSpeed > 0f ? _moveSpeed : 1f));
            float animatedSpeed = Mathf.Lerp(0.3f, 1.4f, normalizedSpeed);
            _animator.SetFloat("Speed", animatedSpeed);
        }
    }

    private void JumpAndGravity()
    {
        bool isGrounded = _characterController.isGrounded;
        bool justLanded = !_wasGrounded && isGrounded;
        _wasGrounded = isGrounded;

        if (isGrounded)
        {
            if (justLanded)
            {
                float hSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;
                _animator.SetBool("Grounded", true);
                _animator.SetFloat("LandBlend", (hSpeed > 0.1f) ? 1f : 0f);
            }

            _fallTimeoutDelta = FallTimeout;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _jumpTimeoutDelta = JumpTimeout;
                _animator.SetBool("Jump", true);
            }

            if (_jumpTimeoutDelta > 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            _animator.SetBool("Grounded", true);
            _animator.SetBool("FreeFall", false);
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta > 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            _animator.SetBool("Grounded", false);
            _animator.SetBool("Jump", false);
            _animator.SetBool("FreeFall", _verticalVelocity < 0.0f);
        }

        if (!_characterController.isGrounded && _characterController.enabled)
        {
            _characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}
