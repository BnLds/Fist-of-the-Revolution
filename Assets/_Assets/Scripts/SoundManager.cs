using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundsEffectsVolume";

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    [SerializeField] private AudioSource _camAudioSource;

    private float _volume = 1f;
    private bool _isWeaponEquipped = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        float volumeMultiplier= 1f;
        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volumeMultiplier * _volume);
    }

    private void Start()
    {
        PlayerController.Instance?.OnAttackPerformed.AddListener(PlayAttackSound);

        PlayerController.Instance?.OnItemEquipped.AddListener(() =>
        {
            PlaySound(_audioClipRefsSO.ObjectPickUp);
        });

        PlayerController.Instance?.OnWeaponEquipped.AddListener(() =>
        {
            _isWeaponEquipped = true;
        });

        BreakablesCollectionManager.Instance?.OnDestroyedObject.AddListener(() =>
        {
            PlaySound(_audioClipRefsSO.DestroyedBillboard);
        });
    }

    private void OnDisable()
    {
        PlayerController.Instance?.OnAttackPerformed.RemoveAllListeners();
        PlayerController.Instance?.OnItemEquipped.RemoveAllListeners();
        PlayerController.Instance?.OnWeaponEquipped.RemoveAllListeners();
        BreakablesCollectionManager.Instance?.OnDestroyedObject.RemoveAllListeners();
    }

    private void PlayAttackSound(Transform arg0)
    {
        if(_isWeaponEquipped)
        {
            PlaySound(_audioClipRefsSO.AttackArmed);
        }
        else
        {
            PlaySound(_audioClipRefsSO.AttackUnarmed);
        }
    }

    public void PlayFootstepsSound()
    {
        PlaySound(_audioClipRefsSO.Footstep);

    }

    private void PlaySound(AudioClip[] audioClipArray)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)]);
    }

    private void PlaySound(AudioClip audioClip)
    {
        _camAudioSource.PlayOneShot(audioClip, _volume);
    }

    public void PlayButtonClickSound()
    {
        PlaySound(_audioClipRefsSO.ButtonClick);
    }

    public void ChangeVolume()
    {
        _volume += .1f;
        if(_volume > 1f)
        {
            _volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return _volume;
    }

}
