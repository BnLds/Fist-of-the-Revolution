using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private LayerMask _breakableMask;
    [SerializeField] private LayerMask _avoidCollisionMask;

    [HideInInspector] public UnityEvent<Transform> OnDamageDone;

    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;
    [SerializeField] private int _playerDamage = 1;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _playerRigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        GameInput.Instance.OnInteract.AddListener(PerformAttack);
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(GameInput.Instance.GetMovementVectorNormalized().x, 0,GameInput.Instance.GetMovementVectorNormalized().y);

        _playerRigidbody.velocity = moveInput * _moveSpeed;
    }

    private void PerformAttack()
    {
        float detectionRadius = 2f;
        Collider[] breakableColliders = Physics.OverlapSphere(transform.position, detectionRadius, _breakableMask);
        foreach(Collider collider in breakableColliders)
        {
            collider.GetComponent<BreakableController>().Damage(_playerDamage);
            OnDamageDone?.Invoke(transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(1 << collision.collider.gameObject.layer == _avoidCollisionMask.value)
        {
            Physics.IgnoreCollision(collision.collider, _playerCollider);
        }
    }
}
