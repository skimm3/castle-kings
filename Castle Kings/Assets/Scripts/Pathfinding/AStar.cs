using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    private static Dictionary<Point, Node> nodes;


    private static void CreateNodes()
    {
        nodes = new Dictionary<Point, Node>();

        //Loop through every tile found in the game
        foreach (TileHandler tile in LevelManager.Instance.Tiles.Values)
        {
            nodes.Add(tile.GridPos, new Node(tile));
        }
    }

    public static Stack<Node> GetPath(Point start, Point goal)
    {
        if(nodes == null)
        {
            CreateNodes();
        }

        HashSet<Node> openList = new HashSet<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Stack<Node> path = new Stack<Node>();

        Node currentNode = nodes[start];

        //Adds the start node to the openlist
        openList.Add(currentNode);


        while(openList.Count > 0)
        {
            //Loop through neighbours
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPos.X + x, currentNode.GridPos.Y + y);

                    if (neighbourPos != currentNode.GridPos && LevelManager.Instance.ValidPos(neighbourPos) && LevelManager.Instance.Tiles[neighbourPos].Walkable)
                    {
                        int gCost = 0;

                        //Vertical and horizontal cost
                        if (Math.Abs(x - y) == 1)
                        {
                            gCost = 10;
                        }
                        //Diagonal cost
                        else
                        {
                            if(!ConnectedCorner(currentNode, nodes[neighbourPos]))
                            {
                                //Skip the rest of the loop
                                continue;
                            }
                            gCost = 14;
                        }

                        //Add valid neighbours to the open list
                        Node neighbour = nodes[neighbourPos];

                        if (openList.Contains(neighbour))
                        {
                            //If the neighbour already have a parent but the new one is better, recalc all the values
                            if (currentNode.G + gCost < neighbour.G)
                            {
                                neighbour.CalcValues(currentNode, nodes[goal], gCost);
                            }
                        }
                        //New neighbour, welcome my friend!
                        else if (!closedList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                            neighbour.CalcValues(currentNode, nodes[goal], gCost);
                        }
                    }

                }
            }
            //Move the current node from the openList to the closedList
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Find the node with the lowest F score
            if (openList.Count > 0)
            {
                //Sorts by F score and select the first node
                currentNode = openList.OrderBy(n => n.F).First();
            }

            //WE ARE DONE
            if(currentNode == nodes[goal])
            {
                //Find the path
                while(currentNode.GridPos != start)
                {
                    path.Push(currentNode);
                    currentNode = currentNode.Parent;
                }
                break;
            }
        }
        return path;


        //DEBUG CODE:
        //GameObject.Find("AStarTester").GetComponent<AStarTest>().DebugPath(openList, closedList, path);
    }

    private static bool ConnectedCorner(Node currentNode, Node neightbour)
    {
        Point direction = neightbour.GridPos - currentNode.GridPos;
        Point first = new Point(currentNode.GridPos.X + direction.X, currentNode.GridPos.Y);
        Point second = new Point(currentNode.GridPos.X, currentNode.GridPos.Y + direction.Y);
        if(LevelManager.Instance.ValidPos(first) && !LevelManager.Instance.Tiles[first].Walkable)
        {
            return false;
        }
        if(LevelManager.Instance.ValidPos(second) && !LevelManager.Instance.Tiles[second].Walkable)
        {
            return false;
        }
        return true;
    }


}
