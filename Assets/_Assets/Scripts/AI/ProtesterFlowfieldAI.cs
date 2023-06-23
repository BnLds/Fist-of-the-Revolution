public class ProtesterFlowfieldAI : IFlowfieldAI
{
    protected override void Start()
    {
        base.Start();
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
    }

    protected override void ProtestFlowfield_OnFlowFieldsCreated()
    {
        base.ProtestFlowfield_OnFlowFieldsCreated();
        InvokeRepeating(FOLLOW_PROTEST_PATH, 0f, _aiUpdateDelay);
    }
}
