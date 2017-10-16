using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    [SerializeField]
    private float speed;
    [SerializeField]
    public Image healthBar;

    private Stack<Node> path;
    private List<Unit> aggroList; //Keep track on enemies in aggro range
    private List<Unit> attacklist; //Keep track on enemies in attack range

    public Point GridPos { get; set; }

    //private Teams Team
    //{
    //    get
    //    {
    //        return team;
    //    }

    //    set
    //    {
    //        team = value;
    //    }
    //}

    public int Id { get; set; }

    public float Health
    {
        get
        {
            return health;
        }

        private set
        {
            health = value;
        }
    }




    //The next tile to move to
    private Vector3 destination;

    //enum Teams { LeftTeam, RightTeam };
    private Teams team;
    private Color leftColor = Color.green;
    private Color rightColor = Color.red;
    private float health;
    private float maxHealth = 100;
    private float attackDamage;
    private Unit target;
    private float attackSpeed;
    private float attackTimer;
    private bool canAttack;

    private bool targetInRange = false;
    private bool moving;

    UnitInfo unitInfo;

    private void Awake()
    {
        aggroList = new List<Unit>();
        attacklist = new List<Unit>();
        path = new Stack<Node>();
        unitInfo = GetComponent<UnitInfo>();

    }

    // Use this for initialization
    void Start ()
    {
        maxHealth = unitInfo.getMaxHealth(this.name);
        attackDamage = unitInfo.getAttackDamage(this.name);
        attackSpeed = unitInfo.getAttackSpeed(this.name);
        Health = maxHealth;
        canAttack = true;       
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        UpdateTarget();
        Attack();
        //Debug.Log("Id: " + this.Id + " is targeting: " + target);
	}


    public void Spawn(Point spawnPos, Stack<Node> path, Teams team)
    {
        //Debug.Log("SPAWNING UNIT: " + this.name + " with ID: " + Id + " at: " + spawnPos.X + ", " + spawnPos.Y);
        transform.position = LevelManager.Instance.Tiles[spawnPos].transform.position;
        GridPos = spawnPos;
        if (team == Teams.LeftTeam)
        {
            this.team = Teams.LeftTeam;
            aggroList.Add(GameManager.Instance.RightCastle);
        }
        else
        {
            this.team = Teams.RightTeam;
            aggroList.Add(GameManager.Instance.LeftCastle);

        }
        this.GetComponent<SpriteRenderer>().sortingOrder = 50; //TODO: FIX SORTING SYSTEM
        SetPath(path);
        setHealthBarColor();
        Health = maxHealth;
        UpdateHealthBar();
        

    }

    public void SetPath(Stack<Node> newPath)
    {
        if(newPath != null && newPath.Count > 0)
        {
            path = newPath;
            GridPos = path.Peek().GridPos;
            destination = path.Pop().WorldPos;
        }
    }

    private void Move()
    {
        //Debug.Log("Unit: " + this.name + " from team: " + this.Team + " moving to: " + destination);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //This function is currently not used, using aggrorangetrigger and attackrangetrigger instead
        
    }

    private void OnDeath()
    {
        //Makes the unit inactive
        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }

    //Called from child object AggroRange
    public void AggroRangeTrigger(Collider2D other)
    {
        //Found another unit
        if(other.tag == "Unit" && other.GetComponent<Unit>().team != team && !aggroList.Contains(other.GetComponent<Unit>()))
        {
            //Debug.Log("AggroTrigger on unit: " + other.GetComponent<Unit>().name + " with id: " + other.GetComponent<Unit>().Id);
            aggroList.Add(other.GetComponent<Unit>());
        }
    }

    //Called from child object AttackRange
    public void AttackRangeTrigger(Collider2D other)
    {
        if(other.tag == "Unit")
        {
           // Debug.Log("AT: " + other.name);
        }
        //if (other.tag == "Unit" && other.GetComponent<Unit>() == target)
        //{
        //    Debug.Log("AttackTrigger on unit: " + target.name + " with id: " + target.Id);
        //    targetInRange = true;
        //}
        if (other.tag == "Unit" && other.GetComponent<Unit>().team != team && !attacklist.Contains(other.GetComponent<Unit>()))
        {
            attacklist.Add(other.GetComponent<Unit>());
        }
    }

    public void AggroRangeTriggerExit(Collider2D other)
    {
        Unit u = other.GetComponent<Unit>();
        if(aggroList.Contains(u))
        {
            aggroList.Remove(u);
        }
    }
    public void AttackRangeTriggerExit(Collider2D other)
    {


        if(other.tag == "Unit")
        {
            Unit u = other.GetComponent<Unit>();
            if (aggroList.Contains(u))
            {
                aggroList.Remove(u);
            }

            //Debug.Log("Name: " + other.name);
            //Unit u = other.GetComponent<Unit>();
            //if (u.tag == "Unit" && target && u.Id == target.Id)
            //{
            //    Debug.Log("range FALSE");
            //    targetInRange = false;
            //}
        }

    }

    private void Attack()
    {
        if(!canAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackSpeed)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        //If we have a valid target
        if(target != null)
        {
            //If we are in range
            if (targetInRange)
            {
                //If the attackTimer is rdy
                if (canAttack)
                {
                    Hit();
                    canAttack = false;
                }
            }

        }
    }
    private void Hit()
    {
        
        target.TakeDamage(attackDamage);
        if(target.Health <= 0)
        {
            target = null;
        }

    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        UpdateHealthBar();
        if(Health<= 0)
        {
            OnDeath();
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = Health / maxHealth;
    }

    private void MoveToUnit(Unit target)
    {
        SetPath(LevelManager.Instance.GeneratePath(GridPos, target.GridPos));
    }

    private void StopMoving()
    {
        path = null;
    }

    private void UpdateTarget()
    {
        //Debug.Log("Id: " + this.Id + " listCount: " + enemyList.Count);
        //if the target is out of range or is dead, remove it as the current target
        //if (!enemyList.Contains(target))
        //{
        //    target = null;
        //}

        UpdateAggroList();
        UpdateAttackList();
        

        //If target is in range, stop. Otherwise move to it
        if (targetInRange && target)
        {
            //MAYBE DO TARGET STUFF HERE
            if(moving)
            {

                StopMoving(); //temp I think
                SetPath(null); //temp
                moving = false;
            }
            
        }
        else if(target && !moving)
        {
            //Debug.Log("Moving to: " + target.name + " id: " + target.Id);
            MoveToUnit(target);
            moving = true;
        }
           

    }

    private void UpdateAggroList()
    {
        //If the target no longer exists, pick the next enemy in the list
        if (target == null || !target.gameObject.activeSelf)
        {

            if (aggroList.Count > 0)
            {
                //Debug.Log("PICKING NEW TARGET");
                target = aggroList[0];
                aggroList.RemoveAt(0);
            }
        }

        //If enemies are in range but the unit is attacking the castle, change target
        if (target && target.name == "Castle" && aggroList.Count > 0)
        {
            aggroList.Add(target);
            target = aggroList[0];
            aggroList.RemoveAt(0);
           // Debug.Log("New target: " + target.name);
        }
    }

    private void UpdateAttackList()
    {
        //target exists and is in range
        if(target && attacklist.Contains(target))
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
        }
    }

    private void setHealthBarColor()
    {
        Image[] bars = this.gameObject.GetComponentsInChildren<Image>();
        //bars[0] = HealthBG, bars[0] = HealthBar
        Image healthBar = bars[1];

        if (this.team == Teams.LeftTeam)
        {
            healthBar.color = leftColor;
        }
        else
        {
            healthBar.color = rightColor;
        }
    }





}
