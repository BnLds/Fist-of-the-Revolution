using UnityEngine;
using UnityEngine.Events;


public class PolicemanController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector3> OnMovement;

    [Header("Initialization Parameters")]
    [SerializeField] private PoliceUnitSM _policeUnitSM;
    [SerializeField] private ProtesterAI _protesterAI;
    [SerializeField] private LayerMask _avoidCollisionMask;
    [SerializeField] private Collider _policemanCollider;

    [Header("Game Balance Parameters")]
    [SerializeField] private float _moveSpeed = 3f;
    
    private Vector3 _moveDirectionFollowProtest;
    private Rigidbody _policemanRB;


    private void Awake()
    {
        _moveDirectionFollowProtest = Vector3.zero;
        _policemanRB = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _protesterAI.OnMoveDirectionInput.AddListener(protesterAI_OnMoveDirectionInput);
    }
    

    private void FixedUpdate()
    {
        if(_policeUnitSM.CurrentState == _policeUnitSM.FollowProtestState)
        {
            //if the policeman is in the FollowProtest state, apply protester momevement logic
            if(_policeUnitSM.FollowProtestState.IsFollowingProtest)
            {
                //follow the protest path
                _policemanRB.velocity = _moveDirectionFollowProtest * _moveSpeed;
                OnMovement?.Invoke(_moveDirectionFollowProtest);
            }
            else
            {
                //await 
                _policemanRB.velocity = Vector3.zero;
            }
            
        }
        else
        {
            //apply policeman move logic
            _policemanRB.velocity =  _policeUnitSM.MoveDirectionInput * _moveSpeed;
            OnMovement?.Invoke(_policeUnitSM.MoveDirectionInput);

        }
    }

    private void protesterAI_OnMoveDirectionInput(Vector3 direction)
    {
        _moveDirectionFollowProtest = direction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(1 << collision.collider.gameObject.layer == _avoidCollisionMask.value)
        {
            Physics.IgnoreCollision(collision.collider, _policemanCollider);
        }
    }
}
