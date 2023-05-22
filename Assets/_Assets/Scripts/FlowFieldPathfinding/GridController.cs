using UnityEngine;
using UnityEditor;

public class GridController : MonoBehaviour
{
    public static GridController Instance { get; private set; }

    [SerializeField] private Vector2 _gridWorldSize = new Vector2(50,50);
    [SerializeField] private float _nodeRadius = .6f;
    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private LayerMask _encumberedMask;

    private FlowField _currentFlowField;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else 
        {
            Instance = this;
        }

    }
   
    private void InitializeFlowField()
    {
        _currentFlowField = new FlowField(_nodeRadius, _gridWorldSize, _unwalkableMask, _encumberedMask);
        _currentFlowField.CreateGrid(transform.position);
    }

    public FlowField GenerateFlowField(Vector3 targetPosition)
    {
        InitializeFlowField();
        _currentFlowField.CreateCostField();
        Node targetGridPosition = _currentFlowField.GetNodeFromWorldPoint(targetPosition);
        if(targetGridPosition.Cost == byte.MaxValue)
        {
            Debug.LogWarning("Trying to create a flowfield but destination node is unreachable. A flowfield to the nearest reachable point will be generated.");
        }
        _currentFlowField.CreateIntegrationField(targetGridPosition);

        _currentFlowField.CreateFlowField();

        return _currentFlowField;
    }
    
    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        if(_currentFlowField != null)
        {
            if(_currentFlowField.Grid != null)
            {
                foreach(Node node in _currentFlowField.Grid)
                {
                    Gizmos.color = node.Walkable ? Color.green : Color.red;
                    Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * (_nodeRadius*2 - .1f));
                    
                    
                    //float t = (float) node.BestCost / 75;
                    //Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, t);
                    //Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeRadius*2 - .1f));
                    
                    //Handles.Label(node.WorldPosition, node.BestCost.ToString());
                }
            }
        }
    }*/
    
    
    
    
    
}
