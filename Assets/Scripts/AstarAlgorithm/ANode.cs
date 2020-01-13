using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANode// : IHeapItem<ANode>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public ANode parent;
    int heapIndex;

    public ANode(bool nWalkable, Vector3 nWorldPos, int nX, int nY, int nPenalty)
    {
        walkable = nWalkable;
        worldPosition = nWorldPos;
        gridX = nX;
        gridY = nY;
        movementPenalty = nPenalty;
    }

    public int fCost
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

    //public int CompareTo(ANode nodeToCompare)
    //{
    //    int compare = fCost.CompareTo(nodeToCompare.fCost);
    //    if(compare==0)
    //    {
    //        compare = hCost.CompareTo(nodeToCompare.hCost);
    //    }
    //    return -compare;
    //}
}
