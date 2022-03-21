using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    Node[,] grid;                   // save grid
    public Vector2 gridSize;        // whole grid size
    public float nodeRadius;        // single node radius
    float nodeDiameter;             // single node diameter
    public LayerMask WallLayer;     // wall layer using in ray cast collision
    public Transform player;        // player position
    private int gridCntX, gridCntY; // grid quantity

    public List<Node> path = new List<Node>(); // save path

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        // convert to integer to optimise computing progress
        gridCntX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridCntY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        grid = new Node[gridCntX, gridCntY];
        CreateGrid();
    }

    // return map size 
    public int MaxSize
    {
        get
        {
            return gridCntX * gridCntY;
        }
    }

    // divide map for grids 
    private void CreateGrid()
    {
        Vector3 startPoint = transform.position - gridSize.x * 0.5f * Vector3.right - gridSize.y * 0.5f * Vector3.forward;
        for (int i = 0; i < gridCntX; i++)
        {
            for (int j = 0; j < gridCntY; j++)
            {   
                // point is center point of the grid
                Vector3 worldPoint = startPoint + Vector3.right * (i * nodeDiameter + nodeRadius) + Vector3.forward * (j * nodeDiameter + nodeRadius);
                // if the node has a wall
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, WallLayer);
                // index of the grid array
                grid[i, j] = new Node(walkable, worldPoint, i, j);
            }
        }
    }
    
    // get node by position
    public Node GetFromPos(Vector3 pos)
    {
        float percentX = (pos.x + gridSize.x * 0.5f) / gridSize.x;
        float percentY = (pos.z + gridSize.y * 0.5f) / gridSize.y;
        
        // limit the value between 0 - 1
        percentX = Mathf.Clamp01(percentX); 
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridCntX - 1) * percentX);
        int y = Mathf.RoundToInt((gridCntY - 1) * percentY);

        return grid[x, y];
    }

    // draw path
    void OnDrawGizmos()
    {
        // draw grid
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        // draw occupied grid 
        if (grid == null)
            return;
        Node playerNode = GetFromPos(player.position);
        foreach (var item in grid)
        {
            Gizmos.color = item.canWalk ? Color.white : Color.red;
            Gizmos.DrawCube(item.worldPos, Vector3.one * (nodeDiameter - 0.1f));
        }
        // draw path
        if (path != null)
        {
            foreach (var item in path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(item.worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
        // draw palyer
        if (playerNode != null && playerNode.canWalk)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(playerNode.worldPos, Vector3.one * (nodeDiameter - 0.1f));
        }
    }

    // nodes that should be compared cost
    public List<Node> GetNeighbour(Node node)
    {
        List<Node> neighbour = new List<Node>();
        // top, bottom, left and right neighbours
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int tempX = node.gridX + i;
                int tempY = node.gridY + j;

                if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY)
                {
                    neighbour.Add(grid[tempX, tempY]);
                }
            }
        }
        return neighbour;
    }
}
