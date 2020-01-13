using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public int type;
    public int x;
    public int y;
    public Vector3 worldPosition;

    public MapNode(int nType, int nX, int nY, Vector3 nWorldPosition)
    {
        type = nType;
        x = nX;
        y = nY;
        worldPosition = nWorldPosition;
    }
}
