using System;
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
    private Collider[] _closestColliders;
    private int _maxColliders = 150;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _closestColliders = new Collider[_maxColliders];
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
        int numColliders = Physics.OverlapSphereNonAlloc(_player.position, _maxSoundDistance, _closestColliders, _protesterLayer);

        float shortestDistance = Mathf.Infinity;
        for (int i = 0; i < numColliders; i++)
        {
            shortestDistance = Mathf.Min(shortestDistance, Utility.Distance2DBetweenVector3(_player.position, _closestColliders[i].transform.position));
        }

        float distanceToProtest = shortestDistance;
        _volumeDistanceAttenuation = _minRollOffDistance * (1 / (1 + _volumeRollOffScale * distanceToProtest - 1));
    }
}
