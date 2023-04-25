using UnityEngine;

public class ProtesterController : MonoBehaviour
{
    [SerializeField] private ProtesterAI protesterAI;
    [SerializeField] private float moveSpeed = 5f;
    
    private Vector3 moveDirection;
    private Rigidbody protesterRB;

    private void Awake()
    {
        moveDirection = Vector3.zero;
        protesterRB = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        protesterAI.OnMoveDirectionInput.AddListener(protesterAI_OnMoveDirectionInput);
        protesterAI.OnProtestEndReached.AddListener(protesterAI_OnProtestEndReached);
    }

    private void protesterAI_OnProtestEndReached()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        protesterRB.velocity = moveDirection * moveSpeed;
    }

    private void protesterAI_OnMoveDirectionInput(Vector3 direction)
    {
        moveDirection = direction;
    }
}
