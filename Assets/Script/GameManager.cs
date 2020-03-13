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
        
        foreach (Mob mob in mobsType)
        {
            int i = Random.Range(2, 5); 
            while (i > 0)
            {
                mobs.Add(Instantiate(mob,map.MobNextPos(), Quaternion.identity,transform));
                --i;
            }
        }
    }

    private void Start()
    {
        Debug.Log(map.cubes);
    }
}
