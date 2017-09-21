using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {


    public BuildingButton ClickedButton
    {
        get;
        set;
    }

    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            this.gold = value;
            this.goldText.text = "<color=yellow>$</color>" + value.ToString() ;
        }
    }

    public int Income
    {
        get
        {
            return income;
        }

        set
        {
            this.income = value;
        }
    }

    [SerializeField]
    private Text goldText;

    private int gold;
    private int income;
    private float incomeMultiplier;

    public ObjectPool Pool { get; set; }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
        
    }
    // Use this for initialization
    void Start () {
        Gold = 50000;
        Income = 10;
        incomeMultiplier = 0.05f;
        InvokeRepeating("GiveIncome", 5.0f, 5.0f);
	}
	
	// Update is called once per frame
	void Update () {
        HandleCancel();
	}

    public void PickBuilding(BuildingButton clickedButton)
    {
        if (Gold >= clickedButton.Price)
        {
            this.ClickedButton = clickedButton;
            BuildingHover.Instance.ActivateHover(clickedButton.Sprite);
        }
        
    }

    public void BuyBuilding(Point buildingPos)
    {
        if (Gold >= ClickedButton.Price)
        {
            Gold -= ClickedButton.Price;
            IncreaseIncome(ClickedButton.Price);
            BuildingHover.Instance.DeavtivateHover();
            StartBuilding(buildingPos);

        }
    }

    private void HandleCancel()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            BuildingHover.Instance.DeavtivateHover();
        }
    }

    //TODO: add income animation??
    private void GiveIncome()
    {
        Gold += Income;
        Debug.Log("Current income: " + income);
    }

    private void IncreaseIncome(int goldSpent)
    {
        int incomeIncrese = (int)Math.Round(incomeMultiplier * goldSpent);
        income += incomeIncrese;
    }

    private void StartBuilding(Point buildingPos)
    {
        StartCoroutine(SpawnUnit(3.0f, buildingPos));
    }

    private IEnumerator SpawnUnit(float spawnTime, Point unitPos)
    {
        Debug.Log("SPAWNING A MONSTER");
        //FIX THIS SHIT
        int unitIndex = 0;
        string type = "Soldier";

        Unit unit = Pool.GetObject(type).GetComponent<Unit>();
        Stack<Node> unitPath = LevelManager.Instance.GeneratePath(unitPos, LevelManager.Instance.EnemyCastleSpawn);
        unit.Spawn(unitPos, unitPath);
        yield return new WaitForSeconds(spawnTime);
    }
}

