using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public Vector2Int gridIndex;
    public byte cost; //255 makes a node unwalkable
    public ushort bestCost;
    public GridDirection bestDirection;

    public Node(bool _walkable, Vector3 _worldPosition, Vector2Int _gridIndex)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridIndex = _gridIndex;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amount)
    {
        if(cost == byte.MaxValue) return;
        if(cost + amount == byte.MaxValue)
        {
            cost = byte.MaxValue;
        } 
        else
        {
            cost += (byte) amount;
        }
    }
}
