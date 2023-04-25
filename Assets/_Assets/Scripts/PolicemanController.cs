using UnityEngine;

public class PolicemanController : MonoBehaviour
{
    [SerializeField] private PolicemanAI policemanAI;
    [SerializeField] private float moveSpeed = 3f;
    
    private Vector3 moveDirection;
    private Rigidbody policemanRB;

    private void Awake()
    {
        moveDirection = Vector3.zero;
        policemanRB = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        policemanAI.OnMoveDirectionInput.AddListener(policemanAI_OnMoveDirectionInput);
    }

    private void FixedUpdate()
    {
        policemanRB.velocity = moveDirection * moveSpeed;
    }

    private void policemanAI_OnMoveDirectionInput(Vector3 direction)
    {
        moveDirection = direction;
    }
}
