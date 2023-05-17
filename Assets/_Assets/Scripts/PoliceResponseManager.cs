using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PoliceResponseManager : MonoBehaviour
{
    public static PoliceResponseManager Instance { get; private set; }

    [Header("Initialization Parameters")]
    [SerializeField] private BreakablesCollectionManager _breakablesCollectionManager;
    [SerializeField] private PoliceWatchUI _policeWatchUI;
    [SerializeField] private LayerMask suspectMask;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] private int[] _watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};
    [SerializeField] private float suspectDetectionRadius = 10f;

    [HideInInspector] public UnityEvent OnPlayerUntracked;
    [HideInInspector] public UnityEvent<Transform> OnPlayerHidden;
    [HideInInspector] public UnityEvent<Transform> OnTrackedList;
    [HideInInspector] public UnityEvent<Transform> OnFollowed;
    [HideInInspector] public UnityEvent<Transform> OnSuspectCleared;
    [HideInInspector] public UnityEvent OnPlayerIdentified;

    private List<BreakableController> _breakablesWatched;
    private int _currentWatchValue;
    private int _currentWatchThresholdIndex;
    private PoliceResponseData _policeResponseData;

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

        _currentWatchValue = 0;
        _currentWatchThresholdIndex = 0;
        _policeResponseData = new PoliceResponseData();
    }

    private void Start()
    {
        InitializePoliceResponseData();

        _breakablesWatched = _breakablesCollectionManager.GetBreakablesList();
        ProtesterCollectionManager.Instance.OnPlayerIDFree.AddListener(ProtesterCollectionManager_OnPlayerIDFree);
        ProtesterCollectionManager.Instance.OnPlayerTrackFree.AddListener(ProtesterCollectionManager_OnPlayerTrackFree);

        foreach(BreakableController breakable in _breakablesWatched)
        {
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
            breakable.StartWatch.AddListener(Breakable_StartWatch);
        }
    }

    private void ProtesterCollectionManager_OnPlayerTrackFree()
    {
        (Transform playerSuspectTransform, bool IsPlayerTracked) playerSuspectData = _policeResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform = PlayerController.Instance.transform);
        if(playerSuspectData.playerSuspectTransform != null)
        {
            //remove player in tracked suspects list
            _policeResponseData.TrackedSuspects.Remove(playerSuspectData);
            OnPlayerUntracked?.Invoke();
        }
    }

    private void ProtesterCollectionManager_OnPlayerIDFree(Transform sender)
    {
        _policeResponseData.IsPlayerIdentified = false;
        OnPlayerHidden?.Invoke(sender);
    }

    private void Breakable_StartWatch(int watchValue, Transform sender)
    {
        //add damaged object to list of watched items
        _policeResponseData.WatchPoints.Add(sender);

        //increase watch value as soon as object is damaged
        _currentWatchValue += watchValue;

        int indexTemp = _watchThresholds.Length - 1;
        //find new watch threshhold
        while(_currentWatchValue < _watchThresholds[indexTemp]) indexTemp--;

        _currentWatchThresholdIndex = indexTemp;
        _policeWatchUI.DisplayWatchValue(_currentWatchThresholdIndex);
    }

    private void Breakable_OnDestroyedBreakable(int remainingWatchValue, BreakableController sender)
    {
        _policeResponseData.WatchPoints.Remove(sender.transform);

        //remove listeners
        sender.OnDestroyedBreakable.RemoveAllListeners();
        sender.StartWatch.RemoveAllListeners();
    }

    public void UpdateClosestSuspects(Transform sender, Vector3 damagedBreakablePosition)
    {
        List<Collider> allCollidersDetected = Physics.OverlapSphere(damagedBreakablePosition, suspectDetectionRadius, suspectMask).OrderBy(target => Utility.Distance2DBetweenVector3(sender.position, target.transform.position)).ToList();

        int numberOfSuspects = 5;
        for (int i = 0; i < (int)Mathf.Min(numberOfSuspects, allCollidersDetected.Count); i++)
        {
            if(allCollidersDetected[i] != null) 
            {
                //check if collider is already in trackedSuspect list
                if(_policeResponseData.TrackedSuspects.Select(_=> _.SuspectTransform).ToList().Contains(allCollidersDetected[i].transform))
                {
                    //go to next collider
                    continue;
                }

                //check if collider is already suspected
                if(_policeResponseData.Suspects.Contains(allCollidersDetected[i].transform))
                {
                    //remove it from list of suspects and add it to list of tracked suspects
                    _policeResponseData.Suspects.Remove(allCollidersDetected[i].transform);
                    _policeResponseData.TrackedSuspects.Add((allCollidersDetected[i].transform, IsTracked: false));
                }
                else
                {
                    //add it to suspect list
                    _policeResponseData.Suspects.Add(allCollidersDetected[i].transform);
                }
            }
        }
        
        foreach((Transform suspectTransform, bool isTracked) suspect in _policeResponseData.TrackedSuspects)
        {
            if(suspect.isTracked)
            {
                OnFollowed?.Invoke(suspect.suspectTransform);
            }
            else
            {
                OnTrackedList?.Invoke(suspect.suspectTransform);
            }
        }
        
    }

    public void ClearTrackedSuspect((Transform suspectTransform, bool) suspect)
    {
        if(_policeResponseData.TrackedSuspects.Contains(suspect))
        {
            _policeResponseData.TrackedSuspects.Remove(suspect);
        }
        OnSuspectCleared?.Invoke(suspect.suspectTransform);
    }

    private void InitializePoliceResponseData()
    {
        _policeResponseData.WatchPoints = new List<Transform>();
        _policeResponseData.Suspects = new List<Transform>();
        _policeResponseData.TrackedSuspects = new List<(Transform, bool)>();
        _policeResponseData.IsPlayerIdentified = false;
    }

    public ReadOnlyCollection<Transform> GetWatchPointsData()
    {
        return _policeResponseData.WatchPoints.AsReadOnly();
    }
    
    public ReadOnlyCollection<Transform> GetSuspectsList()
    {
        return _policeResponseData.Suspects.AsReadOnly();
    }

    public ReadOnlyCollection<(Transform SuspectTransform, bool IsTracked)> GetTrackedList()
    {
        return _policeResponseData.TrackedSuspects.AsReadOnly();
    }

    public bool IsPlayerIdentified()
    {
        return _policeResponseData.IsPlayerIdentified;
    }

    public void SetPlayerToIdentified()
    {
        _policeResponseData.IsPlayerIdentified = true;
    }

    public void AddTargetToTrackedList(Transform target)
    {
        (Transform suspectTransform, bool isTracked) suspectData = _policeResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform == target);
        //check if suspect is already in tracked list 
        if(suspectData.suspectTransform == null)
        {
            //add suspect in tracked suspects list if not already in
            (Transform, bool) newTargetData = (target, true) ;
            _policeResponseData.TrackedSuspects.Add(newTargetData);
        }
        else if(suspectData.suspectTransform != null && !suspectData.isTracked)
        {
            //update IsTracked if target already is in TrackedList
            (Transform, bool) newTargetData = (suspectData.suspectTransform, true) ;
            int index = _policeResponseData.TrackedSuspects.IndexOf(suspectData);
            _policeResponseData.TrackedSuspects[index] = newTargetData;
        }
    }

    public void SetTrackedSuspectToFollowed(int index)
    {
        (Transform, bool) newTargetData = (_policeResponseData.TrackedSuspects[index].SuspectTransform, true);
        _policeResponseData.TrackedSuspects[index] = newTargetData;
    }

}
