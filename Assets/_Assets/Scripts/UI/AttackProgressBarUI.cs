using System;
using UnityEngine;
using UnityEngine.UI;

public class AttackProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        _playerController.OnAttackProgressChange.AddListener(PlayerController_OnAttackProgressChange);
        _playerController.OnStartedLoadingAttack.AddListener(Show);
        _playerController.OnStoppedLoadingAttack.AddListener(Hide);
        _image.fillAmount = 0f;

        Hide();
    }

    private void PlayerController_OnAttackProgressChange(float progressNormalized)
    {
        _image.fillAmount = progressNormalized;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
