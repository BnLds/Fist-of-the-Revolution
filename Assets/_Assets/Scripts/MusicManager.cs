using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public enum MusicTag
    {
        MainMenu,
        NewProtester,
        Casseur,
        BlackHat, 
        None
    }

    [Serializable]
    public struct MusicData
    {
        public MusicManager.MusicTag Tag;
        public List<AudioClip> Clips;
    }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    [SerializeField] private MusicDatabaseSO _musicDatabaseSO;

    private float _volume = .3f;
    private AudioSource _audioSource;
    private MusicTag _currentClipTag;

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

        _audioSource = GetComponent<AudioSource>();

        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, _volume);
        _audioSource.volume = _volume;
        _currentClipTag = MusicTag.None;
    }

    private void Start()
    {
        PlayerController.Instance?.OnAttackPerformed.AddListener((Transform arg0) => 
        { 
            PlayMusic(MusicTag.Casseur);
        });

        PoliceResponseManager.Instance?.OnPlayerIdentified.AddListener(() => 
        {
            PlayMusic(MusicTag.BlackHat);
        });
    }

    private void OnDisable()
    {
        PlayerController.Instance?.OnAttackPerformed.RemoveAllListeners();
        PoliceResponseManager.Instance?.OnPlayerIdentified.RemoveAllListeners();
    }

    public void PlayMusic(MusicTag tag)
    {
        if(_currentClipTag == tag)
        {
            return;  
        }
        else
        {
            _currentClipTag = tag;
        } 

        for (int i = 0; i < _musicDatabaseSO.MusicDatabase.Count; i++)
        {
            if(_musicDatabaseSO.MusicDatabase[i].Tag == tag)
            {
                int clipIndex = UnityEngine.Random.Range((int)0, (int)_musicDatabaseSO.MusicDatabase[i].Clips.Count - 1);
                _audioSource.clip = _musicDatabaseSO.MusicDatabase[i].Clips[clipIndex];
            }    
        }

        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void ChangeVolume()
    {
        _volume += .1f;
        if(_volume > 1f)
        {
            _volume = 0f;
        }

        _audioSource.volume = _volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return _volume;
    }
}
