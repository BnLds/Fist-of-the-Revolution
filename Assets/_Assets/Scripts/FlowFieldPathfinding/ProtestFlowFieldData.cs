using UnityEngine;

public struct ProtestFlowFieldData
{
    public int Index;
    public string Name;
    public Vector3 Target;
    public FlowField FlowField;

    public ProtestFlowFieldData(int index, string name, Vector3 target, FlowField grid)
    {
        Index = index;
        Name = name;
        Target = target;
        FlowField = grid;
    }
}
