using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Teams { LeftTeam, RightTeam };


public class GameManager : Singleton<GameManager>
{


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
            this.goldText.text = "<color=yellow>$</color>" + value.ToString();
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
    public GameObject buildingPrefab;

    [SerializeField]
    public GameObject gameOverMenu;
    [SerializeField]
    public GameObject gameWonMenu;


    [SerializeField]
    private Text goldText;


    private int gold;
    private int income;

    private int enemyGold;
    private int enemyIncome;

    private float incomeMultiplier;

    private bool gameOver = false;
    private bool gameWon = false;



    private Building selectedBuilding;

    public ObjectPool Pool { get; set; }

    public Unit LeftCastle
    {
        get
        {
            return leftCastle;
        }

        set
        {
            leftCastle = value;
        }
    }

    public Unit RightCastle
    {
        get
        {
            return rightCastle;
        }

        set
        {
            rightCastle = value;
        }
    }

    private Unit leftCastle;
    private Unit rightCastle;

    private int currentLevel = 1;

    private string enemyNextBuilding = null;
    private int enemyNextBuildingCost = 0;


    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }
    // Use this for initialization
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("Current Level", 1);
        Debug.Log("STARTING THE GAME YOOOOO: " + currentLevel);
        Gold = 50000;
        enemyGold = 75;
        Income = 10;
        enemyIncome = 5;
        incomeMultiplier = 0.02f;
        InvokeRepeating("GiveIncome", 5.0f, 5.0f);
        enemyNextBuilding = "Soldier";
        enemyNextBuildingCost = 50;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCancel();
        EnemyAI();
        checkWinCondition();

    }

    public void PickBuilding(BuildingButton clickedButton)
    {
        if (Gold >= clickedButton.Price)
        {
            this.ClickedButton = clickedButton;
            BuildingHover.Instance.ActivateHover(clickedButton.Sprite);
        }

    }

    public void BuyBuilding(Point buildingPos, Teams team)
    {
        string type;
        if (team == Teams.LeftTeam)
        {
            type = ClickedButton.name.ToString();
            Gold -= ClickedButton.Price;
            IncreaseIncome(ClickedButton.Price, team);
            BuildingHover.Instance.DeavtivateHover();
        }
        else
        {
            type = enemyNextBuilding;
            enemyGold -= enemyNextBuildingCost;
            IncreaseIncome(enemyNextBuildingCost, team);
            EnemyBuildingAI();
        }
        StartBuilding(buildingPos, team, type);


    }

    //TODO: make building selectable to enable upgrades and info etc
    public void SelectBuilding(Building building)
    {

    }

    private void HandleCancel()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            BuildingHover.Instance.DeavtivateHover();
        }
    }

    private void GiveIncome()
    {
        Gold += Income;
        enemyGold += enemyIncome;
        //Debug.Log("Enemy gold: " + enemyGold + " Enemy income: " + enemyIncome);
    }

    private void IncreaseIncome(int goldSpent, Teams team)
    {
        int incomeIncrease = (int)Math.Round(incomeMultiplier * goldSpent);
        if (team == Teams.LeftTeam)
        {
            income += incomeIncrease;
        }
        else
        {
            enemyIncome += incomeIncrease;
        }

    }

    private void StartBuilding(Point buildingPos, Teams team, string type)
    {
        StartCoroutine(SpawnUnit(12.0f, buildingPos, team, type));
    }

    private IEnumerator SpawnUnit(float spawnTime, Point unitPos, Teams team, string type)
    {
        Point goal;
        if (team == Teams.LeftTeam)
        {
            goal = LevelManager.Instance.RightCastleSpawn;
        }
        else
        {
            goal = LevelManager.Instance.LeftCastleSpawn;
        }
        Stack<Node> unitPath = LevelManager.Instance.GeneratePath(unitPos, goal);
        while (true)
        {
            //Debug.Log("Spawning: " + type);
            Unit unit = Pool.GetObject(type).GetComponent<Unit>();

            unit.Spawn(unitPos, unitPath, team);
            yield return new WaitForSeconds(spawnTime);
        }

    }

    private void checkWinCondition()
    {
        if (leftCastle.Health <= 0)
        {
            GameOver();
        }
        if (rightCastle.Health <= 0)
        {
            GameWon();
        }
    }

    private void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void GameWon()
    {
        if(!gameWon)
        {
            gameWon = true;
            Debug.Log("GameWON!!!!!");
            gameWonMenu.SetActive(true);
            currentLevel = 2;
            Time.timeScale = 0;
        }

    }
    //x: 50-55
    //y: 3-15

    private void EnemyAI()
    {
        //The grid where the ai builds
        int buildingStartX = 50, buildingEndX = 55, buildingStartY = 3, buildingEndY = 15;
        if (enemyGold >= enemyNextBuildingCost)
        {
            for (int y = buildingStartY; y < buildingEndY; y++)
            {
                for (int x = buildingStartX; x < buildingEndX; x++)
                {
                    TileHandler tile = LevelManager.Instance.Tiles[new Point(x, y)];
                    if (tile.IsEmpty)
                    {
                        //Debug.Log("Building at: " + x + ", " + y);
                        tile.PlaceBuilding(Teams.RightTeam);
                        return;
                    }
                }
            }
        }
    }

    private void EnemyBuildingAI()
    {
        Debug.Log("Current ASDASDASD: " + currentLevel);
        if (currentLevel == 1)
        {
            enemyNextBuilding = "Soldier";
            enemyNextBuildingCost = 50;
        }
        else
        {
            
            int buildingID = UnityEngine.Random.Range(1, 4);
            switch (buildingID)
            {
                case 1:
                    enemyNextBuilding = "Soldier";
                    enemyNextBuildingCost = 50;
                    break;
                case 2:
                    enemyNextBuilding = "Archer";
                    enemyNextBuildingCost = 75;
                    break;
                case 3:
                    enemyNextBuilding = "Ogre";
                    enemyNextBuildingCost = 150;                
                    break;
                case 4:
                    enemyNextBuilding = "Bat";
                    enemyNextBuildingCost = 100;
                    break;
                default:
                    enemyNextBuilding = "Soldier";
                    enemyNextBuildingCost = 50;
                    break;
            }
            Debug.Log("Next building is: " + enemyNextBuilding);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void PlayNextLevel()
    {
        PlayerPrefs.SetInt("Current Level", 2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

}

