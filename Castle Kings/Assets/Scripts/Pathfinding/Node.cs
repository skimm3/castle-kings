using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public Point GridPos { get; private set; }

    public TileHandler TileRef { get; private set; }

    public Vector2 WorldPos { get; set; }

    public Node Parent { get; private set; }

    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }



    public Node(TileHandler tileRef)
    {
        this.TileRef = tileRef;
        this.GridPos = tileRef.GridPos;
        this.WorldPos = tileRef.WorldPos;
    }

    public void CalcValues(Node parent, Node goal, int gCost)
    {
        this.Parent = parent;
        G = parent.G + gCost;
        H = (Math.Abs(GridPos.X - goal.GridPos.X) + Math.Abs(GridPos.Y - goal.GridPos.Y)) * 10;
        F = G + H;
    }
}
