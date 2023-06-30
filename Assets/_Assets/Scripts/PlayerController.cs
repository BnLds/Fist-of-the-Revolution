using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Initialization Parameters")]
    [SerializeField] private LayerMask _breakableMask;
    [SerializeField] private LayerMask _avoidCollisionMask;
    [SerializeField] private LayerMask _equipmentLayer;
    [SerializeField] private List<Transform> _itemsCasserolade;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackLoadingTotalTime = .5f;
    [SerializeField] private int _playerDamage = 1;
    [SerializeField] private float _attackRadius = 2f;
    [SerializeField] private float _timeToLoseID = 2f;
    [SerializeField] private float _timeToUntrack = 4f;

    [HideInInspector] public UnityEvent<Transform> OnAttackPerformed;
    [HideInInspector] public UnityEvent<Vector3> OnMove;
    [HideInInspector] public UnityEvent<float> OnAttackProgressChange;
    [HideInInspector] public UnityEvent OnStartedLoadingAttack;
    [HideInInspector] public UnityEvent OnStoppedLoadingAttack;
    [HideInInspector] public UnityEvent OnHideTimesChange;
    [HideInInspector] public UnityEvent OnStartedCasserolade;
    [HideInInspector] public UnityEvent OnStoppedCasserolade;

    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;
    private float _attackLoadingProgress;
    private bool _isLoadingAttack;
    private bool _isPerformingCasserolade;
    private EquipmentManager _equipmentManager;

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
        _equipmentManager = GetComponent<EquipmentManager>();
        _isLoadingAttack = false;
        _isPerformingCasserolade = false;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractBegin.AddListener(StartLoadingAttack);
        GameInput.Instance.OnInteractEnd.AddListener(StopLoadingAttack);
        GameInput.Instance.OnCasseroladeBegin.AddListener(PerformCasserolade);
        GameInput.Instance.OnCasseroladeEnd.AddListener(EndCasserolade);
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

        if (_isLoadingAttack && !_isPerformingCasserolade)
        {
            LoadAttack();
        }

    }

    private void LoadAttack()
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

    private void PerformCasserolade()
    {
        foreach(Transform item in _itemsCasserolade)
        {
            if(item.gameObject.activeSelf == false) return;
        }

        _isPerformingCasserolade = true;
        StopLoadingAttack();
        OnStartedCasserolade?.Invoke();
    }

    private void EndCasserolade()
    {
        _isPerformingCasserolade = false;
        OnStoppedCasserolade?.Invoke();
    }

    private void StartLoadingAttack()
    {
        if(!_isPerformingCasserolade)
        {
            OnStartedLoadingAttack?.Invoke();
            _attackLoadingProgress = 0f;
            _isLoadingAttack = true;
        }
    }

    private void StopLoadingAttack()
    {
        OnStoppedLoadingAttack?.Invoke();
        _isLoadingAttack = false;
        _attackLoadingProgress = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //check if collision with protester
        if(1 << collision.collider.gameObject.layer == _avoidCollisionMask.value)
        {
            Physics.IgnoreCollision(collision.collider, _playerCollider);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(1<< collider.gameObject.layer == _equipmentLayer.value)
        {
            _equipmentManager.Equip(collider.transform);
            ShowCasseroladeGuidance(collider.GetComponent<EquipmentData>().GetEquipmentSO().EquipmentName);

            Destroy(collider.gameObject);
        }
    }

    private void ShowCasseroladeGuidance(string equipmentName)
    {
        bool canCasserolade = false;
        foreach (Transform item in _itemsCasserolade)
        {
            if (item.gameObject.activeSelf)
            {
                canCasserolade = true;
            }
            else
            {
                canCasserolade = false;
                return;
            }
        }

        if(canCasserolade) GuidanceUI.Instance.ShowGuidanceCasserolade();
    }

    public float GetLoseIDTime()
    {
        return _timeToLoseID;
    }

    public float GetAttackLoadTime()
    {
        return _attackLoadingTotalTime;
    }

    public float GetUntrackTime()
    {
        return _timeToUntrack;
    }

    private bool IsBreakableInAttackDistance()
    {
        return Physics.CheckSphere(transform.position, _attackRadius, _breakableMask);
    }

    public void IncreaseFlatAttackDamage(int bonusDamage)
    {
        _playerDamage += bonusDamage;
    }

    public void IncreaseMoveSpeed(float percentMoveSpeed)
    {
        _moveSpeed *= 1 + percentMoveSpeed;
    }

    public void IncreaseAttackRange(float percentAttackRange)
    {
        _attackRadius *= 1 + percentAttackRange;
    }

    public void IncreaseAttackSpeed(float percentAttackSpeed)
    {
        _attackLoadingTotalTime *= 1 - percentAttackSpeed;
    }

    public void ReduceHideTime(float percentHideTime)
    {
        _timeToLoseID *= 1 - percentHideTime;
        _timeToUntrack *= 1 - percentHideTime;

        OnHideTimesChange?.Invoke();
    }
}
