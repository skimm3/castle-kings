using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

	public GameObject GetObject(string type)
    {
        foreach (GameObject obj in pooledObjects)
        {
           // Debug.Log("LOOKING THROUGH POOL");
            //If the unit to spawn already exists in the pool and is inactive
            if(obj.name == type && !obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }


        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if(objectPrefabs[i].name == type)
            {
                GameObject newObj = Instantiate(objectPrefabs[i]);
                pooledObjects.Add(newObj);
                newObj.name = type;
                return newObj;
            }
        }
        return null;
    }

    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}


