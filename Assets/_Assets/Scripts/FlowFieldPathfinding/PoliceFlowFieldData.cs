using UnityEngine;

public class PoliceFlowFieldData : MonoBehaviour
{
    public Vector3 target;
    public FlowField flowField;

    public PoliceFlowFieldData(Vector3 _target, FlowField _grid)
    {
        target = _target;
        flowField = _grid;
    }
}
