using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAggro : MonoBehaviour {

    private Unit u;
	// Use this for initialization
	void Start () {
        u = this.GetComponentInParent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        u.AggroRangeTrigger(collision);
    }
    

}
