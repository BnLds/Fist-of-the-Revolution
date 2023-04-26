using UnityEngine;

public class Node
{
    public bool Walkable;
    public Vector3 WorldPosition;
    public Vector2Int GridIndex;
    public byte Cost; //255 makes a node unwalkable
    public ushort BestCost;
    public GridDirection BestDirection;

    public Node(bool _walkable, Vector3 _worldPosition, Vector2Int _gridIndex)
    {
        Walkable = _walkable;
        WorldPosition = _worldPosition;
        GridIndex = _gridIndex;
        Cost = 1;
        BestCost = ushort.MaxValue;
        BestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amount)
    {
        if(Cost == byte.MaxValue) return;
        if(Cost + amount == byte.MaxValue)
        {
            Cost = byte.MaxValue;
        } 
        else
        {
            Cost += (byte) amount;
        }
    }
}
