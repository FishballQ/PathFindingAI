using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath : MonoBehaviour
{
    public Transform player;     // player position is start point of the path
    public Transform EndPoint;   // enemy(another object) position is end point
    Grid grid;                   // map grid

    void Start()
    {
        grid = GetComponent<Grid>();
    }

    private void FixedUpdate()
    {
        FindingPath(player.position, EndPoint.position);
    }

    //
    public void FindingPath(Vector3 StarPos, Vector3 EndPos)
    {
        // start point and final point
        Node startNode = grid.GetFromPos(StarPos);
        Node endNode = grid.GetFromPos(EndPos);
        // instantiate the heap size according to the map size
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        // use hash list as the close list to improve search efficiency
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        // continue searching until no unexplored node in open list
        while (openSet.Count() > 0)
        {
            // remove explored node
            Node currentNode = openSet.RemoveFirst();
            // add new node
            closeSet.Add(currentNode);

            if (currentNode == endNode)
            {
                GeneratePath(startNode,endNode);
                return;
            }

            // find lowest cost node from neighbours
            foreach (var item in grid.GetNeighbour(currentNode))
            {
                if (!item.canWalk || closeSet.Contains(item))
                    continue;
                int newCost = currentNode.gCost + GetDistanceNodes(currentNode, item);
                if (newCost<item.gCost||!openSet.Contains(item))
                {
                    item.gCost = newCost;
                    item.hCost = GetDistanceNodes(item, endNode);
                    item.parent = currentNode;
                    if (!openSet.Contains(item))
                    {
                        openSet.Add(item);
                    }
                }
            }
        }
    }

    private void GeneratePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node temp = endNode;
        while (temp!=startNode)
        {
            path.Add(temp);
            temp = temp.parent;
        }
        // let first item of path list is start point
        path.Reverse();
        // pass the list to grid script, to draw the path visible
        grid.path = path;
    }

    int GetDistanceNodes(Node a, Node b)
    {
        // estimate weight, to compare grid quantities in x-axis and y-axis
        int cntX = Mathf.Abs(a.gridX - b.gridX);
        int cntY = Mathf.Abs(a.gridY - b.gridY);
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }

        // Manhattan algorithm 
        //return Mathf.Abs(a.gridX - b.gridX) * 10 + Mathf.Abs(a.gridY - b.gridY) * 10;
    }
}
