using UnityEngine;

public class PolicemanController : MonoBehaviour
{
    //[SerializeField] private PolicemanAI _policemanAI;
    [SerializeField] private PoliceUnitSM _policeUnitSM;
    [SerializeField] private float _moveSpeed = 3f;
    
    private Vector3 _moveDirection;
    private Rigidbody _policemanRB;

    private void Awake()
    {
        _moveDirection = Vector3.zero;
        _policemanRB = GetComponent<Rigidbody>();
    }
    /*private void Start()
    {
        _policemanAI.OnMoveDirectionInput.AddListener(policemanAI_OnMoveDirectionInput);
    }
    */

    private void FixedUpdate()
    {
        //_policemanRB.velocity = _moveDirection * _moveSpeed;
        _policemanRB.velocity =  _policeUnitSM.MoveDirectionInput * _moveSpeed;

    }

    /*private void policemanAI_OnMoveDirectionInput(Vector3 direction)
    {
        _moveDirection = direction;
    */
}
