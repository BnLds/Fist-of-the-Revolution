using UnityEngine;

public class ProtesterController : MonoBehaviour
{
    [SerializeField] private ProtesterAI _protesterAI;
    [SerializeField] private float _moveSpeed = 3f;
    
    private Vector3 _moveDirection;
    private Rigidbody _protesterRB;

    private void Awake()
    {
        _moveDirection = Vector3.zero;
        _protesterRB = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _protesterAI.OnMoveDirectionInput.AddListener(protesterAI_OnMoveDirectionInput);
        _protesterAI.OnProtestEndReached.AddListener(protesterAI_OnProtestEndReached);
    }

    private void protesterAI_OnProtestEndReached()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        _protesterRB.velocity = _moveDirection * _moveSpeed;
    }

    private void protesterAI_OnMoveDirectionInput(Vector3 direction)
    {
        _moveDirection = direction;
    }
}
