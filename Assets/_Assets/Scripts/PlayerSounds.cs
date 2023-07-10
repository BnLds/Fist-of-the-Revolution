using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private float _footstepTimer;
    private float _footstepTimerMax = .1f;

    private void Udpate()
    {
        _footstepTimer -= Time.deltaTime;
        if(_footstepTimer < 0f)
        {
            _footstepTimer = _footstepTimerMax;

            if(PlayerController.Instance.IsWalking())
            {
                SoundManager.Instance.PlayFootstepsSound();
            }
        }
    }
}
