using UnityEngine;
using PathCreation;
using System.Collections.Generic;

[RequireComponent(typeof(PathCreator))]
public class GeneratePath : MonoBehaviour {

    [SerializeField] private bool _closedLoop = true;
    private List<Transform> _waypoints;

    private void Start () 
    {
        _waypoints = ProtestPath.Instance.GetProtestPath();

        if (_waypoints.Count > 0) {
            // Create a new bezier path from the waypoints.
            BezierPath bezierPath = new BezierPath (_waypoints, _closedLoop, PathSpace.xyz);
            bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
            GetComponent<PathCreator> ().bezierPath = bezierPath;
        }
    }
}
