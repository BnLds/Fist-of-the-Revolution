using UnityEngine;

public struct FlowFieldData
{
    public int index;
    public string name;
    public Vector3 target;
    public FlowField flowField;

    public FlowFieldData(int _index, string _name, Vector3 _target, FlowField _grid)
    {
        index = _index;
        name = _name;
        target = _target;
        flowField = _grid;
    }
}
