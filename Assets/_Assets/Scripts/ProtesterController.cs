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
        protesterAI.OnMoveDirectionInput.AddListener(npcai_OnMoveDirectionInput);
        protesterAI.OnProtestEndReached.AddListener(npcAI_OnProtestEndReached);
    }

    private void npcAI_OnProtestEndReached()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        protesterRB.velocity = moveDirection * moveSpeed;
    }

    private void npcai_OnMoveDirectionInput(Vector3 direction)
    {
        moveDirection = direction;
    }
}
