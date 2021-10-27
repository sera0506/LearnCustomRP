using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform prefab;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    private List<Transform> objectList;

    private void Awake()
    {
        objectList = new List<Transform>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(createKey))
        {
            CreateObject();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
    }

    private void BeginNewGame()
    {
        for(int i = 0; i < objectList.Count; i ++)
        {
            Destroy(objectList[i].gameObject);
        }
        objectList.Clear();
    }

    private void CreateObject()
    {
        Transform t = Instantiate(prefab);
        t.localPosition = Random.insideUnitSphere * 5.0f;
        t.localRotation = Random.rotation;
        objectList.Add(t);
    }
}
