using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public enum CubeType  { dirt, grass, watter }
    public Vector2 position;
    public CubeType type;
    public GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2(x,y);
    }
}
