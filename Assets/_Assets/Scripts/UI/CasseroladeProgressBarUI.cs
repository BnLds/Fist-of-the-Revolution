using System;
using UnityEngine;
using UnityEngine.UI;

public class CasseroladeProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        _playerController.OnCasseroladeProgressChange.AddListener(PlayerController_OnCasseroladeProgressChange);
        _playerController.OnCasseroladeCooldownChange.AddListener(PlayerController_OnCasseroladeCooldownChange);
        _playerController.OnStartedCasserolade.AddListener(Show);
        _playerController.OnCasseroladeCdOver.AddListener(Hide);
        _image.fillAmount = 0f;

        Hide();
    }

    private void PlayerController_OnCasseroladeCooldownChange(float progressNormalized)
    {
        _image.color = Color.yellow;
        _image.fillAmount = 1 -  progressNormalized;
    }

    private void PlayerController_OnCasseroladeProgressChange(float progressNormalized)
    {
        _image.color = Color.green;
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
