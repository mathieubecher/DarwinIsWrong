using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Cube defaultCube;
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

        main = Camera.main;
        main.transform.position = new Vector3(WIDTH/2,0,WIDTH/2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
