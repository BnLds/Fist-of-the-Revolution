using UnityEngine;
using PathCreation;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class PathIndicator : MonoBehaviour
{
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private EndOfPathInstruction _endOfPathInstruction;
    [SerializeField] private float _speed = 5;
    private float distanceTravelled;

    private void Start() 
    {
        transform.position = ProtestPath.Instance.GetEndOfProtest().position;
        
        if (_pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            _pathCreator.pathUpdated += OnPathChanged;
        }
    }

    private void Update()
    {
        if (_pathCreator != null)
        {
            distanceTravelled += _speed * Time.deltaTime;
            transform.position = _pathCreator.path.GetPointAtDistance(distanceTravelled, _endOfPathInstruction);
            transform.rotation = _pathCreator.path.GetRotationAtDistance(distanceTravelled, _endOfPathInstruction);
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    private void OnPathChanged() 
    {
        distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}
