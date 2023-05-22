public class ProtesterFlowfieldAI : IFlowfieldAI
{
    protected override void Start()
    {
        base.Start();
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
    }

    protected override void ProtestManager_OnFlowFieldsCreated()
    {
        base.ProtestManager_OnFlowFieldsCreated();
        StartCoroutine(FollowProtestPath());
    }
}