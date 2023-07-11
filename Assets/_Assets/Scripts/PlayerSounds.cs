using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private OptionsUI _optionsUI;

    private float _footstepTimer;
    private float _footstepTimerMax = .15f;
    private bool _isFootstepsOn = true;

    private void Start()
    {
        _isFootstepsOn = _optionsUI.IsFootstepsOn();

        _optionsUI.OnToggleFootstepsSound.AddListener(() =>
        {
            _isFootstepsOn = _optionsUI.IsFootstepsOn();
        });
    }

    private void Update()
    {
        _footstepTimer -= Time.deltaTime;
        if(_footstepTimer < 0f)
        {
            _footstepTimer = _footstepTimerMax;

            if(PlayerController.Instance.IsWalking() && _isFootstepsOn)
            {
                SoundManager.Instance.PlayFootstepsSound(.5f);
            }
        }
    }
}
