using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProtestSound : MonoBehaviour
{
    [Header("Initialization Parameters")]
    [SerializeField] private Transform _player;
    [SerializeField] private ProtesterCollectionManager _protesterCollectionManager;
    [SerializeField] private LayerMask _protesterLayer;

    [Space(5)]
    [Header("Sound Design Parameters")]
    [SerializeField][Range(0f,1f)] private float _maxVolume = 1f;
    [SerializeField] private float _maxSoundDistance = 20f;
    [SerializeField] private float _minRollOffDistance = 5f;
    [SerializeField] private float _volumeRollOffScale = 8f;

    private AudioSource _audioSource;
    private float _volumeDistanceAttenuation;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        UpdateVolumeAttenuation();
        _audioSource.volume =_maxVolume * _volumeDistanceAttenuation;
    }

    private void Update()
    {
        UpdateVolumeAttenuation();
        float currentVolume = _audioSource.volume;
        _audioSource.volume = _maxVolume * _volumeDistanceAttenuation;
    }

    private void UpdateVolumeAttenuation()
    {
        List<Collider> closestProtesters = Physics.OverlapSphere(_player.position, _maxSoundDistance, _protesterLayer).ToList();

        float shortestDistance = Mathf.Infinity;
        foreach(Collider protester in closestProtesters)
        {
            shortestDistance = Mathf.Min(shortestDistance, Utility.Distance2DBetweenVector3(_player.position, protester.transform.position));
        }

        float distanceToProtest = shortestDistance;
        _volumeDistanceAttenuation = _minRollOffDistance * (1 / (1 + _volumeRollOffScale * distanceToProtest - 1));
    }


}
