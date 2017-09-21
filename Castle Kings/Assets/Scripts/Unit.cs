using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Stack<Node> path;

    public Point GridPos { get; set; }

    //The next tile to move to
    private Vector3 destination;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
	}

    public void Spawn(Point spawnPos, Stack<Node> path)
    {
        Debug.Log("SPAWNING UNIT");
        transform.position = LevelManager.Instance.Tiles[spawnPos].transform.position;
        this.GetComponent<SpriteRenderer>().sortingOrder = 50; //TODO: FIX SORTING SYSTEM
        SetPath(path);
    }

    public void SetPath(Stack<Node> newPath)
    {
        if(newPath != null)
        {
            path = newPath;
            GridPos = path.Peek().GridPos;
            destination = path.Pop().WorldPos;
        }
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if(transform.position == destination)
        {
            if(path != null && path.Count > 0)
            {
                GridPos = path.Peek().GridPos;
                destination = path.Pop().WorldPos;
            }
        }
    }

     
}
