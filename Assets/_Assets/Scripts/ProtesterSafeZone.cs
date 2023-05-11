using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProtesterSafeZone : MonoBehaviour
{
    [Header("Initialization Parameters")]
    [SerializeField] private ProtesterData _protesterData;

    [Space(5)]
    [Header("Game Balance Parameters")]
    //MinAttribute equal to 2x character radius
    [SerializeField] [MinAttribute(1f)] private float _safeZoneRadius = 1.5f;
    [SerializeField] private float _countdownToLoseIDMax = 2f;
    [SerializeField] private float _countdownToUntrackMax = 7f;

    [HideInInspector] public UnityEvent OnPlayerEnterSafeZone;
    [HideInInspector] public UnityEvent OnPlayerExitSafeZone;
    [HideInInspector] public UnityEvent<Transform> OnPlayerIDedFree;
    [HideInInspector] public UnityEvent OnPlayerTrackedFree;

    private bool _isPlayerAlreadyInSafeZone;
    private float _countdownToLoseID;
    private float _countdownToUntrack;

    private void Awake()
    {
        _isPlayerAlreadyInSafeZone = false;
        _countdownToLoseID = _countdownToLoseIDMax;
        _countdownToUntrack = _countdownToUntrackMax;
    }

    private void Update()
    {
        bool isSameSkinAsPlayer = _protesterData.Skin == PlayerController.Instance.GetComponentInChildren<PlayerVisual>().GetSkinSO();
        bool isPlayerWithinSafeZoneDistance = Utility.Distance2DBetweenVector3(transform.position, PlayerController.Instance.transform.position) <= _safeZoneRadius;

        if(_isPlayerAlreadyInSafeZone && isPlayerWithinSafeZoneDistance)
        {
            if (PoliceResponseData.IsPlayerIdentified)
            {
                _countdownToLoseID -= Time.deltaTime;
                if (_countdownToLoseID <= 0)
                {
                    Debug.Log("PLAYER NOT IDED ANYMORE");
                    OnPlayerIDedFree?.Invoke(_protesterData.transform);
                    _countdownToLoseID = _countdownToLoseIDMax;
                }
            }
            else
            {
                //player is tracked
                _countdownToUntrack -= Time.deltaTime;
                if (_countdownToUntrack <= 0)
                {
                    Debug.Log("PLAYER NOT TRACKED ANYMORE");
                    OnPlayerTrackedFree?.Invoke();
                    _countdownToUntrack = _countdownToUntrackMax;
                }
            }
            _isPlayerAlreadyInSafeZone = false;
        }
        else if(_isPlayerAlreadyInSafeZone && !isPlayerWithinSafeZoneDistance)
        {
            //player exited safe zone
            _countdownToLoseID = _countdownToLoseIDMax;
            _isPlayerAlreadyInSafeZone = false;
            OnPlayerExitSafeZone?.Invoke();
        }

        bool playerIsTracked = PoliceResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform == PlayerController.Instance.transform).SuspectTransform != null;
        if ((PoliceResponseData.IsPlayerIdentified || playerIsTracked) && isSameSkinAsPlayer && isPlayerWithinSafeZoneDistance && !_isPlayerAlreadyInSafeZone)
        {
            _isPlayerAlreadyInSafeZone = true;
            OnPlayerEnterSafeZone?.Invoke();
        }
    }

    public float GetSafeZoneRadius()
    {
        return _safeZoneRadius;
    }
}
