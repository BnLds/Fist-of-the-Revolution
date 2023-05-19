using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

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
    }

    private void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        float turnSpeed = 4f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
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
