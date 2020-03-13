using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Torso torso;

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
}
