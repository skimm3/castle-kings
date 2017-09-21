using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour {

    private TileHandler start, goal;

    [SerializeField]
    private Sprite sprite;

	// Use this for initialization
	void Start () {
		
	}
	
    //Draws the generated path
	// Update is called once per frame
	//void Update () {
 //       ClickTile();
 //       if(Input.GetKeyDown(KeyCode.Space))
 //       {
 //           AStar.GetPath(start.GridPos, goal.GridPos);
 //       }
	//}

    private void ClickTile()
    {
        if(Input.GetMouseButtonDown(2))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                TileHandler tmp = hit.collider.GetComponent<TileHandler>();
                if(tmp != null)
                {
                    if(start == null)
                    {
                        start = tmp;
                        start.SpriteRenderer.sprite = sprite;
                        start.SpriteRenderer.color = Color.green;
                    }
                    else if(goal == null)
                    {

                        goal = tmp;
                        goal.SpriteRenderer.sprite = sprite;

                        goal.SpriteRenderer.color = Color.red;

                    }
                }
            }
        }
    }

    public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList, Stack<Node> path)
    {
        foreach(Node node in openList)
        {
            if(node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.sprite = sprite;
                node.TileRef.SpriteRenderer.color = Color.cyan;
            }
            //Debug.Log(node.GridPos.X + ", " + node.GridPos.Y + " G: " + node.G + " H: " + node.H + " F: " + node.F);
            //Debug.Log("gScore of tile: " + node.GridPos.X + ", " + node.GridPos.Y + ": " + node.G);

        }

        foreach (Node node in closedList)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.sprite = sprite;
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
        }

        foreach (Node node in path)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.sprite = sprite;
                node.TileRef.SpriteRenderer.color = Color.green;
            }
        }
    }
}
