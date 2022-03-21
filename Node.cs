using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{

    // if there are walls, preventing walking path
    public bool canWalk;

    // node position in the world coordinate
    public Vector3 worldPos;

    // coordinate in the grid array
    public int gridX, gridY;

    // exact cost and estimated cost
    public int gCost;
    public int hCost;
    // total cost
    public int fCost
    {
        get 
        {
            if (canWalk == false){
                return 99999;
            }
            return gCost + hCost;
        
        }
    }

    // pointer
    int heapIndex;

    // internal construction pointer
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

    // save path structure
    public Node parent;
    public Node(bool _canWalk, Vector3 _worldPos, int _gridX, int _gridY)
    {
        this.canWalk = _canWalk;
        this.worldPos = _worldPos;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }

    // compare node cost
    public int CompareTo(Node nodeToCompare)
    {
        //a.compareto(b) a<b=-1 a=b=0 a>b=1
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare==0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
