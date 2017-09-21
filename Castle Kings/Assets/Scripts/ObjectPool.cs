using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefabs;

	public GameObject GetObject(string type)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if(objectPrefabs[i].name == type)
            {
                GameObject newObj = Instantiate(objectPrefabs[i]);
                newObj.name = type;
                return newObj;
            }
        }
        return null;
    }
}
