using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cube : MonoBehaviour
{
    public enum Type  { none = 0,dirt, grass, watter }

    public float weight = 0;
    [SerializeField]
    public Vector2i position;
    public Type type;
    public GameManager manager;
    [SerializeField] private Decoration onSurface;
    public Material m;
    public float timer = 0;
    public bool activeM = false;
    public Decoration OnSurface
    {
        get => onSurface;
        set
        {
            onSurface = value;
            isSurface = value != null;
        }
    }

    public bool isSurface = false;
    // Start is called before the first frame update

    private void Awake()
    {
        m = GetComponent<Renderer>().material;
        if(isSurface && onSurface == null)
        {
            
            Debug.Log("WTF");
        }
    }

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    public void AddDecoration()
    {
        
        manager = FindObjectOfType<GameManager>();
        
        float rand = Random.value;
       
        if (Random.value > 0.95)
        {
            int index = -1;
        
            List<Decoration> decorations = manager.map.decorationType;
            float totalWeight = TypeWeight(decorations);
            while (rand >= 0 && index < decorations.Count)
            {
                ++index;
                rand -= decorations[index].weight / totalWeight;

            }
            
            Decoration decorType = manager.map.decorationType[index];
            int i = 0;
            while (i < decorType.acceptFloor.Count && type != decorType.acceptFloor[i]) ++i;
            if(i<decorType.acceptFloor.Count) OnSurface = Instantiate(decorType, new Vector3(position.x,1,position.y),Quaternion.identity,transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (activeM)
        {
            timer += Time.deltaTime/2;
            m.color = Color.Lerp(Color.red, Color.green, timer);
            if (timer > 1) activeM = false;
        }
        else timer = 0;
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2i(x,y);
    }
    float TypeWeight(List<Decoration> decorations)
    {
        float weight = 0;
        for (int i = 0; i < decorations.Count; ++i) weight += decorations[i].weight;
        return weight;
    }

    public void SpawnGrassNeighbour()
    {
        List<Cube> neighbours = new List<Cube>();
        if (position.x > 0 && manager.map.GetCube((int)position.x - 1, (int)position.y).type == Type.grass 
                           && manager.map.GetCube((int)position.x - 1, (int)position.y).onSurface == null) 
            neighbours.Add(manager.map.GetCube((int)position.x - 1, (int)position.y));
        
        if (position.x < manager.map.WIDTH-1 && manager.map.GetCube((int)position.x + 1, (int)position.y).type == Type.grass 
                                             && manager.map.GetCube((int)position.x + 1, (int)position.y).onSurface == null) 
            neighbours.Add(manager.map.GetCube((int)position.x + 1, (int)position.y));
        
        if (position.y > 0 && manager.map.GetCube((int)position.x , (int)position.y - 1).type == Type.grass 
                           && manager.map.GetCube((int)position.x , (int)position.y - 1).onSurface == null)
            neighbours.Add(manager.map.GetCube((int)position.x , (int)position.y - 1));
        
        if (position.y < manager.map.WIDTH-1 && manager.map.GetCube((int)position.x , (int)position.y + 1).type == Type.grass 
                                             && manager.map.GetCube((int)position.x , (int)position.y + 1).onSurface == null) 
            neighbours.Add(manager.map.GetCube((int)position.x , (int)position.y + 1));
        
        if (neighbours.Count == 0) return;
        int index = Random.Range(0, neighbours.Count);
        neighbours[index].SpawnGrass();
    }

    public void SpawnGrass()
    {
        GrassDecoration grass = Instantiate(manager.map.decorationType.Find(x => x.type == Decoration.Type.grass), new Vector3(position.x,1,position.y),Quaternion.identity,transform) as GrassDecoration;
        OnSurface = grass;
    }

    public bool Walkable()
    {
        return type != Type.watter && (!isSurface || !onSurface.Block());
    }
}
[Serializable]
public struct Vector2i
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;

    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !(a == b);
    }

    public float Distance(Vector2i b)
    {
        return Mathf.Abs(b.x - x) + Mathf.Abs(b.y - y);
    }
}
