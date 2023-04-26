using UnityEngine;
using System.Collections.Generic;

public class FlowField
{
    private const int ENCUMBERED_COST = 5;

    public Node[,] Grid { get; private set; }
    public Vector2 GridWorldSize { get; private set; }
    public float NodeRadius { get; private set; }
    public LayerMask UnwalkableMask { get; private set; }
    public LayerMask EncumberedMask { get; private set; }


    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    private Vector3 _worldBottomLeftCorner;
    private Vector3 _worldGridPosition;
    private Node _destinationNode;


    public FlowField(float _nodeRadius, Vector2 _gridWorldSize, LayerMask _unwalkableMask, LayerMask _encumburedMask)
    {
        NodeRadius = _nodeRadius;
        GridWorldSize = _gridWorldSize;
        UnwalkableMask = _unwalkableMask;
        EncumberedMask = _encumburedMask;
        _nodeDiameter = _nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
    }

    public void CreateGrid(Vector3 position)
    {
        Grid = new Node[_gridSizeX, _gridSizeY];
        _worldGridPosition = position;
        _worldBottomLeftCorner = position - Vector3.right * GridWorldSize.x / 2 - Vector3.forward * GridWorldSize.y / 2;
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = _worldBottomLeftCorner + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.forward * (y * _nodeDiameter + NodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, NodeRadius, UnwalkableMask));
                Grid[x, y] = new Node(walkable, worldPoint, new Vector2Int(x,y));
            }
        }
    }

    public void CreateCostField()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                bool hasIncreasedCost = false;
                Vector3 worldPoint = _worldBottomLeftCorner + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.forward * (y * _nodeDiameter + NodeRadius);
                if(Physics.CheckSphere(worldPoint, NodeRadius, UnwalkableMask))
                {
                    Grid[x, y].Cost = byte.MaxValue;
                }
                else if(Physics.CheckSphere(worldPoint, NodeRadius, EncumberedMask) && !hasIncreasedCost)
                {
                    Grid[x, y].IncreaseCost(ENCUMBERED_COST);
                    hasIncreasedCost = true;
                }
            }
        }

    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPosition.z + GridWorldSize.y / 2) / GridWorldSize.y;

        int x = Mathf.FloorToInt(Mathf.Clamp(_gridSizeX * percentX, 0, _gridSizeX - 1)); // need to substract 1 from gridSizeX as x and y node coordinates are 0 based in the grid array 
        int y = Mathf.FloorToInt(Mathf.Clamp(_gridSizeY * percentY, 0, _gridSizeY - 1));

        return (Grid[x, y]);
    }

    public void CreateIntegrationField(Node _destinationNode)
    {
        this._destinationNode = _destinationNode;

        this._destinationNode.Cost = 0;
        this._destinationNode.BestCost = 0;

        Queue<Node> nodesToCheck = new Queue<Node>();
        nodesToCheck.Enqueue(this._destinationNode);

        while(nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();
            List<Node> currentNeighbours = GetNeighbourNodes(currentNode.GridIndex, GridDirection.CardinalDirections);
            foreach(Node currentNeighbour in currentNeighbours)
            {
                if(currentNeighbour.Cost == byte.MaxValue) continue;
                if(currentNeighbour.Cost + currentNode.BestCost < currentNeighbour.BestCost)
                {
                    currentNeighbour.BestCost = (ushort)(currentNeighbour.Cost + currentNode.BestCost);
                    nodesToCheck.Enqueue(currentNeighbour);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach(Node currentNode in Grid)
        {
            List<Node> currentNeighbours = GetNeighbourNodes(currentNode.GridIndex, GridDirection.allDirections);

            int bestCost = currentNode.BestCost;

            foreach(Node currentNeighbour in currentNeighbours)
            {
                if(currentNeighbour.BestCost < bestCost)
                {
                    bestCost = currentNeighbour.BestCost;
                    currentNode.BestDirection = GridDirection.GetDirectionFromVector2Int(currentNeighbour.GridIndex - currentNode.GridIndex);
                }
            }
        }
    }

    private List<Node> GetNeighbourNodes(Vector2Int nodeIndex, List<GridDirection> directions)
    {
        List<Node> neighbourNodes = new List<Node>();
        foreach(Vector2Int direction in directions)
        {
            Node newNeighbour = GetCellAtRelativePosition(nodeIndex, direction);
            if(newNeighbour != null)
            {
                neighbourNodes.Add(newNeighbour);
            }
        }
        return neighbourNodes;
    }

    private Node GetCellAtRelativePosition(Vector2Int originalPosition, Vector2Int relativePosition)
    {
        Vector2Int finalPosition = originalPosition + relativePosition;
        if(finalPosition.x < 0 || finalPosition.x >= _gridSizeX || finalPosition.y < 0 || finalPosition.y >= _gridSizeY)
        {
            return null;
        }
        else
        {
            return Grid[finalPosition.x, finalPosition.y];
        }
    }
}
