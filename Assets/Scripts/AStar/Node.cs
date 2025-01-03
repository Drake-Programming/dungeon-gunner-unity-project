using System;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // Distance from starting node
    public int hCost = 0; // Distance from finishing node
    public Node parentNode;
    int heapIndex;

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        /*
         * Compare will be < 0 if this instance Fcost is less than nodeToCompare.Fcost
         * Compare will be > 0 if this instance Fcost is greater than nodeToCompare.Fcost
         * Compare will be == 0 if the values are the same
        */

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
