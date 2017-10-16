using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileHandler : MonoBehaviour
{

    public Point GridPos { get; set; }

    public bool IsEmpty { get; set; }

    private GameObject hoverSprite = null;

    public SpriteRenderer SpriteRenderer { get; set; }

    //private SpriteRenderer spriteRenderer;
    public bool Walkable { get; set; }



    //Returns the center point of a tile
    public Vector2 WorldPos
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
        }
    }

    // Use this for initialization
    void Start () {
        SpriteRenderer = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        Walkable = true;
        IsEmpty = true;
        this.GridPos = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedButton != null)
        {

            if(hoverSprite == null)
            {
                CreateHoverSprite();
            }


            if (Input.GetMouseButtonDown(0) && IsEmpty)
            {
                PlaceBuilding(Teams.LeftTeam);
            }
        }
        
    }

    private void OnMouseExit()
    {
        RemoveHoverSprite();
    }

    public void PlaceBuilding(Teams team)
    {

        //Change the first arg to "GameManager.Instance.ClickedButton.BuildingPrefab" if different buildings get added
        GameObject newBuilding = Instantiate(GameManager.Instance.buildingPrefab, transform.position, Quaternion.identity);

        //Make sure towers don't render under each other
        newBuilding.GetComponent<SpriteRenderer>().sortingOrder = GridPos.Y;
        
        newBuilding.transform.SetParent(transform);

        IsEmpty = false;
        Walkable = false;
        RemoveHoverSprite();
        GameManager.Instance.BuyBuilding(GridPos, team);               
    }

    //TODO: enable/disable hoversprite instead of instantiating/destroying (sprite.Enable)
    private void CreateHoverSprite()
    {
        GameObject prefab = LevelManager.Instance.emptyTilePrefab;
        if (IsEmpty)
        {
            prefab = LevelManager.Instance.emptyTilePrefab;
        }
        else
        {
            prefab = LevelManager.Instance.occupiedTilePrefab;

        }

        hoverSprite = Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void RemoveHoverSprite()
    {

        Destroy(hoverSprite);
    }
}
