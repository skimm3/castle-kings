using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour {

    [SerializeField]
    private GameObject buildingPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceText;


    public GameObject BuildingPrefab
    {
        get
        {
            return buildingPrefab;
        }

    }

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }

    }

    public int Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
        }
    }

    private void Start()
    {
        priceText.text = "$" + Price;
    }
}
