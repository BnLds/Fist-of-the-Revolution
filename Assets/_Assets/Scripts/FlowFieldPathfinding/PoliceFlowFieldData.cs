using UnityEngine;

public class PoliceFlowFieldData : MonoBehaviour
{
    public Vector3 Target;
    public FlowField FlowField;

    public PoliceFlowFieldData(Vector3 _target, FlowField _grid)
    {
        Target = _target;
        FlowField = _grid;
    }
}
