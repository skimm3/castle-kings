﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed = 0;

    [SerializeField]
    private float edgeDistance = 0;



    private float xMax;
    private float yMin;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update () {
        GetInput();
	}

    private void GetInput()
    {
        if(Input.GetKey(KeyCode.UpArrow) || (Input.mousePosition.y > Screen.height - edgeDistance))
        {
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || (Input.mousePosition.x < edgeDistance))
        {
            transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow) || (Input.mousePosition.y < edgeDistance))
        {
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow) || (Input.mousePosition.x > Screen.width - edgeDistance))
        {
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, xMax), Mathf.Clamp(transform.position.y, yMin, 0), -10);
    }

    //Restrict the camera movement to the map
    public void SetLimits(Vector3 maxTile)
    {
        Vector3 wp = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)); //Bottom-right corner

        xMax = maxTile.x - wp.x;
        yMin = maxTile.y - wp.y;
    }
}
