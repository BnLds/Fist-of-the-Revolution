using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private LayerMask breakableMask;

    public UnityEvent OnDamageBreakable;

    private Rigidbody playerRigidbody;
    [SerializeField] private int playerDamage = 1;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameInput.Instance.OnInteract.AddListener(PerformInteractAction);
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(GameInput.Instance.GetMovementVectorNormalized().x, 0,GameInput.Instance.GetMovementVectorNormalized().y);

        playerRigidbody.velocity = moveInput * moveSpeed;
    }

    private void PerformInteractAction()
    {
        float detectionRadius = 2f;
        Collider[] breakableColliders = Physics.OverlapSphere(transform.position, detectionRadius, breakableMask);
        foreach(Collider collider in breakableColliders)
        {
            collider.GetComponent<BreakableController>().Damage(playerDamage);
        }
    }
}
