using UnityEngine;

public class PoliceFlowfieldAI : IFlowfieldAI
{
    [SerializeField] private PoliceUnitSM _policeUnitSM;

    protected override void Start()
    {
        base.Start();

        _policeUnitSM.OnFollowProtestEntry.AddListener(PoliceUnitSM_OnFollowProtestEntry);
    }

    protected override void ProtestManager_OnFlowFieldsCreated()
    {
        base.ProtestManager_OnFlowFieldsCreated();

        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
        StartCoroutine(FollowProtestPath());
    }

    private void PoliceUnitSM_OnFollowProtestEntry()
    {
        //Update the flowfield to go the most forward protest point when a cop re-enter the FollowProtestState 
        _protesterData.CurrentFlowFieldIndex = ProtesterCollectionManager.Instance.GetForwardProtestPointIndex();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        CancelInvoke(PERFORM_DETECTION);
    }
}
