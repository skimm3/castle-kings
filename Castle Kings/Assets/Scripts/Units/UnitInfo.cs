using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour {
    enum unitData { MaxHealth, AttackDamage, AttackSpeed };

    private Dictionary<unitData, float> Castle = new Dictionary<unitData, float>
    {
        { unitData.MaxHealth, 2500},
        { unitData.AttackDamage, 0},
        { unitData.AttackSpeed, 2}
    };
    private Dictionary<unitData, float> Soldier = new Dictionary<unitData, float>
    {
        { unitData.MaxHealth, 100},
        { unitData.AttackDamage, 10},
        { unitData.AttackSpeed, 1}
    };
    private Dictionary<unitData, float> Archer = new Dictionary<unitData, float>
    {
        { unitData.MaxHealth, 50},
        { unitData.AttackDamage, 20},
        { unitData.AttackSpeed, 1.5f}
    };
    private Dictionary<unitData, float> Ogre = new Dictionary<unitData, float>
    {
        { unitData.MaxHealth, 250},
        { unitData.AttackDamage, 30},
        { unitData.AttackSpeed, 2}
    };
    private Dictionary<unitData, float> Bat = new Dictionary<unitData, float>
    {
        { unitData.MaxHealth, 75},
        { unitData.AttackDamage, 10},
        { unitData.AttackSpeed, 0.5f}
    };


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float getMaxHealth(string unit)
    {
        switch(unit)
        {
            case "Soldier":
                return Soldier[unitData.MaxHealth];
            case "Castle":
                return Castle[unitData.MaxHealth];
            case "Archer":
                return Archer[unitData.MaxHealth];
            case "Ogre":
                return Ogre[unitData.MaxHealth];
            case "Bat":
                return Bat[unitData.MaxHealth];
            default:
                return 0;
        }
    }

    public float getAttackDamage(string unit)
    {
        switch (unit)
        {
            case "Soldier":
                return Soldier[unitData.AttackDamage];
            case "Castle":
                return Castle[unitData.AttackDamage];
            case "Archer":
                return Archer[unitData.AttackDamage];
            case "Ogre":
                return Ogre[unitData.AttackDamage];
            case "Bat":
                return Bat[unitData.AttackDamage];
            default:
                return 0;
        }
    }

    public float getAttackSpeed(string unit)
    {
        switch (unit)
        {
            case "Soldier":
                return Soldier[unitData.AttackSpeed];
            case "Castle":
                return Castle[unitData.AttackSpeed];
            case "Archer":
                return Archer[unitData.AttackSpeed];
            case "Ogre":
                return Ogre[unitData.AttackSpeed];
            case "Bat":
                return Bat[unitData.AttackSpeed];
            default:
                return 0;
        }
    }
 

}
