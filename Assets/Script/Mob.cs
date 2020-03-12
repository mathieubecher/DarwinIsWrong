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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
