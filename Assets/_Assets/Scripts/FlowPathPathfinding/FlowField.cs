using UnityEngine;
using System.Collections.Generic;

public class FlowField
{
    private const int ENCUMBERED_COST = 5;

    public Node[,] grid { get; private set; }
    public Vector2 gridWorldSize { get; private set; }
    public float nodeRadius { get; private set; }
    public LayerMask unwalkableMask { get; private set; }
    public LayerMask encumberedMask { get; private set; }


    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private Vector3 worldBottomLeftCorner;
    private Vector3 worldGridPosition;
    private Node destinationNode;


    public FlowField(float _nodeRadius, Vector2 _gridWorldSize, LayerMask _unwalkableMask, LayerMask _encumburedMask)
    {
        nodeRadius = _nodeRadius;
        gridWorldSize = _gridWorldSize;
        unwalkableMask = _unwalkableMask;
        encumberedMask = _encumburedMask;
        nodeDiameter = _nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    public void CreateGrid(Vector3 position)
    {
        grid = new Node[gridSizeX, gridSizeY];
        worldGridPosition = position;
        worldBottomLeftCorner = position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeftCorner + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, new Vector2Int(x,y));
            }
        }
    }

    public void CreateCostField()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                bool hasIncreasedCost = false;
                Vector3 worldPoint = worldBottomLeftCorner + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                if(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                {
                    grid[x, y].cost = byte.MaxValue;
                }
                else if(Physics.CheckSphere(worldPoint, nodeRadius, encumberedMask) && !hasIncreasedCost)
                {
                    grid[x, y].IncreaseCost(ENCUMBERED_COST);
                    hasIncreasedCost = true;
                }
            }
        }

    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        int x = Mathf.FloorToInt(Mathf.Clamp(gridSizeX * percentX, 0, gridSizeX - 1)); // need to substract 1 from gridSizeX as x and y node coordinates are 0 based in the grid array 
        int y = Mathf.FloorToInt(Mathf.Clamp(gridSizeY * percentY, 0, gridSizeY - 1));

        return (grid[x, y]);
    }

    public void CreateIntegrationField(Node _destinationNode)
    {
        destinationNode = _destinationNode;

        destinationNode.cost = 0;
        destinationNode.bestCost = 0;

        Queue<Node> nodesToCheck = new Queue<Node>();
        nodesToCheck.Enqueue(destinationNode);

        while(nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();
            List<Node> currentNeighbours = GetNeighbourNodes(currentNode.gridIndex, GridDirection.CardinalDirections);
            foreach(Node currentNeighbour in currentNeighbours)
            {
                if(currentNeighbour.cost == byte.MaxValue) continue;
                if(currentNeighbour.cost + currentNode.bestCost < currentNeighbour.bestCost)
                {
                    currentNeighbour.bestCost = (ushort)(currentNeighbour.cost + currentNode.bestCost);
                    nodesToCheck.Enqueue(currentNeighbour);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach(Node currentNode in grid)
        {
            List<Node> currentNeighbours = GetNeighbourNodes(currentNode.gridIndex, GridDirection.allDirections);

            int bestCost = currentNode.bestCost;

            foreach(Node currentNeighbour in currentNeighbours)
            {
                if(currentNeighbour.bestCost < bestCost)
                {
                    bestCost = currentNeighbour.bestCost;
                    currentNode.bestDirection = GridDirection.GetDirectionFromVector2Int(currentNeighbour.gridIndex - currentNode.gridIndex);
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
        if(finalPosition.x < 0 || finalPosition.x >= gridSizeX || finalPosition.y < 0 || finalPosition.y >= gridSizeY)
        {
            return null;
        }
        else
        {
            return grid[finalPosition.x, finalPosition.y];
        }
    }
}
