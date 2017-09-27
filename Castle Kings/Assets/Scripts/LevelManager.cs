using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    public GameObject emptyTilePrefab;

    [SerializeField]
    public GameObject occupiedTilePrefab;

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;



    private Point playerCastleSpawn = new Point(7, 9);
    private Point enemyCastleSpawn = new Point(45,9);
    private int castleWidth = 5; //Needs to be odd
    private int castleHeight = 4;

    
    

    [SerializeField]
    private GameObject castleWallPrefab;
    [SerializeField]
    private GameObject castleEntrancePrefab;

    public Dictionary<Point, TileHandler> Tiles { get; set; }

    private Stack<Node> path;

    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    public Point EnemyCastleSpawn
    {
        get
        {
            return enemyCastleSpawn;
        }
    }

    public Point PlayerCastleSpawn
    {
        get
        {
            return playerCastleSpawn;
        }
    }


    // Use this for initialization
    void Start ()
    {
        CreateLevel();
    }

    // Update is called once per frame
    void Update () {
	}

    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileHandler>();

        //Load the level from a txt file
        string[] mapData = LoadLevel();

        int mapXSize = mapData[0].ToCharArray().Length;
        int mapYSize = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        //Make sure the world starts in the top left corner of the screen
        Vector3 worldStartPos = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        for (int y = 0; y < mapYSize; y++)
        {

            char[] tileRow = mapData[y].ToCharArray();

            for (int x = 0; x < mapXSize; x++)
            {
                PlaceTile(tileRow[x].ToString(),x, y, worldStartPos);

            }

        }
        //Save the bottom-right tile
        maxTile = Tiles[new Point(mapXSize - 1, mapYSize - 1)].transform.position;

        //Limit the camera movement to the tiles created
        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

        CreateCastles();
    }

    //Places a tile in a pos x,y relative to the top left corner of the screen
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStartPos)
    {
        int tileIndex = Convert.ToInt32(tileType,16); //Hex to int
        TileHandler newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileHandler>();

        newTile.Setup(new Point(x, y), new Vector3(worldStartPos.x + (TileSize * x), worldStartPos.y - (TileSize * y), 0), map);


    }

    private string[] LoadLevel()
    {
        TextAsset data = Resources.Load("Level") as TextAsset;
        string levelText = data.text.Replace(Environment.NewLine, string.Empty);

        return levelText.Split('-');
    }

    private void CreateCastles()
    {

        //Create the entrance
        Instantiate(castleEntrancePrefab, Tiles[playerCastleSpawn].GetComponent<TileHandler>().WorldPos, Quaternion.identity);
        Tiles[playerCastleSpawn].GetComponent<TileHandler>().IsEmpty = false;
        Tiles[playerCastleSpawn].GetComponent<TileHandler>().Walkable = true;


        //Create the rest of the castle
        for (int x = playerCastleSpawn.X - ((castleWidth-1)/2); x <= playerCastleSpawn.X + ((castleWidth - 1) / 2); x++)
        {
            for (int y = playerCastleSpawn.Y; y > playerCastleSpawn.Y - castleHeight; y--)
            {
                if (x != playerCastleSpawn.X || y != playerCastleSpawn.Y)
                {
                    Instantiate(castleWallPrefab, Tiles[new Point(x, y)].GetComponent<TileHandler>().WorldPos, Quaternion.identity);
                    Tiles[new Point(x, y)].GetComponent<TileHandler>().IsEmpty = false;
                    Tiles[new Point(x, y)].GetComponent<TileHandler>().Walkable = false;


                }
            }
        }

        //TODO: CREATE ENEMY CASTLE
        Instantiate(castleEntrancePrefab, Tiles[enemyCastleSpawn].GetComponent<TileHandler>().WorldPos, Quaternion.identity);
        Tiles[enemyCastleSpawn].GetComponent<TileHandler>().IsEmpty = false;
        Tiles[enemyCastleSpawn].GetComponent<TileHandler>().Walkable = true;

        for (int x = enemyCastleSpawn.X - ((castleWidth - 1) / 2); x <= enemyCastleSpawn.X + ((castleWidth - 1) / 2); x++)
        {
            for (int y = enemyCastleSpawn.Y; y > enemyCastleSpawn.Y - castleHeight; y--)
            {
                if (x != enemyCastleSpawn.X || y != enemyCastleSpawn.Y)
                {
                    Instantiate(castleWallPrefab, Tiles[new Point(x, y)].GetComponent<TileHandler>().WorldPos, Quaternion.identity);
                    Tiles[new Point(x, y)].GetComponent<TileHandler>().IsEmpty = false;
                    Tiles[new Point(x, y)].GetComponent<TileHandler>().Walkable = false;


                }
            }
        }

    }

    public bool ValidPos(Point pos)
    {
        return Tiles.ContainsKey(pos);
    }

    public Stack<Node> GeneratePath(Point start, Point goal)
    {
        return AStar.GetPath(start, goal);
    }
}
