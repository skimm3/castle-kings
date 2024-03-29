﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHover : Singleton<BuildingHover> {

    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start () {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        FollowMouse();
	}

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    public void ActivateHover(Sprite sprite)
    {
        this.spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;

    }

    public void DeavtivateHover()
    {
        spriteRenderer.enabled = false;
        GameManager.Instance.ClickedButton = null;
    }
}
