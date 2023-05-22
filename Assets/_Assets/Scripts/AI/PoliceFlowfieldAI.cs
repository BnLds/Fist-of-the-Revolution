using UnityEngine;

public class PoliceFlowfieldAI : IFlowfieldAI
{
    [SerializeField] private PoliceUnitSM _policeUnitSM;

    protected override void Start()
    {
        base.Start();

        _policeUnitSM.OnFollowProtestEntry.AddListener(PoliceUnitSM_OnFollowProtestEntry);
        _policeUnitSM.OnFollowProtestExit.AddListener(PoliceUnitSM_OnFollowProtestExit);
    }

    private void PoliceUnitSM_OnFollowProtestEntry()
    {
        _protesterData.CurrentFlowFieldIndex = ProtesterCollectionManager.Instance.GetForwardProtestPointIndex();

        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
        StartCoroutine(FollowProtestPath());
    }

    private void PoliceUnitSM_OnFollowProtestExit()
    {
        CancelInvoke(PERFORM_DETECTION);
        StopCoroutine(FollowProtestPath());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        _policeUnitSM.OnFollowProtestEntry.RemoveListener(PoliceUnitSM_OnFollowProtestEntry);
    }
}
