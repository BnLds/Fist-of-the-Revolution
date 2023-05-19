using System;
using UnityEngine;

public class PolicemanVisual : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private PolicemanController _policemanController;
    [SerializeField] private PoliceUnitSM _policeUnitSM;

    private Animator _animator;
    private Vector3 _targetDirection;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool(IS_FLASHING, false);
        _policemanController.OnMovement.AddListener(PolicemanController_OnMovement);
    }

    private void Update()
    {
        if(_policeUnitSM.CurrentState == _policeUnitSM.ChasePlayerState)
        {
            _animator.SetBool(IS_FLASHING, true);
        }
        else
        {
            _animator.SetBool(IS_FLASHING, false);
        }

        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        float turnSpeed = 4f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    } 

    private void PolicemanController_OnMovement(Vector3 direction)
    {
        //skin has a 180° rotation to forward vector
        _targetDirection = -direction;
    }
}
