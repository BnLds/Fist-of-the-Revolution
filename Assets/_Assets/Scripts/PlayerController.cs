using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Initialization Parameters")]
    [SerializeField] private LayerMask _breakableMask;
    [SerializeField] private LayerMask _avoidCollisionMask;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackLoadingTotalTime = .5f;
    [SerializeField] private int _playerDamage = 1;
    [SerializeField] private float _attackRadius = 2f;

    [HideInInspector] public UnityEvent<Transform> OnAttackPerformed;
    [HideInInspector] public UnityEvent<Vector3> OnMove;
    [HideInInspector] public UnityEvent<float> OnAttackProgressChange;
    [HideInInspector] public UnityEvent OnStartedLoadingAttack;
    [HideInInspector] public UnityEvent OnStoppedLoadingAttack;



    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;
    private float _attackLoadingProgress;
    private bool _isLoadingAttack;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _playerRigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<CapsuleCollider>();
        _isLoadingAttack = false;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractBegin.AddListener(StartLoadingAttack);
        GameInput.Instance.OnInteractEnd.AddListener(StopLoadingAttack);
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(GameInput.Instance.GetMovementVectorNormalized().x, 0,GameInput.Instance.GetMovementVectorNormalized().y);
        OnMove?.Invoke(moveInput);
        _playerRigidbody.velocity = moveInput * _moveSpeed;
    }

    private void Update()
    {
        if(IsBreakableInAttackDistance())
        {
            GuidanceUI.Instance.ShowGuidanceAttack();
        }

        if(_isLoadingAttack)
        {
            _attackLoadingProgress += Time.deltaTime;
            float progressNormalized = (float)_attackLoadingProgress / (float)_attackLoadingTotalTime;
            OnAttackProgressChange?.Invoke(progressNormalized);

            if(_attackLoadingProgress >= _attackLoadingTotalTime)
            {
                Collider[] breakableColliders = Physics.OverlapSphere(transform.position, _attackRadius, _breakableMask);
                foreach(Collider collider in breakableColliders)
                {
                    collider.GetComponent<BreakableController>().Damage(_playerDamage);
                    OnAttackPerformed?.Invoke(transform);
                    StopLoadingAttack();
                }
            }
        }
    }

    private void StartLoadingAttack()
    {
        OnStartedLoadingAttack?.Invoke();
        _attackLoadingProgress = 0f;
        _isLoadingAttack = true;
    }

    private void StopLoadingAttack()
    {
        OnStoppedLoadingAttack?.Invoke();
        _isLoadingAttack = false;
        _attackLoadingProgress = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(1 << collision.collider.gameObject.layer == _avoidCollisionMask.value)
        {
            Physics.IgnoreCollision(collision.collider, _playerCollider);
        }
    }

    private float GetAttackRadius()
    {
        return _attackRadius;
    }

    private bool IsBreakableInAttackDistance()
    {
        return Physics.CheckSphere(transform.position, _attackRadius, _breakableMask);
    }
}
