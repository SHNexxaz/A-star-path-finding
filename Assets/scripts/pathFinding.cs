using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class pathFinding : MonoBehaviour {

    grid grid;
    public Transform seeker, target;

    void Awake()
    {
        grid = GetComponent<grid>();
    }

    void Update()
    {
        findPath(seeker.position, target.position);
    }

    void findPath(Vector3 startPos, Vector3 targetPos)
    {
        node startNode = grid.nodeFromWorldPoint(startPos);
        node targetNode = grid.nodeFromWorldPoint(targetPos);

        //use list to hold open set since we need to search for lowest f_cost
        List<node> openSet = new List<node>();
        //use hash set to hold closed set since no search operation is needed
        //hash set is a lot quicker
        HashSet<node> closedSet = new HashSet<node>();

        //add the start node to open set
        openSet.Add(startNode);

        //while open set is not empty
        while(openSet.Count>0)
        {
            node currentNode = openSet[0];
            for(int i=1;i<openSet.Count;++i)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    //find the node in open set with lowest f_cost
                    currentNode = openSet[i];
            }

            //remove current node from open set and add it to closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            //path has been found
            {
                RetracePath(startNode, targetNode);
                return;
            }
                

            foreach (node neighbour in grid.getNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    //skip to the next neighbour
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                //if new path to neighbour is shorter OR neighbour is not in open set
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //choose this neighbour to proceed
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, targetNode);
                    //set parent
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }

            }
        }
    }

    void RetracePath(node startNode, node endNode)
    {
        List<node> path = new List<node>();
        node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.RetractPath = path;
    }

    int getDistance(node nodeA, node nodeB)
    {
        int Xdistance = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int Ydistance = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (Xdistance > Ydistance)
            return 14 * Ydistance + 10 * (Xdistance - Ydistance);
        return 14 * Xdistance + 10 * (Ydistance - Xdistance);
    }
}

