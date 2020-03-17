using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Map map;
    public List<Mob> mobsType;
    [HideInInspector] public List<Mob> mobs;
    public Camera main;
    // Start is called before the first frame update
    private void Awake()
    {
        main = Camera.main;
        
    }

    private void Start()
    {
        Debug.Log(map.cubes);
    }
}
