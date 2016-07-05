using UnityEngine;
using System.Collections;

public class node {
    public bool walkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    //position in the grid map
    public int gridX;
    public int gridY;
    public node parent;

    public node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
