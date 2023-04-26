using UnityEngine;
using UnityEditor;

public class GridController : MonoBehaviour
{
    public static GridController Instance { get; private set; }

    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField] private float _nodeRadius = .5f;
    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private LayerMask _encumberedMask;

    private FlowField _currentFlowField;

    private void Awake()
    {
        if(Instance != null)
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

    public FlowField GenerateFlowField(Transform targetTransform)
    {
        InitializeFlowField();
        _currentFlowField.CreateCostField();
        Node targetGridPosition = _currentFlowField.GetNodeFromWorldPoint(targetTransform.position);
        _currentFlowField.CreateIntegrationField(targetGridPosition);

        _currentFlowField.CreateFlowField();

        return _currentFlowField;
    }


    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(currentFlowField != null)
        {
            if(currentFlowField.grid != null)
            {
                foreach(Node node in currentFlowField.grid)
                {
                    //Gizmos.color = node.walkable ? Color.green : Color.red;
                    
                    //float t = (float) node.bestCost / 75;
                    //Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, t);
                    //Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));
                    
                    //Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));
                    Handles.Label(node.worldPosition, node.bestCost.ToString());
                }
            }
        }
    }
    */
    
    
    
    
}
