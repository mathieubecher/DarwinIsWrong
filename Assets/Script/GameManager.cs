using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Cube defaultCube;
    public List<Cube> CubeType;
    public int WIDTH = 50;
    public Cube[,] cubes;
    
    public Camera main;
// Start is called before the first frame update
    void Awake()
    {
        cubes = new Cube[WIDTH,WIDTH];
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int z = 0; z < WIDTH; ++z)
            {
                Cube cube = Instantiate(defaultCube, new Vector3(x, 0, z), Quaternion.identity, transform);
                cubes[x, z] = cube;
                cube.SetPosition(x,z);
            }
        }

        GenerateMap();
        
        main = Camera.main;
        main.transform.position = new Vector3(WIDTH/2,0,WIDTH/2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap()
    {
        List<Cube> toTreat = new List<Cube>();
        List<Cube> treats = new List<Cube>();
        for (int i = 0; i < 5; ++i)
        {
            Cube cube = cubes[(int) (Random.value * WIDTH), (int) (Random.value * WIDTH)];
            cube = ChangeCube(cube);
            toTreat.Add(cube);
        }
        List<Cube> next = new List<Cube>();
        foreach (Cube treat in toTreat)
        {
            
        }
        toTreat = next;
        next = new List<Cube>();
    }

    Cube ChangeCube(Cube cube)
    {
        Vector2 position = cube.position;
        Destroy(cube.gameObject);
        cubes[(int)position.x, (int)position.y] = null;
        
        Cube cubeSpawn = Instantiate(defaultCube, new Vector3(position.x, position.y), Quaternion.identity, transform);
        cubes[(int)position.x, (int)position.y] = cube;
        cube.SetPosition((int)position.x, (int)position.y);
        return cubeSpawn;
    }
}
