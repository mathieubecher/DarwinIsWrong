using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Cube defaultCube;
    public List<Cube> cubeType;
    public int WIDTH = 50;
    public Cube[,] cubes;
    [HideInInspector]
    public GameManager manager;
    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
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
            int index = (int) Mathf.Floor(Random.value * cubeType.Count);
            Debug.Log(index);
            cube = ChangeCube(cube,cubeType[index]);
            toTreat.Add(cube);
        }
        List<Cube> next = new List<Cube>();
        foreach (Cube treat in toTreat)
        {
            
        }
        toTreat = next;
        next = new List<Cube>();
    }

    Cube ChangeCube(Cube cube, Cube type)
    {
        Vector2 position = cube.position;
        Destroy(cube.gameObject);
        cubes[(int)position.x, (int)position.y] = null;
        
        Cube cubeSpawn = Instantiate(type, new Vector3(position.x,0, position.y), Quaternion.identity, transform);
        cubes[(int)position.x, (int)position.y] = cubeSpawn;
        cubeSpawn.SetPosition((int)position.x, (int)position.y);
        return cubeSpawn;
    }
}
