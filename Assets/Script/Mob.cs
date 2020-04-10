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

    public int id = 0;
    public Sexe sexe = Sexe.male;
    public Alimentation alimentation = Alimentation.herbivore;
    
    [Range(0,1)]
    public float force = 0.5f;
    [Range(0,1)]
    public float vitesse = 0.5f;
    
    
    // FOOD
    [Range(0,1)] public float satiete;
    public float lastEat = 0;
    private List<Cube> grass;
    private List<Mob> mobs;
    private float stun = 0;
    
    // REPRODUCTION
    [Range(0,1)]
    public float maturationTemps = 1;
    [Range(0,1)]
    public float cycleReproduction = 0.5f;

    public float timerCycle = 0;
    [Range(0, 1)]
    public float poids;
    
    public Cube next;
    private float timeMove = 0;
    private float moveSpeed = 0.2f;
    
    [HideInInspector] public Torso torso;

    public Cube pos;
    public Cube target;
    public bool isTarget;
    public bool isGoTo;
    public bool isMobile;
    
    private CubeStar direction;
    private GameManager manager;
    
    private void OnDestroy()
    {
        Destroy(GetComponent<Renderer>().material);
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
        grass = new List<Cube>();
        mobs = new List<Mob>();
        manager = FindObjectOfType<GameManager>();
        if (target != null) isTarget = true;
        moveSpeed = 1/(vitesse*8);
        timerCycle = maturationTemps;
    }


    private void Update()
    {
        
        grass.RemoveAll(i => !i.isSurface);
        mobs.RemoveAll(i => i == null);
        
        lastEat += Time.deltaTime/60 * ((alimentation == Alimentation.carnivore)?0.5f:1);
        timerCycle -= Time.deltaTime / 60 * ((alimentation == Alimentation.carnivore)?0.5f:1);
        if (stun > 0)
        {
            stun -= Time.deltaTime;
        }
        else
        {
            if (lastEat > satiete) Dead();

            if (!isMobile && alimentation == Alimentation.herbivore && pos.isSurface && pos.OnSurface.type == Decoration.Type.grass)
            {
                lastEat = 0;
            
                GameObject eatgrass = pos.OnSurface.gameObject;
                pos.OnSurface = null;
                pos.activeM = true;
                Destroy(eatgrass);
            }

            else if (alimentation == Alimentation.carnivore && lastEat > satiete / 3)
            {
                foreach (Mob mob in mobs)
                {
                    if (alimentation == Alimentation.carnivore && mob.alimentation == Alimentation.herbivore && mob.pos == pos)
                    {
                        if (Random.value + force > Random.value + mob.force)
                        {
                            mob.Dead();
                            lastEat = 0;
                        }
                        else stun = 2;
                    }
                }
            }

            if (timerCycle < 0)
            {
                foreach (Mob mob in mobs)
                {
                    if (mob.id == id && mob.timerCycle < 0 && mob.pos == pos)
                    {
                        timerCycle = cycleReproduction;
                        mob.timerCycle = cycleReproduction;
                    
                        Mob m = Instantiate(manager.mobsType.Find(x => x.id == id), new Vector3(pos.position.x,1,pos.position.y), Quaternion.identity, manager.map.transform);
                        m.pos = pos;
                        manager.mobs.Add(m);
                    }
                }
            }
        
            bool findNext = false;
            if (!findNext && timerCycle < 0 && lastEat < satiete * 2 / 3 && !isMobile && mobs.Count > 0) findNext = FindReproduce();
            if (!findNext && lastEat > satiete / 3 && !isMobile) findNext = FindFood();
            if(!findNext) RandomDirection();

            Move();
        }
        
    }

    private void RandomDirection()
    {
        if (!isTarget && !isGoTo)
        {
            isTarget = true;
            target = manager.map.GetCube(Random.Range(0,manager.map.WIDTH),Random.Range(0,manager.map.WIDTH));
            while (!target.Walkable())
            {
                target = manager.map.GetCube(Random.Range(0,manager.map.WIDTH),Random.Range(0,manager.map.WIDTH)); 
            }
        }
       
    }

    private bool FindReproduce()
    {
        float dist = 100;
        bool find = false;
        foreach (Mob mob in mobs)
        {
            if (mob.id == id && mob.timerCycle < 0 && mob != this)
            {
                float distMob = (mob.transform.position - transform.position).magnitude;
                if (dist > distMob)
                {
                    find = true;
                    target = mob.pos;
                }
            }
        }
        if (find)
        {
            isMobile = true;
            isTarget = true;
            isGoTo = false;
            return true;
        }

        return false;
    }
    private bool FindFood()
    {

        if (alimentation == Alimentation.herbivore)
        {
            if(grass.Count == 0 || (isGoTo && direction.c.isSurface)) return false;
            else
            {
                target = grass[0];
                float dist = (target.transform.position - pos.transform.position).magnitude;
                foreach (Cube grassCube in grass)
                {
                    float grassDist = (grassCube.transform.position - pos.transform.position).magnitude;
                    if (grassDist < dist)
                    {
                        dist = grassDist;
                        target = grassCube;
                    }
                }
                isTarget = true;
                isGoTo = false;
                isMobile = true;
                return true;
            }
        }
        else
        {
            if (mobs.Count == 0) return false;
            bool find = false;
            float dist = 100;
            foreach (Mob mob in mobs)
            {
                if (mob.alimentation == Alimentation.herbivore)
                {
                    float mobDist = (mob.transform.position - this.transform.position).magnitude;
                    if (mobDist < dist)
                    {
                        dist = mobDist;
                        target = mob.pos;
                        find = true;
                    }
                }
            }
            if(find){
                isTarget = true;
                isGoTo = false;
                isMobile = true;
                return true;
            }
        }

        return false;
    }
    
    
    private void Move()
    {
        if (isTarget && !isGoTo)
        {
            direction = Goto(target);
            //direction.Draw();
            isTarget = false;
            isGoTo = true;
            
            timeMove = moveSpeed;
            --direction.depth;    
            next = direction.GetLast().c;
        }
        else if (isGoTo)
        {
            if (timeMove > 0)
            {
                isMobile = true;
                transform.position = Vector3.Lerp(pos.transform.position,next.transform.position, (moveSpeed-timeMove)/moveSpeed) + new Vector3(0,1,0);
                timeMove -= Time.deltaTime;
            }
            else
            {
                isMobile = false;
                pos = next;
                timeMove = moveSpeed;
                --direction.depth;  
                CubeStar nextStar = direction.GetLast();
                next = nextStar.c;
            }
            
            if (pos == target) isGoTo = false;
        }

    }
    
    private CubeStar Goto(Cube targetGo)
    {
        AStar star = new AStar(manager.map);
        return star.FindWay(pos, targetGo);
    }

    private CubeStar Goto(int x, int y)
    {
        if (x >= 0 && x < manager.map.WIDTH && y >= 0 && y < manager.map.WIDTH)
            return Goto(manager.map.GetCube(x, y));
        else return null;
    }

    public void Dead()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == this.gameObject) return;
        if(other.gameObject.layer == 9) grass.Add(other.gameObject.transform.parent.gameObject.GetComponent<Cube>());
        if(other.gameObject.layer == 8) mobs.Add(other.gameObject.GetComponent<Mob>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == this.gameObject) return;
        if(other.gameObject.layer == 9) grass.Remove(other.gameObject.transform.parent.gameObject.GetComponent<Cube>());
        if(other.gameObject.layer == 8) mobs.Remove(other.gameObject.GetComponent<Mob>());
    }
}















public class CubeStar : IComparable<CubeStar>
{
    public Cube c;
    public CubeStar last;
    public float dist;
    public int depth = 0;
    public CubeStar(Cube c ,CubeStar last, float dist = 0)
    {
        this.c = c;
        this.last = last;
        this.depth = last.depth + 1;
        this.dist = dist;
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

    public CubeStar GetLast(int _depth)
    {
        if (last == null || _depth <= 0) return this;
        return last.GetLast(_depth-1);
    }
    public CubeStar GetLast()
    {
        return GetLast(depth);
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
        int nbTreat = 0;
        bool vertical = Mathf.Abs(origin.position.y - target.position.y) > Mathf.Abs(origin.position.x - target.position.x);
        toTreats = new List<CubeStar>();
        treats = new List<CubeStar>();
        toTreats.Add(new CubeStar(origin));
        
        do
        {
            CubeStar actual = toTreats[0];
            //actual.c.m.color = new Color(nbTreat/((float)(_map.WIDTH*_map.WIDTH)),0,0);
            ++nbTreat;
            toTreats.Remove(actual);
            treats.Add(actual);
            if (actual.c.position == target.position)
            {
                return actual;
            }
            int x = (int)actual.c.position.x, y = (int)actual.c.position.y;
            
            if(Treatable(x - 1, y)) 
                toTreats.Add(new CubeStar(_map.GetCube(x-1,y),actual, _map.GetCube(x-1,y).position.Distance(target.position) - ((vertical)?0:0.01f)));
            
            if(Treatable(x + 1, y)) 
                toTreats.Add(new CubeStar(_map.GetCube(x+1,y),actual, _map.GetCube(x+1,y).position.Distance(target.position) - ((vertical)?0:0.01f)));
            
            if(Treatable(x, y - 1)) 
                toTreats.Add(new CubeStar(_map.GetCube(x,y-1),actual, _map.GetCube(x,y-1).position.Distance(target.position) - (!(vertical)?0:0.01f)));
            
            if(Treatable(x,y + 1)) 
                toTreats.Add(new CubeStar(_map.GetCube(x,y+1),actual, _map.GetCube(x,y+1).position.Distance(target.position) - (!(vertical)?0:0.01f)));

            toTreats.Sort();

        } while (toTreats.Count > 0);

        
        return new CubeStar(origin);
    }

    private bool Treatable(int x, int y)
    {
        if (! (x >= 0 && x < _map.WIDTH && y >= 0 && y < _map.WIDTH)) return false;
        Cube c = _map.GetCube(x, y);
        bool reachable = c.Walkable();
        
        return reachable && !treats.Exists(treat => treat.c.position.x == x && treat.c.position.y == y)
               && !toTreats.Exists(treat => treat.c.position.x == x && treat.c.position.y == y);
    }
    
}