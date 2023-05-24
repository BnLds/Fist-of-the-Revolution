using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_LOADING_ATTACK = "IsLoadingAttack";
    private const string LOAD_ATTACK_SPEED = "LoadAttackSpeed";
    private const string PERFORM_ATTACK = "PerformAttack";

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private List<SkinSO> _skinSOs;
    [SerializeField] private List<MeshRenderer> _coloredClothes;

    private SkinSO _currentSkin;
    private Vector3 _targetDirection;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _currentSkin = _skinSOs[0];
        foreach(MeshRenderer meshRenderer in _coloredClothes)
        {
            meshRenderer.material = _currentSkin.skinMaterial;
        }
    }

    private void Start()
    {
        _playerController.OnMove.AddListener(PlayerController_OnMove);
        _playerController.OnStartedLoadingAttack.AddListener(PlayerController_OnStartedLoadingAttack);
        _playerController.OnStoppedLoadingAttack.AddListener(PlayerController_OnStoppedLoadingAttack);
        _playerController.OnAttackPerformed.AddListener(PlayerController_OnAttackPerformed);
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

    private void PlayerController_OnStartedLoadingAttack()
    {
        _animator.SetFloat(LOAD_ATTACK_SPEED, (float)1/_playerController.GetAttackLoadTime());
        _animator.SetBool(IS_LOADING_ATTACK, true);
    }

    private void PlayerController_OnStoppedLoadingAttack()
    {
        _animator.SetBool(IS_LOADING_ATTACK, false);
    }

    private void PlayerController_OnAttackPerformed(Transform t)
    {
        _animator.SetTrigger(PERFORM_ATTACK);
    }

    private void PlayerController_OnMove(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            //character visual has a 180° rotation to forward vector
            _targetDirection = -direction;
            _animator.SetBool(IS_WALKING, true);
        }
        else
        {
            _animator.SetBool(IS_WALKING, false);
        }
    }

    public SkinSO GetSkinSO()
    {
        return _currentSkin;
    }
}
