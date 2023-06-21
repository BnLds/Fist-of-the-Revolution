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
    }

    private void Start()
    {
        ProtesterCollectionManager.Instance.OnPlayerTrackFree.AddListener(ProtesterCollectionManager_OnPlayerTrackFree);
        PlayerController.Instance.OnHideTimesChange.AddListener(PlayerController_OnHideTimeChange);

        _countdownToLoseID = PlayerController.Instance.GetLoseIDTime();
        _countdownToUntrack = PlayerController.Instance.GetUntrackTime();
    }

    private void PlayerController_OnHideTimeChange()
    {
        //check if the timers are not running
        if(!_isPlayerAlreadyInSafeZone)
        {
            //update them if not running
            _countdownToLoseID = PlayerController.Instance.GetLoseIDTime();
            _countdownToUntrack = PlayerController.Instance.GetUntrackTime();
        }
    }

    private void Update()
    {

        if(PoliceResponseManager.Instance.IsPlayerTracked())
        {
            bool isPlayerWithinSafeZoneDistance = Utility.Distance2DBetweenVector3(transform.position, PlayerController.Instance.transform.position) <= _safeZoneRadius;
            if(isPlayerWithinSafeZoneDistance && !_isPlayerAlreadyInSafeZone)
            {
                bool isSameSkinAsPlayer = _protesterData.Skin == PlayerController.Instance.GetComponentInChildren<PlayerVisual>().GetSkinSO();

                if (isSameSkinAsPlayer)
                {
                    //display safe zone if player is within its area and is tracked or IDed
                    _isPlayerAlreadyInSafeZone = true;
                    OnPlayerEnterSafeZone?.Invoke();
                }
            }
        }

        if(_isPlayerAlreadyInSafeZone)
        {
            bool isPlayerWithinSafeZoneDistance = Utility.Distance2DBetweenVector3(transform.position, PlayerController.Instance.transform.position) <= _safeZoneRadius;

            if(isPlayerWithinSafeZoneDistance)
            {
                if (PoliceResponseManager.Instance.IsPlayerIdentified())
                {
                    _countdownToLoseID -= Time.deltaTime;
                    if (_countdownToLoseID <= 0)
                    {
                        Debug.Log("PLAYER NOT IDED ANYMORE");
                        OnPlayerIDedFree?.Invoke(_protesterData.transform);
                        _countdownToLoseID += PlayerController.Instance.GetLoseIDTime();
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
                        _countdownToUntrack += PlayerController.Instance.GetUntrackTime();
                        _isPlayerAlreadyInSafeZone = false;
                    }
                }
            }
            else if(!isPlayerWithinSafeZoneDistance)
            {
                //player exited safe zone
                _countdownToLoseID = PlayerController.Instance.GetLoseIDTime();
                _countdownToUntrack = PlayerController.Instance.GetUntrackTime();
                _isPlayerAlreadyInSafeZone = false;
                OnPlayerExitSafeZone?.Invoke();
            }
        }
    }

    private void ProtesterCollectionManager_OnPlayerTrackFree()
    {
        //ensure no zone is displayed if another protester cleared player from tracking
        _isPlayerAlreadyInSafeZone = false;
        OnPlayerExitSafeZone?.Invoke();
    }

    public float GetSafeZoneRadius()
    {
        return _safeZoneRadius;
    }
}
