using UnityEngine;

public class PolicemanController : MonoBehaviour
{
    //[SerializeField] private PolicemanAI _policemanAI;
    [SerializeField] private PoliceUnitSM _policeUnitSM;
    [SerializeField] private ProtesterAI _protesterAI;
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
        //_policemanAI.OnMoveDirectionInput.AddListener(policemanAI_OnMoveDirectionInput);
        _protesterAI.OnMoveDirectionInput.AddListener(protesterAI_OnMoveDirectionInput);
    }
    

    private void FixedUpdate()
    {
        if(_policeUnitSM.CurrentState == _policeUnitSM.FollowProtestState && _policeUnitSM.FollowProtestState.IsFollowingProtest)
        {
            //follow the protest path
            _policemanRB.velocity = _moveDirectionFollowProtest * _moveSpeed; 
        }
        else
        {

            _policemanRB.velocity =  _policeUnitSM.MoveDirectionInput * _moveSpeed;
        }
    }

    private void protesterAI_OnMoveDirectionInput(Vector3 direction)
    {
        _moveDirectionFollowProtest = direction;
    }

    /*private void policemanAI_OnMoveDirectionInput(Vector3 direction)
    {
        _moveDirection = direction;
    */
}
