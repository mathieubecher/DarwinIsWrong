using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour
{
    public Mob defaultMob;
    
    public Mob.Alimentation alimentation;
    [Range(0,1)]
    public float force = 0.5f;
    [Range(0,1)]
    public float vitesse = 0.5f;
    [Range(0,1)]
    public float cycleReproduction = 0.5f;
    
    public Torso torso;

    public void CreateMonster()
    {
        //throw new System.NotImplementedException();
    }
}
