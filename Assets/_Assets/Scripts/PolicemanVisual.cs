using System;
using UnityEngine;

public class PolicemanVisual : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_CATCHING = "IsCatching";


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
        _animator.SetBool(IS_WALKING, false);
        _policemanController.OnMovement.AddListener(PolicemanController_OnMovement);
        _policeUnitSM.OnCatchAttempt.AddListener(PoliceUnitSM_OnCatchAttempt);
    }

    private void PoliceUnitSM_OnCatchAttempt()
    {
        _animator.SetTrigger(IS_CATCHING);
    }

    private void Update()
    {
        if(_targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
            float turnSpeed = 4f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    } 

    private void PolicemanController_OnMovement(Vector3 direction)
    {      
        if(direction != Vector3.zero)
        {
            _animator.SetBool(IS_WALKING, true);
            //skin has a 180° rotation to forward vector
            _targetDirection = -direction;
        }
        else
        {
            _animator.SetBool(IS_WALKING, false);
            //skin has a 180° rotation to forward vector
            _targetDirection = -(PlayerController.Instance.transform.position - transform.position);
        }
    }
}
