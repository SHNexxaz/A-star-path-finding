using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class grid : MonoBehaviour {
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Transform player;
    public List<node> RetractPath;
    //create a 2d array to represent nodes
    node[,] _grid;
    float nodeDiameter;
    int gridNumX, gridNumY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        //calculate how many node can be fit into the grid
        gridNumX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridNumY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new node[gridNumX, gridNumY];
        //get the button left corner position of the grid
        Vector3 worldButtonLeft = transform.position - Vector3.right * gridWorldSize.x / 2
                                - Vector3.forward * gridWorldSize.y / 2;
        
        //loop through all the position that nodes will be in
        //to see if the position is walkable or not by collision checking
        for(int x=0;x<gridNumX;x++)
        {
            for (int y = 0; y < gridNumX; y++)
            {
                //get the position for each node
                Vector3 worldPoint = worldButtonLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                   + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //check collision with unwalkable layer mask
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                _grid[x, y] = new node(walkable, worldPoint,x,y);
            }
        }
    }

    public List<node> getNeighbours(node node)
    {
        List<node> neighbours = new List<node>();
        //create a 3*3 grid around the node
        for (int x=-1;x<=1;++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                //make sure neighbours are inside the grid map
                if (checkX >= 0 && checkX < gridNumX && checkY >= 0 && checkY < gridNumY)
                    neighbours.Add(_grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    //a method that converts world postion to grid x-y coordinate
    public node nodeFromWorldPoint(Vector3 worldPosition)
    {
        //using percentage to represent how far the node position is 
        //from buttom left corner
        float percentX = (worldPosition.x + gridWorldSize.x/2)/gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y/2)/gridWorldSize.y;
        //make sure it is always between 0-1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //map to the grid x-y coordinate
        int x = Mathf.RoundToInt((gridNumX - 1) * percentX); //0 based so -1
        int y = Mathf.RoundToInt((gridNumY - 1) * percentY);
        return _grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(_grid != null)
        {
            node playerNode = nodeFromWorldPoint(player.position);
            foreach (node n in _grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                if (playerNode == n)
                    Gizmos.color = Color.cyan;
                if(RetractPath != null)
                {
                    if (RetractPath.Contains(n))
                        Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f)); //give some margin
            }
        } 
    }
}
