using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mob : MonoBehaviour
{
    public enum Sexe
    {
        femele,
        male
    };

    public enum Alimentation
    {
        herbivore,
        carnivore
    };
    
    public Sexe sexe = Sexe.male;
    public Alimentation alimentation = Alimentation.herbivore;
    
    [Range(0,1)]
    public float force = 0.5f;
    [Range(0,1)]
    public float vitesse = 0.5f;
    [Range(0,1)]
    public float satiete;
    [Range(0,1)]
    public float maturationTemps = 1;
    [Range(0,1)]
    public float cycleReproduction = 0.5f;
    [Range(0, 1)]
    public float poids;

    [HideInInspector] public Torso torso;

    private void OnDestroy()
    {
        /*
        DestroyImmediate(torso.head);
        torso.members.ForEach(DestroyImmediate);
        DestroyImmediate(torso);
        DestroyImmediate(gameObject);
        */
    }

    public void ComputeStats()
    {
        sexe = (Sexe) Random.Range(0, 2);
        alimentation = torso.GetAlimentation();
        force = torso.GetForce();
        vitesse = torso.GetVitesse();
        satiete = torso.GetSatiete();
        maturationTemps = torso.GetMaturationTemps();
        cycleReproduction = torso.GetCycleReproduction();
        poids = torso.GetPoids();
    }


    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (target != null && direction == null)
        {
            direction = Goto(target);
            direction.Draw();
        }
    }

    public Cube pos;
    public Cube target;
    
    private CubeStar direction;
    private GameManager manager;
    private CubeStar Goto(Cube target)
    {
        AStar star = new AStar(manager.map);
        return star.FindWay(pos, target);
    }

    private CubeStar Goto(int x, int y)
    {
        if (x >= 0 && x < manager.map.WIDTH && y >= 0 && y < manager.map.WIDTH)
            return Goto(manager.map.GetCube(x, y));
        else return null;
    }
}

public class CubeStar : IComparable<CubeStar>
{
    public Cube c;
    public CubeStar last;
    public float dist;

    public CubeStar(Cube c ,CubeStar last, float dist = 0)
    {
        this.c = c;
        this.last = last;
    }

    public CubeStar(Cube c)
    {
        this.c = c;
    }
    // Default comparer for Part type.
    public int CompareTo(CubeStar b)
    {
        // A null value means that this object is greater.
        if (b == null)
            return 1;

        else  return (dist < b.dist) ? -1 : ((dist > b.dist) ? 1 : 0);
    }

    public void Draw()
    {
        if (last == null) return;
        Debug.DrawLine(new Vector3(c.position.x,1,c.position.y),new Vector3(last.c.position.x,1,last.c.position.y),Color.red,60);
        last.Draw();
    }

    public string Print()
    {
        string print = "[" + c.position.x + ", " + c.position.y + "]";
        if (last == null) return print;
        return last.Print() + " -> " + print;
    }
}

public class AStar
{
    private Map _map;

    public AStar(Map map)
    {
        _map = map;
    }

    private List<CubeStar> toTreats;
    private List<CubeStar> treats;
    public CubeStar FindWay(Cube origin, Cube target)
    {
        toTreats = new List<CubeStar>();
        treats = new List<CubeStar>();
        toTreats.Add(new CubeStar(origin));
        do
        {
            CubeStar actual = toTreats[0];
            toTreats.Remove(actual);
            treats.Add(actual);
            if (actual.c.position == target.position) return actual;
            int x = (int)actual.c.position.x, y = (int)actual.c.position.y;
            
            if(Treatable(x - 1, y)) 
                toTreats.Add(new CubeStar(_map.GetCube(x-1,y),actual, _map.GetCube(x-1,y).position.Distance(target.position)));
            
            if(Treatable(x + 1, y)) 
                toTreats.Add(new CubeStar(_map.GetCube(x+1,y),actual, _map.GetCube(x+1,y).position.Distance(target.position)));
            
            if(Treatable(x, y - 1)) 
                toTreats.Add(new CubeStar(_map.GetCube(x,y-1),actual, _map.GetCube(x,y-1).position.Distance(target.position)));
            
            if(Treatable(x,y + 1)) 
                toTreats.Add(new CubeStar(_map.GetCube(x,y+1),actual, _map.GetCube(x,y+1).position.Distance(target.position)));

            treats.Sort();

        } while (toTreats.Count > 0);

        return null;
    }

    private bool Treatable(int x, int y)
    {
        return x >= 0 && x < _map.WIDTH && y >= 0 && y < _map.WIDTH  && _map.GetCube(x,y).Walkable() && (_map.GetCube(x,y).onSurface == null || !_map.GetCube(x,y).onSurface.Block())
               && !treats.Exists(treat => treat.c.position.x == x && treat.c.position.y == y)
               && !toTreats.Exists(treat => treat.c.position.x == x && treat.c.position.y == y);
    }
    
}