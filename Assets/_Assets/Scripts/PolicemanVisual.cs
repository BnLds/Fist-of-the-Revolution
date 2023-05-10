using UnityEngine;

public class PolicemanVisual : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private PoliceUnitSM _policeUnitSM;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool(IS_FLASHING, false);

    }

    private void Update()
    {
        if(_policeUnitSM.CurrentState == _policeUnitSM.FollowSuspectState)
        {
            _animator.SetBool(IS_FLASHING, true);
        }
        else
        {
            _animator.SetBool(IS_FLASHING, false);
        }
    } 
}
