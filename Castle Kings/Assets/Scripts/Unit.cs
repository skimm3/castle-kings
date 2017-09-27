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

    public Point GridPos { get; set; }

    private Teams Team
    {
        get
        {
            return team;
        }

        set
        {
            team = value;
        }
    }




    //The next tile to move to
    private Vector3 destination;

    enum States { MovingToCastle, MovingToUnit, AttackingUnit, AttackingCastle };
    enum Teams { LeftTeam, RightTeam };
    private States currentState;
    private Teams team;

    private float health;
    private float maxHealth = 100;
    private float attackDamage;
    private Unit target;
    private float attackSpeed;
    private float attackTimer;
    private bool canAttack;
    



    // Use this for initialization
    void Start ()
    {
        currentState = States.MovingToCastle;
        maxHealth = 100;
        health = maxHealth;
        attackDamage = 20;
        attackSpeed = 1.0f;
        canAttack = true;

	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        Attack();
	}

    public void Spawn(Point spawnPos, Stack<Node> path, string team)
    {
        //Debug.Log("SPAWNING UNIT");
        transform.position = LevelManager.Instance.Tiles[spawnPos].transform.position;
        this.GetComponent<SpriteRenderer>().sortingOrder = 50; //TODO: FIX SORTING SYSTEM
        SetPath(path);
        health = maxHealth;
        UpdateHealthBar();
        if (team == "LeftTeam")
        {
            this.Team = Teams.LeftTeam;
        }
        else
        {
            this.Team = Teams.RightTeam;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //remove when units can die/fight       
        if(collision.tag == "CastleEntrance")
        {
            Debug.Log("Found enemy castle");
            OnDeath();
        }
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
        if(other.tag == "Unit" && other.GetComponent<Unit>().Team != Team)
        {
            Debug.Log("Enemy unit in AGGRO range");
            SetState(States.MovingToUnit);
            target = other.GetComponent<Unit>();
            MoveToUnit(target);
        }
        //Found the enemy castle
        else if(other.tag == "Castle")
        {          
            //Debug.Log("Enemy castle in AGGRO range");
            SetState(States.MovingToCastle);
            //TODO: create a castle unit
            //target = other.GetComponent<Unit>();
        }
    }

    //Called from child object AttackRange
    public void AttackRangeTrigger(Collider2D other)
    {
        //Found another unit
        if (other.tag == "Unit" && other.GetComponent<Unit>().Team != Team)
        {
            
            Debug.Log("Enemy unit" + other.name + " in ATTACK range");
            SetState(States.AttackingUnit);
            StopMoving();
            SetPath(null); //temp
        }
        //Found the enemy castle
        else if (other.tag == "Castle")
        {
            //Debug.Log("Enemy castle in ATTACK range");
            SetState(States.AttackingCastle);
        }
    }

    private void SetState(States newState)
    {
        this.currentState = newState;
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
        if(target != null && currentState == States.AttackingUnit && target.gameObject.activeSelf)
        {
            if(canAttack)
            {
                Hit();
                canAttack = false;
            }
        }
    }

    private void Hit()
    {
        
        target.TakeDamage(attackDamage);
        if(target.health <= 0)
        {
            target = null;
        }
       // Debug.Log("HIT! Current hp: " + health);

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthBar();
        if(health<= 0)
        {
            OnDeath();
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = health / maxHealth;

    }

    private void MoveToUnit(Unit target)
    {
        SetPath(LevelManager.Instance.GeneratePath(GridPos, target.GridPos));
    }

    private void StopMoving()
    {
        path = null;
    }




}
