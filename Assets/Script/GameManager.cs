using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Map map;
    public Camera main;
    // Start is called before the first frame update
    private void Awake()
    {
        main = Camera.main;
        main.transform.position = new Vector3(map.WIDTH/2,main.transform.position.y,map.WIDTH/2);
    }
}
