using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Cube defaultCube;
    public GameObject baseWatter;
    public GameObject baseFloor;
    public List<Cube> cubeType;
    public List<Decoration> decorationType;
    public int nbBaseCube = 10;
    public int WIDTH = 50;
    [HideInInspector]
    public Cube[,] cubes;
    [HideInInspector]
    public GameManager manager;
    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        

        //GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap()
    {
        // Reinitialisation de la map
        while(transform.childCount > 0)
                DestroyImmediate(transform.GetChild(0).gameObject);
        GameObject watter = Instantiate(baseWatter, new Vector3(WIDTH / 2.0f - 0.5f, -0.1f, WIDTH / 2.0f - 0.5f), Quaternion.identity, transform);
        GameObject floor = Instantiate(baseFloor, new Vector3(WIDTH / 2.0f - 0.5f, -1, WIDTH / 2.0f - 0.5f), Quaternion.identity, transform);
        watter.transform.localScale = new Vector3(WIDTH-0.1f, 0.8f, WIDTH-0.1f);
        floor.transform.localScale = new Vector3(WIDTH, 1f, WIDTH);
        cubes = new Cube[WIDTH,WIDTH];
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int z = 0; z < WIDTH; ++z)
            {
                GameObject goCube = Instantiate(defaultCube.gameObject, new Vector3(x, 0, z), Quaternion.identity, transform);
                Cube cube = goCube.GetComponent<Cube>();
                cubes[x, z] = cube;
                cube.SetPosition(x,z);
            }
        }
        
        // Création des cubes de base
        List<Cube> toTreat = new List<Cube>();
        List<Cube> treats = new List<Cube>();
        float totalWeight = TypeWeight();
        for (int i = 0; i < nbBaseCube; ++i)
        {
            Cube cube = cubes[(int) (Random.value * WIDTH), (int) (Random.value * WIDTH)];
            float rand = Random.value;
            int index = -1;
            while (rand >= 0 && index < cubeType.Count)
            {
                ++index;
                rand -= cubeType[index].weight / totalWeight;

            }
            
            cube = ChangeCube(cube,cubeType[index].type);
            toTreat.Add(cube);
        }
        
        // Propagation des cubes de base
        while (toTreat.Count > 0)
        {
            List<Cube> next = new List<Cube>();
            foreach (Cube treat in toTreat)
            {
                treat.AddDecoration();
                int x = (int) treat.position.x;
                int y = (int) treat.position.y;
                if (x > 0 && cubes[x - 1, y].type == Cube.Type.none) next.Add(ChangeCube(cubes[x - 1, y], treat.type)); 
                if (x < WIDTH-1 && cubes[x + 1, y].type == Cube.Type.none) next.Add(ChangeCube(cubes[x + 1, y], treat.type)); 
                if (y > 0 && cubes[x, y - 1].type == Cube.Type.none) next.Add(ChangeCube(cubes[x, y - 1], treat.type)); 
                if (y < WIDTH-1 && cubes[x, y + 1].type == Cube.Type.none) next.Add(ChangeCube(cubes[x, y + 1], treat.type)); 
            }
            toTreat = next;
            next = new List<Cube>();
        }
    }

    Cube ChangeCube(Cube cube, Cube.Type type)
    {
        Vector2 position = cube.position;
        DestroyImmediate(cube.gameObject);
        cubes[(int)position.x, (int)position.y] = null;
        Cube typeCube = defaultCube;
        int i = 0;
        while (i < cubeType.Count && cubeType[i].type != type) ++i;
        Cube cubeSpawn = Instantiate(cubeType[i], new Vector3(position.x,0, position.y), Quaternion.identity, transform);
        cubes[(int)position.x, (int)position.y] = cubeSpawn;
        cubeSpawn.SetPosition((int)position.x, (int)position.y);

        return cubeSpawn;
    }

    float TypeWeight()
    {
        float weight = 0;
        for (int i = 0; i < cubeType.Count; ++i) weight += cubeType[i].weight;
        return weight;
    }
}
